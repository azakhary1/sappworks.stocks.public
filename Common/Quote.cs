using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.Common
{
    public class Quote
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal MovingAverage { get; set; }
        public decimal FiftyTwoWeekHigh { get; set; }
        public decimal FiftyTwoWeekLow { get; set; }
    }
}
