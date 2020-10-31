using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dotented.Interfaces;
using Newtonsoft.Json;

namespace Dotented.Internal
{
    internal class DotentedContentFactory
    {
        private readonly DotentedSettings Settings;
        private readonly IDotentedBuilder Builder;
        private readonly HttpClient Client;

        public DotentedContentFactory(HttpClient client, DotentedSettings settings, IDotentedBuilder builder)
        {
            Settings = settings;
            Builder = builder;
            Client = client;
        }

        public async Task<IList<DotentedContent>> Query(DotentedOptions options)
        {
            var jsonObject = new { query = options.Query };
            var json = JsonConvert.SerializeObject(jsonObject);

            var request = new HttpRequestMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post,
                RequestUri = new System.Uri(string.Format(Settings.BaseUri, Settings.SpaceId, Settings.EnvironmentId))
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Settings.ApiKey);

            var response = await Client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            var mapped = JsonConvert.DeserializeObject<DotentedData>(body, new DotentedJsonConverter(Builder.TypeCache));

            return mapped?.Data?.Pages?.Items;
        }
    }
}