using System;
using System.Collections.Generic;
using System.Net.Http;
using Dotented.Interfaces;

namespace Dotented.Internal
{
    internal class DotentedBuilder : IDotentedBuilder
    {
        public IDictionary<string, Type> TypeCache { get; } = new Dictionary<string, Type>();
        public List<DotentedOptions> PagesTypes { get; }

        private readonly DotentedSettings settings;

        public DotentedBuilder(DotentedSettings settings)
        {
            this.PagesTypes = new List<DotentedOptions>();
            this.settings = settings;
        }

        public IDotentedBuilder WithComponent<T>() where T : DotentedContent, new()
        {
            TypeCache[typeof(T).Name.ToLower()] = typeof(T);
            return this;
        }

        public IDotentedBuilder WithPage<T>(Action<DotentedOptions> configure = null) where T : DotentedContent, new()
        {
            var options = new DotentedOptions(typeof(T));
            configure?.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.View))
            {
                options.View = typeof(T).Name;
            }

            TypeCache[typeof(T).Name.ToLower()] = typeof(T);
            PagesTypes.Add(options);
            return this;
        }

        public IDotentedGenerator Build()
        {
            return new DotentedGenerator(new DotentedContentfulClient(new HttpClient(), settings, this), this, settings);
        }
    }
}