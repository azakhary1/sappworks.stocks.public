using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevDefined.OAuth.Framework;

using Stocks.Common;

using Stocks.ServiceClients.ETrade;
using Stocks.ServiceClients.ETrade.ObjectModel;
using Stocks.ServiceClients.Yahoo;

namespace Stocks.DAL
{
    public class StocksRepository
    {
        private ETradeClient _etradeClient;

        private List<Stocks.ServiceClients.Yahoo.ObjectModel.Quote> _yahooQuotes;

        private DateTime _lastYahooQuotesGet;

        public StocksRepository(Common.OAuthToken consumerToken, Common.OAuthToken accessToken = null, bool productionMode = false)
        {
            var cToken = new ConsumerToken { Token = consumerToken.Token, TokenSecret = consumerToken.Secret };
            var aToken = accessToken != null ? new AccessToken { Token = accessToken.Token, TokenSecret = accessToken.Secret } : null;

            _etradeClient = new ETradeClient(cToken, aToken, productionMode);
        }

        public IEnumerable<Common.Position> GetQuotes(IEnumerable<Common.Position> positions)
        {
            var positionsDictionary = positions.ToDictionary(k => k.Symbol, StringComparer.OrdinalIgnoreCase);

            if (
                (_yahooQuotes == null)
                || (_lastYahooQuotesGet.Date != DateTime.Today)
                || (_yahooQuotes.Count != positionsDictionary.Count)
                || (positionsDictionary.Keys.Except(_yahooQuotes.Select(c => c.Symbol), StringComparer.OrdinalIgnoreCase).Count() > 0)
            )
            {
                var yquotes = YahooClient.GetQuotes(positionsDictionary.Keys);

                if (yquotes != null)
                {
                    _yahooQuotes = yquotes.ToList();
                    _lastYahooQuotesGet = DateTime.Now;
                }
                else
                {
                    _yahooQuotes = null;
                }
            }

            var query =
                new
                {
                    symbols = positionsDictionary.Keys.Aggregate((a, b) => a + "," + b),
                    quoteType = "ALL" //Enum.GetName(typeof(QuoteType), QuoteType.All).ToUpper()
                };

            var quotes = from q in _etradeClient.Get<QuoteResponse>(query).QuoteData
                         select new Common.Quote
                         {
                             FiftyTwoWeekHigh = q.all.high52,
                             FiftyTwoWeekLow = q.all.low52,
                             Price = q.all.lastTrade,
                             Symbol = q.product.symbol
                         };

            foreach (var pd in positionsDictionary.Keys)
            {
                positionsDictionary[pd].Quote = quotes.FirstOrDefault(q => string.Equals(q.Symbol, pd, StringComparison.OrdinalIgnoreCase));

                if (positionsDictionary[pd].Quote != null)
                {
                    positionsDictionary[pd].Quote.MovingAverage = _yahooQuotes.FirstOrDefault(yq => string.Equals(yq.Symbol, pd, StringComparison.OrdinalIgnoreCase)).TwoHunderedDayMovingAverage ?? 0m;
                }
            }

            return positionsDictionary.Values.AsEnumerable();
        }

        public Common.Account GetAccountById(int accountId)
        {
            var account = (from a in _etradeClient.Get<AccountListResponse>().Account
                           where (a.accountId == accountId)
                           let b = _etradeClient.Get<AccountBalanceResponse>(new { accountId = a.accountId.ToString() })
                           let p = _etradeClient.Get<AccountPositionsResponse>(new { accountId = a.accountId.ToString() }).AccountPositions
                           select new Common.Account
                           {
                               Id = Convert.ToInt32(a.accountId),
                               Description = a.accountDesc,
                               NetValue = a.netAccountValue,
                               Cash = b != null ? b.accountBalance.netCash : 0m,
                               Positions = 
                                    p != null ? 
                                    p.Select(
                                        ap => new Common.Position
                                        {
                                            Basis = ap.costBasis,
                                            Description = ap.description,
                                            Quantity = ap.qty,
                                            Symbol = ap.productId.symbol
                                    }).ToList() 
                                    : null
                           }).FirstOrDefault();

            var orderListResponse = _etradeClient.Get<GetOrderListResponse>(new { accountId = accountId }).orderListResponse;

            if (orderListResponse.orderDetails != null)
            {
                var symbolsWithOpenOrders =
                    from olr in orderListResponse.orderDetails
                    let old = olr.order.legDetails
                    where (
                        old != null
                        && olr.order.orderStatus == "OPEN"
                    )
                    select old.First().symbolInfo.symbol;

                foreach (var p in account.Positions)
                {
                    p.OutsandingOrdersExist = symbolsWithOpenOrders.Contains(p.Symbol);
                }
            }

            return account;
        }

        public IEnumerable<Common.Account> GetAccounts()
        {
            var accounts = (from a in _etradeClient.Get<AccountListResponse>().Account
                            let b = _etradeClient.Get<AccountBalanceResponse>(new { accountId = a.accountId.ToString() })
                            select new Common.Account
                            {
                                Id = Convert.ToInt32(a.accountId),
                                Description = a.accountDesc,
                                NetValue = a.netAccountValue,
                                Cash = b != null ? b.accountBalance.netCash : 0m,
                            }).ToDictionary(k => k.Id);

            foreach (var accountId in accounts.Keys)
            {
                var positionsResponse = _etradeClient.Get<AccountPositionsResponse>(new { accountId = accountId.ToString() });
                if (positionsResponse.count > 0)
                {
                    accounts[accountId].Positions = (from p in positionsResponse.AccountPositions
                                                     select new Common.Position
                                                     {
                                                         Basis = p.costBasis,
                                                         Description = p.description,
                                                         Quantity = p.qty,
                                                         Symbol = p.productId.symbol
                                                     }).ToList();
                }
            }

            //string test = GetAccountBalanceAsString(accounts.Keys.First());  // this works

            //List<Account> finalAccounts = new List<Account>();
            //foreach (var account in accounts)
            //{
            //    account.Cash = Get

            //    // a way to get Moving Averages:  http://finance.yahoo.com/d/quotes.csv?s=MSFT,CSCO&f=l1m3m7
            //}

            return accounts.Values;
        }

        public IEnumerable<Common.OrderSubmission> ExecuteOrders(int accountId, IEnumerable<Common.Order> orders)
        {
            var equityOrderRequests = orders.ToEquityOrderRequest(accountId);

            var responses = new List<Common.OrderSubmission>();

            foreach (var eor in equityOrderRequests)
            {
                var peor = _etradeClient.Post<EquityOrderRequest, PlaceEquityOrderResponse>(eor).equityOrderResponse;

                responses.Add(
                    new OrderSubmission
                    {
                        EstimatedCommission = peor.estimatedCommission,
                        EstimatedTotalAmount = peor.estimatedTotalAmount,
                        OrderAction = peor.orderAction,
                        OrderNumber = peor.orderNum,
                        Quantity = peor.quantity,
                        Symbol = peor.symbol.ToUpper()
                    }
                );
            }

            return responses;
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
