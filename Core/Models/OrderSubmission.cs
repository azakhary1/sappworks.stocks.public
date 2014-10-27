
namespace Sappworks.Stocks
{
    public class OrderSubmission
    {
        public string Symbol { get; set; }
        public int OrderNumber { get; set; }
        public double EstimatedCommission { get; set; }
        public double EstimatedTotalAmount { get; set; }
        public int Quantity { get; set; }
        public string OrderAction { get; set; }
    }
}
