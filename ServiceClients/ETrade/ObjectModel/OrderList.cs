
namespace Stocks.ServiceClients.ETrade.ObjectModel
{
    partial class GetOrderListResponse : IResource, IBelongToOrderService
    {
        private const string ResourceNameFormatString = "/order/rest/orderlist/{accountId}";
        private const string SandboxResourceNameFormatString = "/order/sandbox/rest/orderlist/{accountId}";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }
    }
}
