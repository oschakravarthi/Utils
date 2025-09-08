using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Core.Data
{
    public class FieldSchema
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }   // string, integer, object, array, etc.
        public bool Required { get; set; }
        public string Placeholder { get; set; }
        public string Description { get; set; }
        public string Pattern { get; set; }

        // For nested structures
        public List<FieldSchema> Fields { get; set; }  // object
        public FieldSchema Items { get; set; }         // array
    }
}
