using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.Common
{
    public class Account
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Cash { get; set; }
        public decimal NetValue { get; set; }
        public IEnumerable<Position> Positions { get; set; }
    }
}
