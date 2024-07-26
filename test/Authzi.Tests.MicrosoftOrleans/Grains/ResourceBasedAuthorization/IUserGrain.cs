using System.Threading.Tasks;
using Authzi.Security.Authorization;
using Orleans;

namespace Authzi.MicrosoftOrleans.Grains.ResourceBasedAuthorization
{
    [Authorize]
    public interface IUserGrain : IGrainWithStringKey
    {
        Task<string> GetDocumentContent(string docName);

        Task<string> ModifyDocumentContent(string docName, string newContent);
    }
}
