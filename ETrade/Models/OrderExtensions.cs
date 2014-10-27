using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sappworks.Stocks.ETrade
{
    public static class OrderExtensions
    {
        public static IEnumerable<PlaceEquityOrder> ToEquityOrderRequest(this IEnumerable<Order> orders, uint accountId)
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

            List<PlaceEquityOrder> convertedOrders = null;

            if (orders != null)
            {
                convertedOrders = orders.Select(
                    o => new PlaceEquityOrder
                    {
                        Item = new PlaceEquityOrderEquityOrderRequest 
                        {
                            accountId = accountId,
                            clientOrderId = DateTime.Now.ToFileTime().ToString(CultureInfo.InvariantCulture),
                            limitPrice = o.Price,
                            marketSession = "REGULAR",
                            orderAction = o.IsSale ? "SELL" : "BUY",
                            orderTerm = "GOOD_FOR_DAY",
                            priceType = "LIMIT",
                            quantity = o.Quantity,
                            symbol = o.Symbol
                        }
                    }
                ).ToList();
            }

            return convertedOrders;
        }

    }
}
