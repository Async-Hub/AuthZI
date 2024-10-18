using System.Threading.Tasks;
using AuthZI.Security.Authorization;
using Orleans;

namespace AuthZI.Tests.MicrosoftOrleans.Grains
{
    public interface IGlobalSecretStorageGrain : IGrainWithStringKey
    {
        [Authorize(Roles = "Admin")]
        Task<string> TakeUserSecret(string userId);
    }
}