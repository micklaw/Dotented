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
        }
    }
}