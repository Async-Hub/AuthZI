using AuthZI.MicrosoftOrleans;
using AuthZI.Security;
using AuthZI.Security.Authorization;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DocumentsRegistryGrain(
        [PersistentState("state", "MemoryGrainStorage")]
        IPersistentState<List<Document>> state,
        IAuthorizationService authorizationService,
        IClaimTypeResolver claimTypeResolver)
        : GrainWithClaimsPrincipal(claimTypeResolver), IDocumentsRegistryGrain
    {
        public async Task Add(Document doc)
        {
            state.State.Add(doc);

            await state.WriteStateAsync();
        }

        public async Task<string> Modify(string docName, string newContent)
        {
            var document = state.State.Single(doc => doc.Name == docName);

            var authorizationResult = await authorizationService
                .AuthorizeAsync(User, document, AuthorizationConfig.DocumentModifyAccessPolicy);

            // ReSharper disable once InvertIf
            if (authorizationResult.Succeeded)
            {
                document.Content = newContent;
                await state.WriteStateAsync();
                return document.Content;
            }

            return null;
        }

        public Task<Document> Take(string docName)
        {
            var document = state.State.Single(doc => doc.Name == docName);

            return Task.FromResult(document);
        }
    }
}