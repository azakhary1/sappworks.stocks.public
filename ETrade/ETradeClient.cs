
namespace Sappworks.Stocks.ETrade
{
    using DevDefined.OAuth.Consumer;
    using DevDefined.OAuth.Framework;

    using MoreLinq;
    using System.Data;

    using Sappworks.Stocks;
    
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Xml;
    using System.Xml.Serialization;

    public class ETradeClient : OAuthStockbrokerServiceInterface
    {
        private const string
            RequestUrl = "https://etws.etrade.com/oauth/request_token",
            AuthorizeUrl = "https://us.etrade.com/e/t/etws/authorize",
            AccessTokenUrl = "https://etws.etrade.com/oauth/access_token",
            DataUrl = "https://etws.etrade.com",
            SandboxDataUrl = "https://etwssandbox.etrade.com",
            RenewAccessTokenUrl = "https://etws.etrade.com/oauth/renew_access_token";

        private readonly OAuthSession _session;
        private readonly OAuthConsumerContext _consumerContext;
        private RequestToken _requestToken;
        private readonly ConsumerToken _consumerToken;
        private readonly bool _productionMode;

        private AccessToken _accessToken;

        public bool AccessTokenIsSet
        {
            get 
            { 
                return _accessToken.IsSet(); 
            }
        }

        // Rate Limits:
        // Orders          2 incoming requests per second per user, 7000 per hour
        // Accounts        2 incoming requests per second per user, 7000 per hour
        // Quotes          4 incoming requests per second per user, 14000 per hour
        // Notifications   2 incoming requests per second (per user?)
        private readonly static TokenBucket
            _ordersTokenBucket = new TokenBucket(2, new TimeSpan(0, 0, 0, 1, 46)),
            _accountsTokenBucket = new TokenBucket(2, new TimeSpan(0, 0, 0, 1, 102)),
            _quotesTokenBucket = new TokenBucket(4, new TimeSpan(0, 0, 0, 1, 46));



        public ETradeClient(Stocks.OAuthToken consumerToken, bool productionMode = false, Stocks.OAuthToken accessToken = null)
        {
            _consumerToken = new ConsumerToken { Token = consumerToken.Token, TokenSecret = consumerToken.Secret };

            _consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = _consumerToken.Token,
                ConsumerSecret = _consumerToken.TokenSecret,
                SignatureMethod = SignatureMethod.HmacSha1,
                UseHeaderForOAuthParameters = true,
                CallBack = "oob"
            };

            _session = new OAuthSession(_consumerContext, RequestUrl, AuthorizeUrl, AccessTokenUrl, RenewAccessTokenUrl);

            _productionMode = productionMode;

            if (accessToken != null)
            {
                _accessToken = new AccessToken { Token = accessToken.Token, TokenSecret = accessToken.Secret};
            }
        }

        private void GetRequestToken(Uri callbackUri = null)
        {
            if (!_consumerToken.IsSet()) { throw new OAuthGetRequestTokenException("Consumer token and secret are required."); }
            if (_consumerContext == null) { throw new OAuthGetRequestTokenException("Consumer context is not set up."); }
            if (_session == null) { throw new OAuthGetRequestTokenException("OAuthSession is not estabblished"); }

            //// debug only
            //_session.ResponseBodyAction = ResponseBodyAction;

            try
            {
                lock (_locker)
                {
                    if (callbackUri != null)
                    {
                        // in desktop program the user can copy/paste the verification key, or 
                        // in a web application, etrade can automatically redirect the user back 
                        // to your application with verification key included in the querystring

                        _session.CallbackUri = callbackUri;
                    }

                    _requestToken = _session.GetRequestToken().ToRequestToken();
                }
            }
            catch (OAuthException ex)
            {
                var exception = new OAuthGetRequestTokenException(ex.Message, ex);

                MapProblemReport(ex, exception);

                throw exception;
            }
        }

        private void MapProblemReport(OAuthException oauthException, AuthenticationException coreException)
        {
            if (oauthException.Report != null)
            {
                coreException.AcceptableVersionTo = oauthException.Report.AcceptableVersionTo;
                coreException.AcceptableVersionFrom = oauthException.Report.AcceptableVersionFrom;
                coreException.ParametersRejected = oauthException.Report.ParametersRejected;
                coreException.ParametersAbsent = oauthException.Report.ParametersAbsent;
                coreException.ProblemAdvice = oauthException.Report.ProblemAdvice;
                coreException.Problem = oauthException.Report.Problem;
                coreException.AcceptableTimeStampsTo = oauthException.Report.AcceptableTimeStampsTo;
                coreException.AcceptableTimeStampsFrom = oauthException.Report.AcceptableTimeStampsFrom;
            }

            if (oauthException.Context != null)
            {
                coreException.RequestUri = oauthException.Context.RawUri;
                coreException.AuthorizationHeaders = oauthException.Context.AuthorizationHeaderParameters.AllKeys.Select(k => k + ": " + oauthException.Context.AuthorizationHeaderParameters[k]);
            }
        }

