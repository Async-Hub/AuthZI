using System.Threading.Tasks;
using AuthZI.MicrosoftOrleans.Authorization;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
{
  public class UserGrain(SecureGrainContext secureGrainContext) : 
    SecureGrain(secureGrainContext), IUserGrain
  {
    public async Task<string> GetDocumentContent(string docName)
    {
      var grain = GrainFactory.GetGrain<IDocumentsRegistryGrain>(DocumentsRegistry.Default);
      var doc = await grain.Take(docName);

      return doc.Content;
    }

    public async Task<string> ModifyDocumentContent(string docName, string newContent)
    {
      var grain = GrainFactory.GetGrain<IDocumentsRegistryGrain>(DocumentsRegistry.Default);

      return await grain.Modify(docName, newContent);
    }
  }
}