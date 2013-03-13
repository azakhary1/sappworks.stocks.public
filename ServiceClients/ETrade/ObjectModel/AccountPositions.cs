
namespace Stocks.ServiceClients.ETrade.ObjectModel
{
    partial class AccountPositionsResponse : IResource, IBelongToAccountService
    {
        private const string ResourceNameFormatString = "/accounts/rest/accountpositions/{accountId}";
        private const string SandboxResourceNameFormatString = "/accounts/sandbox/rest/accountpositions/{accountId}";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }
    }
}
