using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;
using System.IO;
using Newtonsoft.Json;
using InfoProxy.Charts;

namespace InfoProxy
{
    /// <summary>
    /// BtspProxy
    /// REQUEST LIMITS: < 600 req/10min
    /// </summary>
    public class PublicMarket
    {
        #region PUBLIC Methods

        /// <summary>
        /// Ticker
        /// </summary>
        /// <returns>Tick</returns>
        public Tick GetTick()
        {
            CookieCollection cookies = new CookieCollection();

            String URI = @"https://www.bitstamp.net/api/ticker";

            try
            {              
                var client = new MyHttpClient(URI);

                var result = client.MakeRequest(out cookies);
                JavaScriptSerializer js = new JavaScriptSerializer();
                var tick = js.Deserialize(result, typeof(Tick)) as Tick;
                return tick;
            }
            catch (Exception exception)
            {
                SingleInstance.Log.Debug(exception.Message);
                return new Tick();
            }
        }

        /// <summary>
        /// OrderBook
        /// </summary>
        /// <returns>OrderBook</returns>
        public OrderBook GetOrderBook()
        {
            String URI = @"https://www.bitstamp.net/api/order_book/?group=1";

            try
            {
                CookieCollection cookies = new CookieCollection();

                var client = new MyHttpClient(URI);
                var result = client.MakeRequest(out cookies);
                var orderBook = JsonConvert.DeserializeObject<OrderBook>(result);
                return orderBook;

                #region WEBCLIENT

                // Create web client simulating IE6.
                //using (WebClient client = new WebClient())
                //{
                //    //client.Headers["User-Agent"] =
                //    //"Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
                //    //"(compatible; MSIE 6.0; Windows NT 5.1; " +
                //    //".NET CLR 1.1.4322; .NET CLR 2.0.50727)";

                //    // Download data.
                //    client.Proxy = WebRequest.DefaultWebProxy;
                //    byte[] arr = client.DownloadData(URI);

                //    // Write values.
                //    var result = Convert.ToString(arr);
                //    var orderBook = JsonConvert.DeserializeObject<OrderBook>(result);
                //    return orderBook;
                //}

                #endregion
            }
            catch (Exception exception)
            {
                SingleInstance.Log.Debug(exception.Message);
                return new OrderBook();
            }
        }

        /// <summary>
        /// Transactions
        /// </summary>
        /// <returns>Transactions</returns>
        public List<Transact> GetTransaction(int delta = 3600)
        {
            String URI = @"https://www.bitstamp.net/api/transactions/?timedelta=" + delta;

            try
            {
                CookieCollection cookies = new CookieCollection();

                var client = new MyHttpClient(URI);
                var result = client.MakeRequest(out cookies);
                var trans = JsonConvert.DeserializeObject<List<Transact>>(result);
                return trans;
            }
            catch (Exception exception)
            {
                SingleInstance.Log.Debug(exception.Message);
                return new List<Transact>();
            }
        }

        /// <summary>
        /// Reserve
        /// </summary>
        /// <returns>Reserve</returns>
        public Reserve GetReserve()
        {
            CookieCollection cookies = new CookieCollection();

            String URI = @"https://www.bitstamp.net/api/bitinstant/";

            try
            {
                var client = new MyHttpClient(URI);
                var result = client.MakeRequest(out cookies);
                var reserve = JsonConvert.DeserializeObject<Reserve>(result);
                return reserve;
            }
            catch (Exception exception)
            {
                SingleInstance.Log.Debug(exception.Message);
                return new Reserve();
            }
        }

        /// <summary>
        /// Conversion
        /// </summary>
        /// <returns>Conversion</returns>
        public Conversion GetConversion()
        {
            CookieCollection cookies = new CookieCollection();

            String URI = @"https://www.bitstamp.net/api/eur_usd/";

            try
            {
                var client = new MyHttpClient(URI);
                var result = client.MakeRequest(out cookies);
                var conv = JsonConvert.DeserializeObject<Conversion>(result);
                return conv;
            }
            catch (Exception exception)
            {
                SingleInstance.Log.Debug(exception.Message);
                return new Conversion();
            }
        }

        #endregion


        public void EvaluateBook(OrderBook orders, out bool toBuy, out bool toSell)
        {
            toBuy = false;
            toSell = false;

            double totAsk = 0.0;
            double totBid = 0.0;

            double price = 0.0;
            double amount = 0.0;

            var bids = orders.bids.Take(100).ToArray().Reverse();
            foreach (var obbb in bids)
            {
                price = double.Parse(obbb[0].TrimEnd());
                amount = double.Parse(obbb[1].TrimEnd());
                totBid += amount;
            }

            var asks = orders.asks;
            foreach (var asss in asks.Take(100))
            {
                price = double.Parse(asss[0].TrimEnd());
                amount = double.Parse(asss[1].TrimEnd());
                totAsk += amount;
            }

            toBuy = false;
            toSell = false;

            return;
        }
    }



    //DTO
    public class Tick
    {
        public string high { get; set; }
        public string last { get; set; }
        public string bid { get; set; }
        public string volume { get; set; }
        public string low { get; set; }
        public string ask { get; set; }
        public string timestamp { get; set; }

        public override string ToString()
        {
            try
            {
                return String.Format("High: {0}, Last {1}, Low {2}, Ask {3},Bid {4}, Volume {5}, Time {6}",
                    high, last, low, ask, bid, volume, ChartUtils.UnixTimestampToDateTime(long.Parse(timestamp)));
            }
            catch
            {
                return "Tick:";
            }
        }
    }

    public class OrderBook
    {
        public string timestamp { get; set; }
        public List<string[]> bids { get; set; }
        public List<string[]> asks { get; set; }
    }

    public class Transact
    {
        public string date;
        public string tid;
        public string price;
        public string amount;
    }

    public class Reserve
    {
        public string usd;
    }

    public class Conversion
    {
        public string buy;
        public string sell;
    }



}
