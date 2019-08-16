using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ask_Alfred.Infrasructure.Interfaces;
using Ask_Alfred.Objects;

namespace Ask_Alfred.Infrasructure
{
    // this will be the class that runs all of the backend - should aggregate the googleSearchEngine, the results
    // Singleton ? then the search engine will not be singltone

    // TODO: this class should be internal?
    // TODO: this class should be singleton (mabye by static class?)
    public class AlfredEngine
    {
        public List<IWebDataSource> WebDataList { get; private set; }
        public AlfredResponse Response { get; }
        private GoogleSearchEngine m_GoogleSearchEngine;
        private readonly Dictionary<eWebSite, string> r_WebSitesUrls;
        public event Action<IPage> AddPageHandler;

        private enum eWebSite
        {
            Stackoverflow,
            // Microsoft
        }

        public AlfredEngine()
        {
            WebDataList = new List<IWebDataSource>();
            Response = new AlfredResponse(); // TODO: must be here?
            m_GoogleSearchEngine = new GoogleSearchEngine();
            r_WebSitesUrls = new Dictionary<eWebSite, string>();

            r_WebSitesUrls.Add(eWebSite.Stackoverflow, "stackoverflow.com");
            //k_WebSitesUrls.Add(eWebSite.Microsoft, "...");
        }

        public AlfredResponse Search(string i_SearchKey)
        {
            // TODO: not all the urls are valid (like tagged page in stackoverflow)

            // TODO: not always stackoverflow should be the only website to search in
            // TODO: maximum of X urls for each search?

            m_GoogleSearchEngine.AddSearchResultsFromQuery("site:" +
                r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_SearchKey);
            //m_GoogleSearchEngine.AddSearchResultsFromQuery("site:" +
            //    r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_SearchKey + " Visual studio");
            //m_GoogleSearchEngine.AddSearchResultsFromQuery("site:" +
            //    r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_SearchKey + " C#");

            //insertGoogleSearchResultToResponse();
            CreateWebDataListFromGoogleResultsAsync(); // TODO: await is needed here?
            insertWebPagesToResponse();

            return Response;
        }

        // currently this method prints the data to the window 
        private void insertWebPagesToResponse()
        {
            foreach (IWebDataSource webData in WebDataList)
            {
                AddPageHandler(webData.Page);
            }
        }

        // currently this method is where we are checking all of the functionallity
        public async Task CreateWebDataListFromGoogleResultsAsync()
        {
            foreach (GoogleSearchResult googleResult in m_GoogleSearchEngine.SearchResults)
            {
                IWebDataSource dataSource = WebDataSourceFactory.CreateWebDataSource(googleResult.Link);
                await dataSource.ParseDataAsync();
                AddPageHandler(dataSource.Page);
            }
        }
    }
}
