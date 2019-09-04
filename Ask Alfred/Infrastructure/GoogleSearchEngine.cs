using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Ask_Alfred.Infrastructure
{
    // TODO: should be singleton ?
    public class GoogleSearchEngine
    {
        private const string k_ApiKey = "AIzaSyBpIpws1ZmWUw8hyvpsHTXFT6C6tOmHVqQ";
        private const string k_CustomSearchKey = "000838436833929131088:wnddlp0oh68"; // cx
        public List<GoogleSearchResult> SearchResults = new List<GoogleSearchResult>();

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
            WebClient client = new WebClient();
            string urlContent;

            // using () pattern
            try
            {
                urlContent = await client.DownloadStringTaskAsync(i_Url);
            }
            catch (WebException e)
            {
                // TODO: (" + e.ToString() + ")" just for debugging
                // TODO: split to two different WebException (over the limit/no interrnet connection)
                throw new WebException("No internet connection (" + e.ToString() + ")");
            }

            return urlContent;
        }

        private async System.Threading.Tasks.Task<dynamic> getResultsAsync(string i_Query)
        {
            // TODO:
            // Add try catch here or in getUrlContentAsString
            // We get exeption when there is no internet connection

            // TODO: &num=5 for limit the results (5 should be const)
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
