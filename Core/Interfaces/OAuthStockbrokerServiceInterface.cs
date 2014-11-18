
namespace Sappworks.Stocks
{
    using System.Collections.Generic;

    public interface OAuthStockbrokerServiceInterface
    {
        IEnumerable<OrderSubmission> ExecuteOrders(uint accountId, IEnumerable<Order> orders);
        OAuthToken GetAccessToken(string verificationKey);
        Account GetAccount(uint accountId = 0);
        IEnumerable<Position> GetQuotes(IEnumerable<Position> positions, QuoteType quoteType = default(QuoteType));
        IEnumerable<Quote> GetQuotes(IEnumerable<string> symbols, QuoteType quoteType = default(QuoteType));
        string GetUserAuthorizationUrl();
        IEnumerable<string> GetOpenOrderSymbols(uint accountId = 0);
        bool AccessTokenIsSet { get; }
    }
}
