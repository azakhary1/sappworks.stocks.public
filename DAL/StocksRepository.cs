
namespace Stocks.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    using Stocks.Common;
    using Stocks.ServiceClients.ETrade;
    using Stocks.ServiceClients.ETrade.ObjectModel;

    public class StocksRepository
    {
        private readonly ETradeClient _etradeClient;

        public StocksRepository(Common.OAuthToken consumerToken, Common.OAuthToken accessToken = null, bool productionMode = false)
        {
            var cToken = new ConsumerToken { Token = consumerToken.Token, TokenSecret = consumerToken.Secret };
            var aToken = accessToken != null ? new AccessToken { Token = accessToken.Token, TokenSecret = accessToken.Secret } : null;

            _etradeClient = new ETradeClient(cToken, aToken, productionMode);
        }

        public IEnumerable<Position> GetQuotes(IEnumerable<Position> positions)
        {
            var positionsDictionary = positions.ToDictionary(k => k.Symbol, StringComparer.OrdinalIgnoreCase);

            var query =
                new
                {
                    symbols = positionsDictionary.Keys.Aggregate((a, b) => a + "," + b),
                    quoteType = Enum.GetName(typeof(QuoteType), QuoteType.All).ToUpper()
                };

            var quotes = (from q in _etradeClient.Get<QuoteResponse>(query).QuoteData
                         select new Quote
                         {
                             FiftyTwoWeekHigh = q.all.high52,
                             FiftyTwoWeekLow = q.all.low52,
                             Price = q.all.lastTrade,
                             Symbol = q.product.symbol
                         })
                         .ToList();

            foreach (var pd in positionsDictionary.Keys)
            {
                positionsDictionary[pd].Quote = quotes.FirstOrDefault(q => string.Equals(q.Symbol, pd, StringComparison.OrdinalIgnoreCase));
            }

            return positionsDictionary.Values.AsEnumerable();
        }

        public Account GetAccountById(int accountId)
        {
            var account = (from a in _etradeClient.Get<AccountListResponse>().Account
                           where (a.accountId == accountId)
                           let b = _etradeClient.Get<AccountBalanceResponse>(new { accountId = a.accountId.ToString(CultureInfo.InvariantCulture) })
                           let p = _etradeClient.Get<AccountPositionsResponse>(new { accountId = a.accountId.ToString(CultureInfo.InvariantCulture) }).AccountPositions
                           select new Account
                           {
                               Id = Convert.ToInt32(a.accountId),
                               Description = a.accountDesc,
                               NetValue = a.netAccountValue,
                               Cash = b != null ? b.accountBalance.netCash : 0m,
                               Positions = 
                                    p != null ? 
                                    p.Select(
                                        ap => new Position
                                        {
                                            Basis = ap.costBasis,
                                            Description = ap.description,
                                            Quantity = ap.qty,
                                            Symbol = ap.productId.symbol
                                    }).ToList() 
                                    : null
                           }).FirstOrDefault();

            var orderListResponse = _etradeClient.Get<GetOrderListResponse>(new { accountId }).orderListResponse;

            if (orderListResponse.orderDetails != null)
            {
                var symbolsWithOpenOrders = (
                        from olr in orderListResponse.orderDetails
                        let old = olr.order.legDetails
                        where (
                            old != null
                            && olr.order.orderStatus == "OPEN"
                        )
                        select old.First().symbolInfo.symbol
                    )
                    .ToList();

                Debug.Assert(account != null, "account != null");

                foreach (var p in account.Positions)
                {
                    p.OutsandingOrdersExist = symbolsWithOpenOrders.Contains(p.Symbol);
                }
            }

            return account;
        }

        public IEnumerable<Account> GetAccounts()
        {
            var accounts = (from a in _etradeClient.Get<AccountListResponse>().Account
                            let b = _etradeClient.Get<AccountBalanceResponse>(new { accountId = a.accountId.ToString(CultureInfo.InvariantCulture) })
                            select new Account
                            {
                                Id = Convert.ToInt32(a.accountId),
                                Description = a.accountDesc,
                                NetValue = a.netAccountValue,
                                Cash = b != null ? b.accountBalance.netCash : 0m,
                            }).ToDictionary(k => k.Id);

            foreach (var accountId in accounts.Keys)
            {
                var positionsResponse = _etradeClient.Get<AccountPositionsResponse>(new { accountId = accountId.ToString(CultureInfo.InvariantCulture) });
                if (positionsResponse.count > 0)
                {
                    accounts[accountId].Positions = (from p in positionsResponse.AccountPositions
                                                     select new Position
                                                     {
                                                         Basis = p.costBasis,
                                                         Description = p.description,
                                                         Quantity = p.qty,
                                                         Symbol = p.productId.symbol
                                                     }).ToList();
                }
            }

            return accounts.Values;
        }

        public IEnumerable<OrderSubmission> ExecuteOrders(int accountId, IEnumerable<Order> orders)
        {
            var equityOrderRequests = orders.ToEquityOrderRequest(accountId);

            return equityOrderRequests.Select(
                eor => _etradeClient.Post<EquityOrderRequest, PlaceEquityOrderResponse>(eor).equityOrderResponse)
                .Select(
                    peor => new OrderSubmission
                    {
                        EstimatedCommission = peor.estimatedCommission, 
                        EstimatedTotalAmount = peor.estimatedTotalAmount, 
                        OrderAction = peor.orderAction, 
                        OrderNumber = peor.orderNum, 
                        Quantity = peor.quantity, 
                        Symbol = peor.symbol.ToUpper()
                    }
                ).ToList();
        }

        public Common.OAuthToken GetAccessToken(string verificationKey)
        {
            return _etradeClient.GetAccessToken(verificationKey).ToAccessToken();
        }

        public string GetUserAuthorizationUrl()
        {
            return _etradeClient.GetUserAuthorizationUrlForToken();
        }
    }
}
