
namespace Sappworks.Stocks.ETrade
{
    partial class AccountListResponse : IResource, IBelongToAccountService
    {
        #region IResource Stuff

        private const string ResourceNameFormatString = "/accounts/rest/accountlist";
        private const string SandboxResourceNameFormatString = "/accounts/sandbox/rest/accountlist";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }

        #endregion
    }
}