        //public void ResponseBodyAction(string action)
        //{
        //    Console.WriteLine(action);
        //}

        public string GetUserAuthorizationUrl()
        {
            if (_requestToken == null || _requestToken.Expired)
            {
                GetRequestToken();
            }

            return _session.GetUserAuthorizationUrlForToken(_consumerToken.Token, _requestToken);
        }

        private readonly object _locker = new object();

        public Stocks.OAuthToken GetAccessToken(string verificationKey)
        {
            if (!_consumerToken.IsSet()) { throw new OAuthGetAccessTokenException("Consumer token and secret are required."); }
            if (!_requestToken.IsSet()) { throw new OAuthGetAccessTokenException("Request token not set, you need to try getting the verification key again."); }
            if (_requestToken.Expired) { throw new OAuthGetAccessTokenException("Request token has expired, you need to try getting the verification key again."); }
            if (string.IsNullOrWhiteSpace(verificationKey)) { throw new OAuthGetAccessTokenException("Verification key is required you need to get it from etrade first."); }
            if (_session == null) { throw new OAuthGetRequestTokenException("OAuthSession is not estabblished"); }

            try
            {
                lock (_locker)
                {
                    _accessToken = _session.ExchangeRequestTokenForAccessToken(_requestToken, verificationKey).ToAccessToken();
                }

                return new Stocks.OAuthToken
                {
                    Token = _accessToken.Token,
                    Secret = _accessToken.TokenSecret
                };
            }
            catch (OAuthException ex)
            {
                var exception = new OAuthGetAccessTokenException(ex.Message, ex);

                MapProblemReport(ex, exception);

                throw exception;                
            }
        }

        private static void ObeyRequestRateLimits(Type t)
        {
            if (t is IBelongToAccountService)
            {
                // get from accounts bucket
                _accountsTokenBucket.Consume();
            }
            else if (t is IBelongToMarketService)
            {
                // get from market bucket
                _quotesTokenBucket.Consume();
            }
            else if (t is IBelongToOrderService)
            {
                // get from order bucket
                _ordersTokenBucket.Consume();
            }
        }

