using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;


namespace InfoProxy
{
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class MyHttpClient
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public WebProxy Proxy { get; set; }

        public MyHttpClient()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "application/json.";
            PostData = "";
        }
        public MyHttpClient(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "application/json.";
            PostData = "";
        }
        public MyHttpClient(string endpoint, HttpVerb method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json.";
            PostData = "";
        }

        public MyHttpClient(string endpoint, HttpVerb method, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json.";
            PostData = postData;
        }


        public string MakeRequest(out CookieCollection cookies)
        {
            return MakeRequest("", out cookies);
        }

        public string MakeRequest(string parameters, out CookieCollection cookies, Cookie cookie = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            request.CookieContainer = new CookieContainer();
            if (cookie != null)
            {
                request.CookieContainer.Add(cookie);
            }

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;
            if (!String.IsNullOrEmpty(this.User) && !String.IsNullOrEmpty(this.Password))
            {
                request.Credentials = new NetworkCredential(this.User, this.Password);
            }

            if (Proxy != null)
            {
                request.Proxy = Proxy;
            }

            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;
                cookies = response.Cookies;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                return responseValue;
            }
        }


        public void SetCredentials(string usr, string pwd)
        {
            this.User = usr;
            this.Password = pwd;
        }

    } // class

}