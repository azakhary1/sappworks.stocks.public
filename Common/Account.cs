
namespace Stocks.Common
{
    using System.Collections.Generic;

    public class Account
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Cash { get; set; }
        public decimal NetValue { get; set; }
        public IEnumerable<Position> Positions { get; set; }
    }
}
