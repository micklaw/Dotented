using System;
using Dotented;
using Dotented.Interfaces;
using MySite.Components;

namespace MySite.Pages
{
    public class Me : DotentedContent
    {
        public static string Query = @"
            query {
                pages: meCollection(limit: 1) {
                    items {
                        type: __typename
                        aboutTitle
                        bioTitle
                        codedLead
                        codedTitle
                        companyName
                        contactLead
                        contactTitle
                        email
                        githubUrl
                        linkedinUrl
                        name
                        skillsTitle
                        testimonialTitle
                        toolsLead
                        toolsTitle
                        twitterUrl
                        startYear
                        metaTitle
                        metaDescription
                        bio {
                            json
                        }
                        about {
                            json
                        }
                        tools
                        companies: codedCompaniesCollection {
                            items {
                                type: __typename
                                title
                                url
                            }
                        }
                        profileImage {
                            title
                            url
                        }
                        testimonials: testimonialsCollection {
                            items {
                                type: __typename
                                name
                                company
                                quote
                                image {
                                    title
                                    url
                                }
                            }
                        }
                        skills: skillsCollection {
                            items {
                                type: __typename
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

        public DotentedCollection<Skills> Skills { get; set; }
    }
}