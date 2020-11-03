using Dotented.Interfaces;

namespace MySite.Pages
{
    public class Skills : DotentedContent
    {
        public static string Query = @"
            query {
                pages: skillsCollection {
                    items {
                        __typename
                        title
                        icon
                        details
                        url
                    }
                }
            }
        ";
        
        public string Title { get; set; }

        public string Icon { get; set; }

        public string Details { get; set; }
    }
}