
namespace Sappworks.Stocks.ETrade
{
    public static class AccessTokenExtensions
    {
        public static Stocks.OAuthToken ToAccessToken(this AccessToken value)
        {
            return
                new Stocks.OAuthToken
                    {
                        Token = value.Token,
                        Secret = value.TokenSecret
                    };
        }
    }
}
