using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

// dynamic vs object ->
// object requires explicit cast (dynamic doesnt)
// object can throw an exception if the convert we use isnt appropriate (lets say we expect json and gets html object from some method)
// with dynamic the compiler has all the info we need 
// Object is useful when we don't have more information about the data type.
// Dynamic is useful when we need to code using reflection or dynamic languages or with the COM objects 
// and when getting result out of the LinQ queries.

namespace Ask_Alfred.Infrasructure
{
    // TODO: should be singleton ?
    public class GoogleSearchEngine
    {
        private const string k_ApiKey = "AIzaSyBpIpws1ZmWUw8hyvpsHTXFT6C6tOmHVqQ";
        private const string k_CustomSearchKey = "000838436833929131088:wnddlp0oh68"; // cx
        public List<GoogleSearchResult> SearchResults = new List<GoogleSearchResult>();

        // TODO: return value?
        // *** this is a method that gets a query as u want to search on google and searches in our engine
        // input -> query as u ask google
        // output -> currently none (think again when you are making architecture)
        public void AddSearchResultsFromQuery(string i_Query)
        {
            dynamic queryResult = getResults(i_Query);
            insertSearchResults(queryResult);
        }

        public void Clear()
        {
            SearchResults.Clear();
        }

        private string getUrlContentAsString(string i_Url)
        {
            WebClient client = new WebClient();
            string urlContent;

            try
            {
                urlContent = client.DownloadString(i_Url);
            }
            catch /*(WebException e)*/
            {
                throw new WebException("No internet connection");
            }

            return urlContent;
        }

        // *** this method gets a google query to search and searches with google api
        // input -> the query
        // output -> none - results are inserted to SearchResults property (also temporary)
        private dynamic getResults(string i_Query)
        {
            // TODO:
            // Add try catch here or in getUrlContentAsString
            // We get exeption when there is no internet connection

            string searchQuery = String.Format(
                "https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}&alt=json",
                k_ApiKey, k_CustomSearchKey, i_Query);

            string searchResults = getUrlContentAsString(searchQuery);
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
                    Link = item.link,
                    Host = item.displayLink,
                    Description = item.snippet
                });
            }
        }

        // *** this is a temporary method to start working on the responses from our sites
        public void TestActivateSearchResultLink()
        {

            //HtmlWeb web = new HtmlWeb();
            //HtmlDocument doc = web.Load(SearchResults[0].Link);

            //HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//div/div/div/pre/code");
            //HtmlNode single = nodeCollection[0];

            //string value = single.InnerText;

            //   return value;
        }
    }
}
