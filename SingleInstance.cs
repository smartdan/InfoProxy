using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace InfoProxy
{
    public sealed class SingleInstance
    {
        // Members
        private static volatile SingleInstance _the;
        private static object syncRoot = new Object();
        private PublicMarket _market;
        private PrivateMarket _buysell;
        public static Logger Log = LogManager.GetCurrentClassLogger();

        // .Ctors
        private SingleInstance()
        {
            _market = new PublicMarket();
            _buysell = new PrivateMarket();
        }

        // Singleton pattern
        public static SingleInstance The
        {
            get
            {
                if (_the == null)
                {
                    lock (syncRoot)
                    {
                        if (_the == null)
                            _the = new SingleInstance();
                    }
                }

                return _the;
            }
        }

        // Properties
        public PublicMarket Market
        {
            get { return _market; }
        }

        // Properties
        public PrivateMarket BuySell
        {
            get { return _buysell; }
        }
    }
}

