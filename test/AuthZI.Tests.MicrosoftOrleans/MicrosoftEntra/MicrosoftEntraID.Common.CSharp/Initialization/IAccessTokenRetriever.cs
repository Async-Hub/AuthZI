using System.Threading.Tasks;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;

public interface IAccessTokenRetriever
{
  Task<string> GetAccessTokenForUserAsync(string appName, string userName);
}