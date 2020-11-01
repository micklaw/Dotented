using System;
using System.Collections.Generic;
using Dotented.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dotented.Internal
{
    public class DotentedJsonConverter : JsonConverter
    {
        private readonly IDictionary<string, Type> typeCache;

        public DotentedJsonConverter(IDictionary<string, Type> typeCache)
        {
            this.typeCache = typeCache;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DotentedContent).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,  Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            // Using a nullable bool here in case "is_album" is not present on an item
            var itemType = jo["type"]?.Value<string>();

            DotentedContent item = null;

            if (itemType != null && typeCache.ContainsKey(itemType.ToLower()))
            {
                var type = typeCache[itemType.ToLower()];
                item = Activator.CreateInstance(type) as DotentedContent;
                serializer.Populate(jo.CreateReader(), item);
            }

            return item;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer,  object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}