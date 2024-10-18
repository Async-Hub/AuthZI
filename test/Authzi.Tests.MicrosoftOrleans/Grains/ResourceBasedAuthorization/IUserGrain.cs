using System.Threading.Tasks;
using AuthZI.Security.Authorization;
using Orleans;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
{
    [Authorize]
    public interface IUserGrain : IGrainWithStringKey
    {
        Task<string> GetDocumentContent(string docName);

        Task<string> ModifyDocumentContent(string docName, string newContent);
    }
}
