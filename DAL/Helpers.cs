using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Stocks;
using Stocks.ServiceClients.ETrade.ObjectModel;

namespace Stocks.DAL
{
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
                convertedOrders = new List<EquityOrderRequest>();

                foreach (var o in orders)
                {
                    convertedOrders.Add(
                        new EquityOrderRequest()
                        {
                            accountId = (uint)accountId,
                            clientOrderId = DateTime.Now.ToFileTime().ToString(),
                            limitPrice = o.Price,
                            marketSession = "REGULAR",
                            orderAction = o.IsSale ? "SELL" : "BUY",
                            orderTerm = "GOOD_FOR_DAY",
                            previewId = null,
                            priceType = "LIMIT",
                            quantity = o.Quantity,
                            symbol = o.Symbol
                        }
                    );

                }
            }

            return convertedOrders;
        }
    }
}
