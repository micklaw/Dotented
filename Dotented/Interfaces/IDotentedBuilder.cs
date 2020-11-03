using System;
using System.Collections.Generic;

namespace Dotented.Interfaces
{
    public interface IDotentedBuilder
    {        
        IDictionary<string, Type> TypeCache { get; }
        
        List<DotentedOptions> PagesTypes { get; }

        IDotentedBuilder WithComponent<T>() where T : DotentedContent, new();

        IDotentedBuilder WithPage<T>(Action<DotentedOptions> configure = null) where T : DotentedContent, new();

        IDotentedGenerator Build();
    }
}