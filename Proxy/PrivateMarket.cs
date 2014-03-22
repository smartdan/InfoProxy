using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace InfoProxy
{

    public class PrivateMarket
    {
        // API METHODS
        public dynamic GetBalance(UserData.UserData user)
        {
            var path = "https://www.bitstamp.net/api/balance/";

            /*
                Returns JSON dictionary:
                usd_balance - USD balance
                btc_balance - BTC balance
                usd_reserved - USD reserved in open orders
                btc_reserved - BTC reserved in open orders
                usd_available- USD available for trading
                btc_available - BTC available for trading
                fee - customer trading fee          
             */
            var nonce = GetNonce();
            var signature = GetSignature(nonce, user);
            var balance = "";
            using (WebClient client = new WebClient())
            {

                byte[] response = client.UploadValues(path, new NameValueCollection()
                {
                   { "key", user.Key },
                   { "signature", signature },
                   { "nonce", nonce.ToString()}
                });

                balance = System.Text.Encoding.Default.GetString(response);
            }

            dynamic setbalance = JsonConvert.DeserializeObject(balance);

            return setbalance;
        }

        public dynamic GetUserTransactions(UserData.UserData user, string skip = "0", string limit = "20", string sort = "desc")
        {
            var path = "https://www.bitstamp.net/api/user_transactions/";
            /*
                offset - skip that many transactions before beginning to return results. Default: 0.
                limit - limit result to that many transactions. Default: 100.
                sort - sorting by date and time (asc - ascending; desc - descending). Default: desc.
              
                Returns descending JSON list of transactions. Every transaction (dictionary) contains:
                datetime - date and time
                id - transaction id
                type - transaction type (0 - deposit; 1 - withdrawal; 2 - market trade)
                usd - USD amount
                btc - BTC amount
                fee - transaction fee
                order_id - executed order id

            */
            var nonce = GetNonce();
            var signature = GetSignature(nonce, user);
            var transactions = "";
            using (WebClient client = new WebClient())
            {

                byte[] response = client.UploadValues(path, new NameValueCollection()
                {
                   { "key", user.Key },
                   { "signature", signature },
                   { "nonce", nonce.ToString()},
                   { "limit", limit.ToString()},
                   { "skip", skip.ToString()},
                   { "sort", sort.ToString()}
                });

                transactions = System.Text.Encoding.Default.GetString(response);
            }

            dynamic settransactions = JsonConvert.DeserializeObject(transactions);

            return settransactions;
        }

        public dynamic GetUserOpenOrders(UserData.UserData user)
        {
            var path = "https://www.bitstamp.net/api/open_orders/";
            /*
                Returns JSON list of open orders. Each order is represented as dictionary:
                id - order id
                datetime - date and time
                type - buy or sell (0 - buy; 1 - sell)
                price - price
                amount - amount
            */
            var nonce = GetNonce();
            var signature = GetSignature(nonce, user);
            var orders = "";

            using (WebClient client = new WebClient())
            {

                byte[] response = client.UploadValues(path, new NameValueCollection()
                {
                   { "key", user.Key },
                   { "signature", signature },
                   { "nonce", nonce.ToString()}
                });

                orders = System.Text.Encoding.Default.GetString(response);
            }

            dynamic dynorders = JsonConvert.DeserializeObject(orders);

            return dynorders;
        }


        public dynamic CancelOrders(UserData.UserData user, string id)
        {
            var path = "https://www.bitstamp.net/api/cancel_order/";
            /*
               Returns 'true' if order has been found and canceled.
            */
            var nonce = GetNonce();
            var signature = GetSignature(nonce, user);
            var cancel = "";

            using (WebClient client = new WebClient())
            {

                byte[] response = client.UploadValues(path, new NameValueCollection()
                {
                   { "key", user.Key },
                   { "signature", signature },
                   { "nonce", nonce.ToString()},
                   { "id", id.ToString()}
                });

                cancel = System.Text.Encoding.Default.GetString(response);
            }

            dynamic dyncancel = JsonConvert.DeserializeObject(cancel);

            return dyncancel;
        }

        public dynamic BuyLimit(UserData.UserData user, string amount, string price)
        {
            var path = "https://www.bitstamp.net/api/buy/";
            /*
               POST https://www.bitstamp.net/api/buy/Params:
                key - API key
                signature - signature
                nonce - nonce
                amount - amount
                price - price

                Returns JSON dictionary representing order:
                id - order id
                datetime - date and time
                type - buy or sell (0 - buy; 1 - sell)
                price - price
                amount - amount
            */
            var nonce = GetNonce();
            var signature = GetSignature(nonce, user);
            var buylimit = "";

            using (WebClient client = new WebClient())
            {

                byte[] response = client.UploadValues(path, new NameValueCollection()
                {
                   { "key", user.Key },
                   { "signature", signature },
                   { "nonce", nonce.ToString()},
                    { "amount", amount.ToString()},
                     { "price", price.ToString()}
                });

                buylimit = System.Text.Encoding.Default.GetString(response);
            }

            dynamic dynbuylimit = JsonConvert.DeserializeObject(buylimit);

            return dynbuylimit;
        }


        public dynamic SellLimit(UserData.UserData user, string amount, string price)
        {
            var path = "https://www.bitstamp.net/api/sell/";
            /*
              SELL LIMIT ORDER
                POST https://www.bitstamp.net/api/sell/Params:
                key - API key
                signature - signature
                nonce - nonce
                amount - amount
                price - price
             * 
                Returns JSON dictionary representing order:
                id - order id
                datetime - date and time
                type - buy or sell (0 - buy; 1 - sell)
                price - price
                amount - amount

            */
            var nonce = GetNonce();
            var signature = GetSignature(nonce, user);
            var selllimit = "";

            using (WebClient client = new WebClient())
            {

                byte[] response = client.UploadValues(path, new NameValueCollection()
                {
                   { "key", user.Key },
                   { "signature", signature },
                   { "nonce", nonce.ToString()},
                    { "amount", amount.ToString()},
                     { "price", price.ToString()}
                });

                selllimit = System.Text.Encoding.Default.GetString(response);
            }

            dynamic dynsellimit = JsonConvert.DeserializeObject(selllimit);

            return dynsellimit;
        }


        #region UTILS

        public int GetNonce()
        {
            return (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static String sign(String key, String stringToSign)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            byte[] keyByte = encoding.GetBytes(key);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);

            return Convert.ToBase64String(hmacsha256.ComputeHash(encoding.GetBytes(stringToSign)));
        }

        private string GetSignature(int nonce, UserData.UserData user)
        {
            string msg = string.Format("{0}{1}{2}",
                nonce,
                user.Usr,
                user.Key);

            return ByteArrayToString(SignHMACSHA256(
                user.Sec, StrinToByteArray(msg))).ToUpper();
        }

        public static byte[] SignHMACSHA256(String key, byte[] data)
        {
            HMACSHA256 hashMaker = new HMACSHA256(Encoding.ASCII.GetBytes(key));
            return hashMaker.ComputeHash(data);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] StrinToByteArray(string str)
        {
            return System.Text.Encoding.ASCII.GetBytes(str);
        }
        #endregion
    }
}
