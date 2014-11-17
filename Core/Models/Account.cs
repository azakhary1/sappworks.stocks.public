
namespace Sappworks.Stocks
{
    using System.Collections.Generic;

    public class Account
    {
        public uint Id { get; set; }
        public string Description { get; set; }
        public double Cash { get; set; }
        public double NetValue { get; set; }
        public IEnumerable<Position> Positions { get; set; }
        public bool IsMargin { get; set; }
        public double MarginableSecurities { get; set; }
        public double MarginEquity { get; set; }
    }
}
