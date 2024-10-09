using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Claims;

namespace Authzi.MicrosoftOrleans
{
    public class ClaimJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Claim));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, 
            object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            
            var issuer = jObject["Issuer"]?.ToString();
            var originalIssuer = jObject["OriginalIssuer"]?.ToString();
            var type = jObject["Type"]?.ToString();
            var value = jObject["Value"]?.ToString();
            var valueType = jObject["ValueType"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new InvalidOperationException("Clam Type is missing.");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("Claim Value is missing.");
            }

            return new Claim(type, value, valueType, issuer, originalIssuer);
        }

        public override bool CanWrite => false;
    }
}