        private T Get<T>(object queryData = null) where T : IResource, new()
        {
            var resourceType = typeof(T);

            string url = GetUrl<T>(queryData);

            var serializer = new XmlSerializer(resourceType);

            ObeyRequestRateLimits(resourceType);

            try
            {
                using (var response = _session.Request(_accessToken).Get().ForUrl(url).ToWebResponse())
                using (var responseStream = response.GetResponseStream())
                using (var responseMemoryStream = new MemoryStream())
                {
                    responseStream.CopyTo(responseMemoryStream);
                    responseMemoryStream.Position = 0;

                    try
                    {
                        return (T)serializer.Deserialize(responseMemoryStream);
                    }
                    catch (InvalidOperationException ex)
                    {
                        responseStream.Position = 0;

                        using (var streamReader = new StreamReader(responseMemoryStream))
                        {
                            throw new DeserializeException(
                                ex.Message,
                                streamReader.ReadToEnd(),
                                ex
                            );
                        }
                    }
                }
            }
            catch (OAuthException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("401"))
                {
                    throw new AuthenticationException(ex.Message, ex);
                }

                throw;
            }
        }

        private TResult Post<TRequest, TResult>(TRequest request) where TRequest : IResource, IRequest, new() 
        {
            var resourceType = typeof(TResult);

            string url = GetUrl<TRequest>();
                        
            //var serializer = new XmlSerializer(typeof(TRequest));

            //string requestXml;

            //using (StringWriter textWriter = new StringWriter())
            //{
            //    using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
            //    {
            //        serializer.Serialize(xmlWriter, request);
            //    }

            //    requestXml = textWriter.ToString();
            //}
                        
            ObeyRequestRateLimits(resourceType);
            
            using (var webResponse = _session.Request(_accessToken).Post().ForUrl(url).WithRawContentType("application/xml").WithRawContent(request.ToXml(), Encoding.UTF8).ToWebResponse())
            using (var responseStream = webResponse.GetResponseStream())
            using (var responseMemoryStream = new MemoryStream())
            {
                responseStream.CopyTo(responseMemoryStream);
                responseMemoryStream.Position = 0; 

                try
                {
                    var deserializer = new XmlSerializer(typeof(TResult));

                    return (TResult)deserializer.Deserialize(responseMemoryStream);
                }
                catch (InvalidOperationException ex)
                {
                    responseMemoryStream.Position = 0;

                    using (var streamReader = new StreamReader(responseMemoryStream))
                    {
                        throw new DeserializeException(
                            ex.Message,
                            streamReader.ReadToEnd(),
                            ex
                        );
                    }
                }
            }
        }

        private string GetUrl<T>(object queryData = null) where T : IResource, new()
        {
            string resourceName = new T().GetResourceName(_productionMode);

            return (_productionMode ? DataUrl : SandboxDataUrl) + (queryData != null ? resourceName.Inject(queryData) : resourceName);
        }

        public IEnumerable<Position> GetQuotes(IEnumerable<Position> positions, QuoteType quoteType = default(QuoteType))
        {
            if (positions == null || !positions.Any())
            {
                return positions;
            }

            var positionsDictionary = positions.ToDictionary(k => k.Symbol, StringComparer.OrdinalIgnoreCase);

            var quotes = GetQuotes(positionsDictionary.Keys, quoteType);

            foreach (var pd in positionsDictionary.Keys)
            {
                positionsDictionary[pd].Quote = quotes.FirstOrDefault(q => string.Equals(q.Symbol, pd, StringComparison.OrdinalIgnoreCase));
            }

            return positionsDictionary.Values;
        }

        const int maxQuotesPerRequest = 25;

        public IEnumerable<Quote> GetQuotes(IEnumerable<string> symbols, QuoteType quoteType = default(QuoteType))
        {
            if (symbols == null || !symbols.Any())
            {
                return null;
            }

            if (symbols.Count() > maxQuotesPerRequest)
            {
                var quotes = new List<Quote>();

                foreach (var quoteBatch in symbols.Batch(maxQuotesPerRequest))
                {
                    quotes.AddRange(GetQuotesInternal(quoteBatch, quoteType));
                }

                return quotes;
            }
            else
            {
                return GetQuotesInternal(symbols, quoteType);
            }
        }

        private IEnumerable<Quote> GetQuotesInternal(IEnumerable<string> symbols, QuoteType quoteType)
        {
            var query =
                    new
                    {
                        symbols = string.Join(",", from s in symbols select s),
                        quoteType = quoteType.ToEtradeQuoteType()
                    };

            var quoteResponse = Get<QuoteResponse>(query);

            return  quoteResponse.QuoteData
                .Select(
                    q => new Quote
                    {
                        FiftyTwoWeekHigh = q.all.high52,
                        FiftyTwoWeekLow = q.all.low52,
                        Price = q.all.lastTrade,
                        Symbol = q.product.symbol,
                        AnnualDividendPercent = q.all.annualDividend / q.all.lastTrade
                    }
                )
                .ToList();
        }

        public IEnumerable<string> GetOpenOrderSymbols(uint accountId = 0)
        {
            if (accountId == 0)
            {
                accountId = Get<AccountListResponse>().Account.FirstOrDefault().accountId;
            }

            var orderListResponse = Get<GetOrderListResponse>(new { accountId }).orderListResponse;

            var symbols = new List<string>();

            if (orderListResponse.orderDetails != null)
            {
                symbols.AddRange(
                    from olr in orderListResponse.orderDetails
                    let old = olr.order.legDetails
                    where (
                        old != null
                        && olr.order.orderStatus == "OPEN"
                    )
                    select old.First().symbolInfo.symbol
                );
            }

            return symbols;
        }

        public Account GetAccount(uint accountId = 0)
        {
            var accountResponse = Get<AccountListResponse>().Account.FirstOrDefault(a => accountId == 0 || a.accountId == accountId);

            var balanceResponse = Get<AccountBalanceResponse>(new { accountId = accountResponse.accountId.ToString(CultureInfo.InvariantCulture) });

            var positionsResponse = Get<AccountPositionsResponse>(new { accountId = accountResponse.accountId.ToString(CultureInfo.InvariantCulture) });

            var isMargin = (accountResponse.marginLevel == "MARGIN");

            var accountPositions = 
                (positionsResponse != null && positionsResponse.AccountPositions != null) ? 
                    positionsResponse.AccountPositions.Select(
                        ap => new Position
                        {
                            Basis = ap.costBasis,
                            Description = ap.description,
                            Quantity = ap.qty,
                            Symbol = ap.productId.symbol
                        }
                    ).ToArray() 
                    : null;

            var symbolsWithOpenOrders = GetOpenOrderSymbols(accountResponse.accountId).ToArray();

            foreach (var p in accountPositions)
            {
                p.OutsandingOrdersExist = symbolsWithOpenOrders.Contains(p.Symbol);
            }

            var account = new Account
            {
                Id = accountResponse.accountId,
                Description = accountResponse.accountDesc,
                NetValue = accountResponse.netAccountValue,
                Positions = accountPositions,
                IsMargin = isMargin
            };

            if (balanceResponse != null)
            {
                account.Cash = balanceResponse.accountBalance.netCash;

                if (isMargin == true && balanceResponse.marginAccountBalance != null)
                {
                    account.MarginableSecurities = balanceResponse.marginAccountBalance.marginableSecurities;
                    account.MarginEquity = balanceResponse.marginAccountBalance.marginEquity;
                }
            }

            return account;
        }

        public IEnumerable<OrderSubmission> ExecuteOrders(uint accountId, IEnumerable<Order> orders)
        {
            var equityOrderRequests = orders.ToEquityOrderRequest(accountId);

            return equityOrderRequests
                .Select(eor => Post<PlaceEquityOrder, PlaceEquityOrderResponse>(eor).Item)
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
    }
}
