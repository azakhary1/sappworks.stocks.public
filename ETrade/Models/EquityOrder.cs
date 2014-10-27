
namespace Sappworks.Stocks.ETrade
{
    public partial class PlaceEquityOrder : IResource, IRequest, IBelongToOrderService
    {
        #region IRequest stuff

        string IRequest.ToXml()
        {

            return string.Format(
                "<PlaceEquityOrder xmlns=\"http://order.etws.etrade.com\">" + 
                "  <EquityOrderRequest>" + 
                "    <accountId>{0}</accountId>" + 
                "    <clientOrderId>{1}</clientOrderId>" +
                "    <limitPrice>{2}</limitPrice>" +
                "    <previewId></previewId>" +
                "    <stopPrice></stopPrice>" +
                "    <stopLimitPrice></stopLimitPrice>" +
                "    <allOrNone></allOrNone>" +
                "    <quantity>{3}</quantity>" +
                "    <reserveOrder></reserveOrder>" +
                "    <reserveQuantity></reserveQuantity>" +
                "    <symbol>{4}</symbol>" +
                "    <orderAction>{5}</orderAction>" +
                "    <priceType>{6}</priceType>" +
                "    <routingDestination></routingDestination>" +
                "    <marketSession>{7}</marketSession>" +
                "    <orderTerm>{8}</orderTerm>" +
                "  </EquityOrderRequest>" +
                "</PlaceEquityOrder>",
                Item.accountId,
                Item.clientOrderId,
                Item.limitPrice,
                Item.quantity,
                Item.symbol,
                Item.orderAction,
                Item.priceType,
                Item.marketSession,
                Item.orderTerm
            );
        }

        #endregion

        #region IResource Stuff

        private const string ResourceNameFormatString = "/order/rest/placeequityorder";
        private const string SandboxResourceNameFormatString = "/order/sandbox/rest/placeequityorder";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }

        #endregion
    }
}
