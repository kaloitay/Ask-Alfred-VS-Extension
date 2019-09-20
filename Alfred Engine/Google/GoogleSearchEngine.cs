using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Ask_Alfred.Infrastructure
{
    public sealed class GoogleSearchEngine
    {
        private static readonly Lazy<GoogleSearchEngine> lazy = new Lazy<GoogleSearchEngine> (() => new GoogleSearchEngine());
        public static GoogleSearchEngine Instance { get { return lazy.Value; } }

        private const string k_ApiKey = "AIzaSyBpIpws1ZmWUw8hyvpsHTXFT6C6tOmHVqQ";
        private const string k_CustomSearchKey = "000838436833929131088:wnddlp0oh68"; // cx

        public List<GoogleSearchResult> SearchResults = new List<GoogleSearchResult>();

        private GoogleSearchEngine() { }
        public async System.Threading.Tasks.Task AddSearchResultsFromQueryAsync(string i_Query)
        {
            dynamic queryResult = await getResultsAsync(i_Query);
            insertSearchResults(queryResult);
        }

        public void ClearResults()
        {
            SearchResults.Clear();
        }

        private async System.Threading.Tasks.Task<string> getUrlContentAsStringAsync(string i_Url)
        {
            string urlContent;

            using (WebClient client = new WebClient())
            {
                urlContent = await client.DownloadStringTaskAsync(i_Url);
            }

            return urlContent;
        }

        private async System.Threading.Tasks.Task<dynamic> getResultsAsync(string i_Query)
        {
            string searchQuery = String.Format(
                "https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}&alt=json",
                k_ApiKey, k_CustomSearchKey, i_Query);

            string searchResults = await getUrlContentAsStringAsync(searchQuery);
            dynamic jsonData = JsonConvert.DeserializeObject(searchResults);

            return jsonData;
        }

        private void insertSearchResults(dynamic i_JsonData)
        {
            foreach (var item in i_JsonData.items)
            {
                SearchResults.Add(new GoogleSearchResult
                {
                    Title = item.title,
                    Url = item.link,
                    Host = item.displayLink,
                    Description = item.snippet
                });
            }
        }
    }
}
