
namespace Stocks.Common
{
    public static class OAuthTokenHelpers
    {
        public static bool IsSet(this OAuthToken token)
        {
            return (token != null && !string.IsNullOrWhiteSpace(token.Token) && !string.IsNullOrWhiteSpace(token.Secret));
        }
    }
}
