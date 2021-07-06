using System;

namespace Authzi.MicrosoftOrleans.Grains.ResourceBasedAuthorization
{
    [Serializable]
    public class Document
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }
    }
}