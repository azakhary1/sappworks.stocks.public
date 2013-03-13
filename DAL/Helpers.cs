
namespace Stocks.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Stocks.ServiceClients.ETrade.ObjectModel;

    static class Helpers
    {
        public static IEnumerable<EquityOrderRequest> ToEquityOrderRequest(this IEnumerable<Common.Order> orders, int accountId)
        {
            // Required bits:
            //   accountId
            //   quantity 
            //   symbol 
            //   orderAction
            //   priceType 
            //   marketSession 
            //   previewId 
            //   orderTerm
            //   clientOrderId

            //   limitPrice - required because these are limit orders

            List<EquityOrderRequest> convertedOrders = null;

            if (orders != null)
            {
                convertedOrders = orders.Select(
                    o => new EquityOrderRequest
                    {
                        accountId = (uint) accountId, 
                        clientOrderId = DateTime.Now.ToFileTime().ToString(CultureInfo.InvariantCulture), 
                        limitPrice = o.Price, 
                        marketSession = "REGULAR", 
                        orderAction = o.IsSale ? "SELL" : "BUY", 
                        orderTerm = "GOOD_FOR_DAY", 
                        previewId = null, 
                        priceType = "LIMIT", 
                        quantity = o.Quantity, 
                        symbol = o.Symbol
                    }
                ).ToList();
            }

            return convertedOrders;
        }
    }
}
