using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dotented.Interfaces;
using Razor.Templating.Core;

namespace Dotented.Internal
{
    internal class DotentedGenerator : IDotentedGenerator
    {
        private readonly DotentedContentFactory client;
        private readonly DotentedBuilder builder;
        private readonly DotentedSettings settings;
        private readonly string filename = "index.html";

        public DotentedGenerator(DotentedContentFactory client, DotentedBuilder builder, DotentedSettings settings)
        {
            this.client = client;
            this.builder = builder;
            this.settings = settings;
        }

        public async Task Generate()
        {
            foreach (var pageType in builder.PagesTypes)
            {
                var pagestoRender = await client.Query(pageType);

                foreach (var page in pagestoRender)
                {
                    await RenderToPage(pageType, page);

                    if (pageType.SingleOnly)
                    {
                        break;
                    }
                }
            }
        }

        private async Task RenderToPage(DotentedOptions options, DotentedContent content) 
        {
           var renderer = RazorViewToStringRendererFactory.CreateRenderer();

           var html = await renderer.RenderViewToStringAsync($"~/Views/{options.View}.cshtml", content);

           if (string.IsNullOrWhiteSpace(html))
           {
               return;
           }

           if (options.SingleOnly)
           {
               var path = GetPath();
               await File.WriteAllTextAsync(Path.Combine(path, filename), html, Encoding.UTF8);
           }
        }

        private string GetPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.Combine(path, settings.OutputFolder);
        }
    }
}