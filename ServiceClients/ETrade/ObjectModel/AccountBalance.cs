
namespace Stocks.ServiceClients.ETrade.ObjectModel
{
    partial class AccountBalanceResponse : IResource, IBelongToAccountService
    {
        private const string ResourceNameFormatString = "/accounts/rest/accountbalance/{accountId}";
        private const string SandboxResourceNameFormatString = "/accounts/sandbox/rest/accountbalance/{accountId}";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }
    }
}
