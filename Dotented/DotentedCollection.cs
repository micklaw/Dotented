using System.Collections.Generic;
using Dotented.Interfaces;

namespace Dotented
{
    public class DotentedCollection<T> where T : DotentedContent
    {
        public List<T> Items { get; set; }
    }
}