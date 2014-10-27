
namespace Sappworks.Stocks.ETrade
{
    partial class AccountBalanceResponse : IResource, IBelongToAccountService
    {
        #region IResource Stuff

        private const string ResourceNameFormatString = "/accounts/rest/accountbalance/{accountId}";
        private const string SandboxResourceNameFormatString = "/accounts/sandbox/rest/accountbalance/{accountId}";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }

        #endregion
    }
}
