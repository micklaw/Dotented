using System;

namespace Dotented.Interfaces
{
    public class DotentedOptions
    {
        public DotentedOptions(Type type)
        {
            Type = type;
            Query = type.GetField("Query").GetValue(null) as string;
        }
        
        public Type Type { get; }

        public string Query { get; }
        
        public bool SingleOnly { get; set; }

        public string View { get; set; }
    }
}