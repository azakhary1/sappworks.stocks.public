
namespace Stocks.ServiceClients.ETrade.ObjectModel
{
    partial class AccountListResponse : IResource, IBelongToAccountService
    {
        private const string ResourceNameFormatString = "/accounts/rest/accountlist";
        private const string SandboxResourceNameFormatString = "/accounts/sandbox/rest/accountlist";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }
    }
}
