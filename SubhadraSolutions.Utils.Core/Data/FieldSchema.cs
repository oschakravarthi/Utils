using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Data
{
    public class FieldSchema
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }   // string, integer, object, array, etc.


        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        // For nested structures

        [JsonProperty("fields")]
        public List<FieldSchema> Fields { get; set; }  // object

        [JsonProperty("items")]
        public FieldSchema Items { get; set; }         // array
    }
}
