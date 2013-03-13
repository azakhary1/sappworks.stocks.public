
namespace Stocks.Common
{
    public class OrderSubmission
    {
        public string Symbol { get; set; }
        public int OrderNumber { get; set; }
        public decimal EstimatedCommission { get; set; }
        public decimal EstimatedTotalAmount { get; set; }
        public int Quantity { get; set; }
        public string OrderAction { get; set; }
    }
}
