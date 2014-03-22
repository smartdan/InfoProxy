using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;

namespace InfoProxy.Charts
{
    public static class ChartUtils
    {
        /*
          ###data###
          var data = google.visualization.arrayToDataTable([
          ['Mon', 20, 28, 38, 45],
          ['Tue', 31, 38, 55, 66],
          ['Wed', 50, 55, 77, 80],
          ['Thu', 77, 77, 66, 50],
          ['Fri', 68, 66, 22, 12],
		  ['Fri', 70, 66, 22, 15]
          // Treat first row as data as well.
        ], true);  
        */

        /*
        // Set chart options
        ###options###
        var options = {'title':'Financial candlestick chart',
		'legend':'left',
		'is3D':true,
        'width':800,
        'height':600};*/

        /*
         function drawChart() {
        var data = google.visualization.arrayToDataTable([
          ['Year', 'Sales', 'Expenses'],
          ['2004',  1000,      400],
          ['2005',  1170,      460],
          ['2006',  660,       1120],
          ['2007',  1030,      540]
        ]);


        var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
        chart.draw(data, options);
      }
         */

        // Charts
        public static string GetTransactionsChart(List<Transact> transactions)
        {
            var html = File.ReadAllText(@".\Charts\chartTransactions.html");

            // Set chart options
            var options = @" var options = {" +
            //",hAxis: {title: 'Time', titleTextStyle: {color: 'red'}} " +
            //",vAxis: { title: 'Price' }" +
            " 'legend': 'bottom' " +
            //", 'width': '600px'" +
            //", 'height': '300px'" +
            "};";

            html = html.Replace("###options###", options);

            var data = @"var data = google.visualization.arrayToDataTable([";
            data += Environment.NewLine;
            data += @"  ['Date', 'Price'],";

            foreach (var tr in transactions.OrderBy(t => t.date))
            {
                data += Environment.NewLine;
                var datetime = UnixTimestampToDateTime(long.Parse(tr.date));
                data += String.Format("['{0}', {1}],", datetime.ToShortTimeString(), tr.price);
            }

            data = data.Remove(data.Length - 1, 1);
            data += Environment.NewLine;
            data += @"]);";
            html = html.Replace("###data###", data);

            return html;
        }
        public static string GetArrowChart(Tick tick)
        {
            var html = File.ReadAllText(@".\Charts\chartArrow.html");

            var data = @" var data = new google.visualization.DataTable(); " +
                  "data.addColumn('string', 'Price'); " +
                  "data.addColumn('number', 'Ask/Last'); " +
                  "data.addRows([";

            data += Environment.NewLine;

            var variation = (decimal.Parse(tick.ask) / decimal.Parse(tick.last)) - 1;

            data += (@"['" + tick.last + @"', { v: " + variation + ", f: '" + (decimal)variation + "%'}]");
            data += Environment.NewLine;
            data += "]);";

            html = html.Replace("###data###", data);

            return html;

        }
        public static string GetGaugeChart(Tick tick)
        {
            var html = File.ReadAllText(@".\Charts\chartGauge.html");

            /*
            ['Last', ###last###],
            ['High', ###high###],
            ['Low', ###low###]
            */
            html = html.Replace("###last###", tick.last);
            html = html.Replace("###high###", tick.high);
            html = html.Replace("###low###", tick.low);

            return html;
        }
        public static string GetOrdersChart(OrderBook orders, Tick tick)
        {
            var html = File.ReadAllText(@".\Charts\chartColorBars.html");

            /*
            ['Last', ###last###],
            ['High', ###high###],
            ['Low', ###low###]
            */

            //###data###
            //['a', 14],
            //['j', 48]
            var data = "";
            var serializer = new JavaScriptSerializer();
            var bids = orders.bids.Take(8).ToArray().Reverse();
            foreach (var obbb in bids)
            {
                data += String.Format("['{0}', {1}],", obbb[0].TrimEnd(), obbb[1].TrimEnd());
                data += Environment.NewLine;
            }
            var asks = orders.asks;

            if (asks.Count > 0)
            {
                html = html.Replace("###last###", asks.First()[0].TrimEnd());
            }
            else
            {
                html = html.Replace("###last###", "0");
            }
            foreach (var asss in asks.Take(8))
            {
                data += String.Format("['{0}', {1}],", asss[0].TrimEnd(), asss[1].TrimEnd());
                data += Environment.NewLine;
            }

            var index = data.LastIndexOf(",");
            data = data.Remove(index, 1);
            html = html.Replace("###data###", data);

            return html;
        }

        #region Utils

        public static DateTime UnixTimestampToDateTime(long _UnixTimeStamp)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(_UnixTimeStamp).ToLocalTime();
        }

        #endregion
    }
}
