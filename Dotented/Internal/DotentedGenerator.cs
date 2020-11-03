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
        private readonly DotentedContentfulClient client;
        private readonly DotentedBuilder builder;
        private readonly DotentedSettings settings;

        public DotentedGenerator(DotentedContentfulClient client, DotentedBuilder builder, DotentedSettings settings)
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
            if (content == null)
            {
                return;
            }

            var html = await RazorTemplateEngine.RenderAsync($"~/Views/{options.View}.cshtml", content);

            if (string.IsNullOrWhiteSpace(html))
            {
                return;
            }

            var filePath = GetPath(content);
            await File.WriteAllTextAsync(filePath, html, Encoding.UTF8);
        }

        private string GetPath(DotentedContent content)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = Path.Combine(path, settings.OutputFolder);

            if (string.IsNullOrEmpty(content.Url))
            {
                throw new Exception("To make a page, a Url property must be define don the content which has at least a filename e.g path/index.html or simply index.html");
            }


            path = Path.Combine(path, content.Url);
            var directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return path.ToLower();
        }
    }
}