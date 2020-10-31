using System.Threading.Tasks;
using Dotented;
using Microsoft.Extensions.DependencyInjection;
using MySite.Components;
using MySite.Pages;

namespace MySite
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            var dotented = services.AddDotented((builder) => 
            {
                return builder
                    .WithComponent<Asset>()
                    .WithComponent<Skills>()
                    .WithComponent<Testimonial>()
                    .WithPage<Me>((options) => 
                    {
                        options.SingleOnly = true;
                        options.Url = "/";
                    })
                    .Build();
            });

            await dotented.Generate();
        }
    }
}
