using System.Threading.Tasks;
using AuthZI.Security.Authorization;
using Orleans;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
{
    [Authorize(Policy = "DocRegistryAccess")]
    public interface IDocumentsRegistryGrain : IGrainWithStringKey
    {
        Task Add(Document doc);
        
        Task<string> Modify(string docName, string newContent);

        Task<Document> Take(string docName);
    }
}