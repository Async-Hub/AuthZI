using Orleans;

namespace Authzi.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
{
    [GenerateSerializer]
    public class Document
    {
        [Id(0)]
        public string Name { get; set; }

        [Id(1)]
        public string Author { get; set; }

        [Id(2)]
        public string Content { get; set; }
    }
}