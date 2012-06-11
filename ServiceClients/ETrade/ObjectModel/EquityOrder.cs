using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.ServiceClients.ETrade.ObjectModel
{
    public partial class EquityOrderRequest : IResource, IRequest
    {
        #region IRequest stuff

        string IRequest.ToXml()
        {
            //return "<PlaceEquityOrder xmlns=\"http://order.etws.etrade.com\">" +
            //    "<EquityOrderRequest>" +
            //        "<accountId>83495799</accountId><clientOrderId>45</clientOrderId>" +
            //    "<limitPrice>3</limitPrice>" +
            //    "<previewId></previewId>" +
            //    "<stopPrice></stopPrice>" +
            //    "<stopLimitPrice></stopLimitPrice>" +
            //    "<allOrNone></allOrNone>" +
            //    "<quantity>4</quantity>" +
            //    "<reserveOrder></reserveOrder>" +
            //    "<reserveQuantity></reserveQuantity>" +
            //    "<symbol>ETFC</symbol>" +
            //    "<orderAction>BUY</orderAction>" +
            //    "<priceType>LIMIT</priceType>" +
            //    "<routingDestination></routingDestination>" +
            //    "<marketSession>REGULAR</marketSession>" +
            //    "<orderTerm>GOOD_FOR_DAY</orderTerm>" +
            //"</EquityOrderRequest>" +
            //"</PlaceEquityOrder>";

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
                accountId,
                clientOrderId,
                limitPrice,
                quantity,
                symbol,
                orderAction,
                priceType,
                marketSession,
                orderTerm
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
