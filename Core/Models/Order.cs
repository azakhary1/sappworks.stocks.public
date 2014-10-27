
namespace Sappworks.Stocks
{
    public class Order
    {
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public bool IsSale { get; set; }
        public double Price { get; set; }
    }
}
