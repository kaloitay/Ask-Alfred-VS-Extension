using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ask_Alfred.Infrastructure.Interfaces;
using System.Timers;


namespace Ask_Alfred.Infrastructure
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

        public event Action<IPage> OnPageAdded;
        public event Action OnTimeoutExpired;
        private const int timeoutDurationInSeconds = 20; // shouldn't be const
        private eStatus m_Status;
        private Timer m_TimeoutTimer = new System.Timers.Timer();

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
                // when removing /questions ParseDataAsync can never end! - must check it
                { eWebSite.Stackoverflow, "stackoverflow.com/questions/" }
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
            m_Status = eStatus.Searching;
            m_TimeoutTimer.Enabled = true;

            // TODO: not always stackoverflow should be the only website to search in
            // TODO: maximum of X urls for each search?
            // TODO: repetition of links is unwanted - FIX that

            m_GoogleSearchEngine.Clear();
            m_GoogleSearchEngine.AddSearchResultsFromQuery("site:" +
                r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_SearchKey);

            // should be checked on projects of various kinds
            // should be checked on project that contain several types (like Alfred [1: c# 2: extension])
            string activeProjectType = Utils.GetProjectTypeAsString();
            if (activeProjectType != null)
            {
                m_GoogleSearchEngine.AddSearchResultsFromQuery("site:" +
                r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_SearchKey + " " + activeProjectType);
            }

            //m_GoogleSearchEngine.AddSearchResultsFromQuery("site:" +
            //    r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_SearchKey + " Visual studio");

            // TODO: this call can never end!
            // https://stackoverflow.com/questions/10134310/how-to-cancel-a-task-in-await
            await CreateWebDataListFromGoogleResultsAsync(); // TODO: await is needed here?

            return Response;
        }

        // currently this method is where we are checking all of the functionallity
        public async System.Threading.Tasks.Task CreateWebDataListFromGoogleResultsAsync()
        {
            foreach (GoogleSearchResult googleResult in m_GoogleSearchEngine.SearchResults)
            {
                IWebDataSource dataSource = WebDataSourceFactory.CreateWebDataSource(googleResult.Link);
                if (dataSource != null)
                {
                    await dataSource.ParseDataAsync();
                    // Should stop during the parse as well
                    if (m_Status == eStatus.Searching)
                        OnPageAdded(dataSource.Page);
                }
                if (m_Status != eStatus.Searching)
                    break;
            }

            if (m_Status == eStatus.Searching)
            {
                m_Status = eStatus.Finished;
                m_TimeoutTimer.Stop();
            }
        }
        private void timeoutExpired(object source, ElapsedEventArgs e)
        {
            m_Status = eStatus.TimeoutExpired;
            m_TimeoutTimer.Stop();
            OnTimeoutExpired();
        }
    }
}
