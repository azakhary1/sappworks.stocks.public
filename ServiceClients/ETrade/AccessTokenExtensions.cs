
namespace Stocks.ServiceClients.ETrade
{
    public static class AccessTokenExtensions
    {
        public static Common.OAuthToken ToAccessToken(this AccessToken value)
        {
            return
                new Common.OAuthToken
                {
                    Token = value.Token,
                    Secret = value.TokenSecret
                };
        }
    }
}
