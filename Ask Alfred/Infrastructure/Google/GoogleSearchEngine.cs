using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Ask_Alfred.Infrastructure
{
    // TODO: should be singleton - YES!
    public class GoogleSearchEngine
    {
        private const string k_ApiKey = "AIzaSyBpIpws1ZmWUw8hyvpsHTXFT6C6tOmHVqQ";
        private const string k_CustomSearchKey = "000838436833929131088:wnddlp0oh68"; // cx
        //private const string k_ApiKey = "AIzaSyAM1Ayj0GtbKiyyb70w0DaINSccWKb_2GY";
        //private const string k_CustomSearchKey = "012719749979738938401:szwvp3cwm1q"; // cx,
        //private const string k_ApiKey = "AIzaSyBA1A0OfQgJ4yuHNXN1SVRRd42WfwkJX0Q";
        //private const string k_CustomSearchKey = "012719749979738938401:1syzzn4sxhb"; // cx
        //private const string k_ApiKey = "AIzaSyDbyXpjHad4oQNixjE0jKZfriUq6E2pky8";
        //private const string k_CustomSearchKey = "011366759328101593804:7bf803lfg9m"; // cx
        //private const string k_ApiKey = "AIzaSyAhuyqu7vVv_d5pwIRydLu04zZXx1eG93I";
        //private const string k_CustomSearchKey = "015558687711821100062:zlpg17dqxoz"; // cx
        //private const string k_ApiKey = "AIzaSyAQvMdBvI3QgPcpwDKNH6BIOn4edEkhakQ";
        //private const string k_CustomSearchKey = "004793129189650674979:v0uurmusbqe"; // cx

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
