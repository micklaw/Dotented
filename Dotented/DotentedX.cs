using System;
using System.IO;
using Dotented.Interfaces;
using Dotented.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core;

namespace Dotented
{
    public static class DotentedStartupExtensions 
    {
        public static IDotentedGenerator AddDotented(this IServiceCollection services, Func<IDotentedBuilder, IDotentedGenerator> configure)
        {
            if (configure == null)
            {
                throw new Exception("Read the docs mofo to configure!");
            }

            RazorTemplateEngine.Initialize();
            
            var settings = new DotentedSettings();
            
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            configuration.Bind("Dotented", settings);

            var builder = new DotentedBuilder(settings);

            return configure.Invoke(builder);
        }
    }
}