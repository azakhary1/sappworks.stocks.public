
namespace Sappworks.Stocks
{
    using System;

    public class Quote
    {
        public string Symbol { get; set; }
        public double Price { get; set; }
        public double MovingAverage { get; set; }
        public double FiftyTwoWeekHigh { get; set; }
        public double FiftyTwoWeekLow { get; set; }
        public DateTime Date { get; set; }
        public double AnnualDividendPercent { get; set; }
    }
}
