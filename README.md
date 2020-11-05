# Dotented, why?
[![NuGet version (Dotented)](https://img.shields.io/nuget/v/Dotented.svg?style=flat-square)](https://www.nuget.org/packages/Dotented/)
![Publish](https://github.com/micklaw/Dotented/workflows/Publish/badge.svg)

I create static sites from time to time which I host in github pages. Its always a pain that users who manage them need a developer to make changes as they are non-technical. https://www.contentful.com/ is a CMS that can be delivered out the way via a GraphQL API, this way a developer can build content types in Contentful and also pages which deliver the json of a page outwards via the API. Dotented is a wrapper around the Contentful CMS for rendering HTML, where you can make POCOs for pages and components and also add Razor views which we can render the pages and components too.

## Your early
This is very much in the experimentl phase right now, so lots in flight. But the long and short of it is this:

- Build a dotnet core console app
- Create some POCOs to map to the graphQL queries
- Configure them in the app
- Add a config file for your Contentful settings
- Run the console app to generate your html

**Note: I usually have two terminals as as this usually goes hand in hand with some sort of NPM**

### Things in flight

- [x] Nuget package
- [x] Environments (working based on config replacements in YAML)
- [ ] Links in content managed automtically (right now they need manually managed)
- [ ] Rich text rendering (Markdown currently working)
- [ ] Docs

## Setup

Some steps on how to currently configure this east

### Installing
[![NuGet version (Dotented)](https://img.shields.io/nuget/v/Dotented.svg?style=flat-square)](https://www.nuget.org/packages/Dotented/)

### Example app
I have actually build my own site with this here, [micklaw/MLWD](https://github.com/micklaw/MLWD) so you can use that as reference to get it working. Works well for me and will give a good guide on how to configure your own site and rip off my YAML etc to configure it.

### Configuring

Create a console app, add some dependency injection to it and also a config file to be copied to the output directory. We also need to change the Project type to be a Razor project so we can render the views, like so.

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <AddRazorSupportForMVC>true</AddRazorSupportForMVC>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Dotented" Version="0.0.1" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="3.1.9" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="3.1.9" />
  </ItemGroup>

</Project>
```

Your local.settings.json or local.settings.dev.json (Not to be committed to source) config file will look something like this

```json
{
  "Dotented": {
    "BaseUri": "https://graphql.contentful.com/content/v1/spaces/{0}/environments/{1}",
    "SpaceId": "wo.....g0i",
    "EnvironmentId": "master",
    "ApiKey": "x9v..........................HPvJo",
    "OutputFolder": "../../../../../dist"
  }
}
```

### Initialisation

Next up ad some POCO(s) so you can map a graph QL query. The GraphQL query should be mapped to a static property called Query on your POCO. It should also inherit the DotentedContent class which has a Url property which is where your content will be written too e.g. blog/index.html or index.html for example in your filesystem. The typeName field also is required so we know what to bind too, this must mtch the name of your POCOs to which you are trying to bind e.g meCollection = ME.cs, skillsCollection = Skills.cs. There are also build in Contenful types e.g Asset = Assset.cs. Finally we need to call our root collection pages as this is used when desialising.

```csharp
namespace MySite.Pages
{
    public class Me : DotentedContent
    {
        public static string Query = @"
            query {
                pages: meCollection(limit: 1) {
                    items {
                        __typename
                        url
                        aboutTitle
                        aboutLead
                        aboutBody
                        tools
                        profileImage {
                            __typename
                            title
                            url
                        }
                        skills: skillsCollection {
                            items {
                                __typename
                                title
                                icon
                                details
                            }
                        }
                    }
                }
            }
        ";

        public string AboutTitle { get; set; }

        public string AboutLead { get; set; }

        public string AboutBody { get; set; }

        public string[] Tools { get; set; }

        public Components.Asset ProfileImage { get; set; }

        public DotentedCollection<Skills> Skills { get; set; }
    }
}
```

Once our models are setup we can configure it to run in out Program.cs. Here we add our IoC and bootsrap the configuration of our Models. Convention will find out where to looks for Views if you do not specify a view path for your type e.g "~/Views/{typeof(T).Name}.cshtml.

```csharp
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
                    .WithComponent<Testimonial>()
                    .WithComponent<Skills>()
                    .WithPage<Me>((options) => 
                    {
                        options.SingleOnly = true;
                    })
                    .Build();
            });

            await dotented.Generate();
        }
    }
}
```

### Views

Views work the ame as Razor views in normal dotnet core web apps. Sure you wont be able to use the full feature set, but the basics youd expect work as intended for rendering partials, layouts etc with extension methods mostly working too for rendering Raw content for example.

```html
@using MySite.Pages
@using Dotented
@model Me
@{
    Layout = "~/Views/Layout.cshtml";
}

<section class="intro-section video-section" id="hello">
    <div class="video-overlay">
        <div class="container"> 
            <div class="row">
                <div class="col-md-8 col-md-offset-2 col-sm-12">
                    <div class="intro_text"> 
                        <h1>@Model.AboutTitle</h1>
                        @Model.BioBody.ToHtml()
                        <div class="buttons scroll-to margin-top-50">
                            <a  href="#me" class="btn btn-lg btn-light-dark ">A bit about me<i class="fa fa-angle-down"></i></a>
                        </div>
                    </div> 
                </div>
            </div>
        </div>
    </div>
</section>
```

#### Markdown

If you have a richtext textbox which contains markdwn in Contentful using the **@Model.BioBody.ToHtml()** extension method will render this as HTML from the given markdown returned via the API.
