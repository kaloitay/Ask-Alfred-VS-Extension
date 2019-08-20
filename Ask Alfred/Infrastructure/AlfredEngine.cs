using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ask_Alfred.Infrasructure.Interfaces;
using Ask_Alfred.Objects;
using System.Timers;

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
        private int m_ResultsAmount;
        private const int k_ResultsThreshold = 5;

        public event Action<IPage> OnPageAdded;
        public event Action<int> OnSearchIsFinished;
        public event Action<int> OnTimeoutExpired;
        private const int timeoutDurationInSeconds = 15; // shouldn't be const
        private eStatus m_Status;
        private Timer m_TimeoutTimer = new System.Timers.Timer();

        // TODO:
        // should be thread safe beacuse other thread is invoking timeoutExpired
        // so m_Status, m_ResultsAmount should be thread-safe?

        private enum eWebSite
        {
            Stackoverflow,
            // Microsoft
        }
        private enum eStatus
        {
            Searching,
            Finished,
            TimeoutExpired
        }

        public AlfredEngine()
        {
            WebDataList = new List<IWebDataSource>();
            Response = new AlfredResponse(); // TODO: must be here?
            m_GoogleSearchEngine = new GoogleSearchEngine();
            r_WebSitesUrls = new Dictionary<eWebSite, string>
            {
                { eWebSite.Stackoverflow, "stackoverflow.com" }
              //{ eWebSite.Microsoft, "..." }
            };

            m_TimeoutTimer.Elapsed += new ElapsedEventHandler(timeoutExpired);
            m_TimeoutTimer.Interval = timeoutDurationInSeconds * 1000;
        }

        public void Clear()
        {
            WebDataList.Clear();
        }
        public async Task<AlfredResponse> SearchAsync(string i_SearchKey)
        {
            m_ResultsAmount = 0;
            m_Status = eStatus.Searching;
            m_TimeoutTimer.Enabled = true;

            // TODO: not all the urls are valid (like tagged page in stackoverflow)

            // TODO: not always stackoverflow should be the only website to search in
            // TODO: maximum of X urls for each search?

            m_GoogleSearchEngine.Clear();
            m_GoogleSearchEngine.AddSearchResultsFromQuery("site:" +
                r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_SearchKey);
            //m_GoogleSearchEngine.AddSearchResultsFromQuery("site:" +
            //    r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_SearchKey + " Visual studio");
            //m_GoogleSearchEngine.AddSearchResultsFromQuery("site:" +
            //    r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_SearchKey + " C#");

            //insertGoogleSearchResultToResponse();
            await CreateWebDataListFromGoogleResultsAsync(); // TODO: await is needed here?

            return Response;
        }

        // currently this method is where we are checking all of the functionallity
        public async Task CreateWebDataListFromGoogleResultsAsync()
        {
            // TODO: link can refer to a specific answer (like https://stackoverflow.com/q/16333625)
            // TODO: or to the tagged page

            foreach (GoogleSearchResult googleResult in m_GoogleSearchEngine.SearchResults)
            {
                IWebDataSource dataSource = WebDataSourceFactory.CreateWebDataSource(googleResult.Link);
                await dataSource.ParseDataAsync(); // TODO: got stuck in links like https://stackoverflow.com/q/16333625

                // Should stop during the parse as well
                if (m_Status == eStatus.Searching)
                {
                    m_ResultsAmount++;
                    OnPageAdded(dataSource.Page);
                }
                else
                    break;
            }

            searchIsFinished();
            //if (m_Status == eStatus.Searching || m_ResultsAmount > k_ResultsThreshold) 
            //    searchIsFinished();
            //else
            //    OnTimeoutExpired(m_ResultsAmount);
        }

        private void searchIsFinished()
        {
            m_Status = eStatus.Finished;
            m_TimeoutTimer.Stop();
            OnSearchIsFinished(m_ResultsAmount);
        }
        private void timeoutExpired(object source, ElapsedEventArgs e)
        {
            m_Status = eStatus.TimeoutExpired;
            m_TimeoutTimer.Stop();
            OnTimeoutExpired(m_ResultsAmount); // TODO: shoule be removed beacuse this callback should be invoke after the foreach
        }
    }
}
