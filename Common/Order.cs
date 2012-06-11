using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.Common
{
    public class Order
    {
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public bool IsSale { get; set; }
        public decimal Price { get; set; }
    }
}
