﻿using Ask_Alfred.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Ask_Alfred.Infrastructure
{
    public sealed class AlfredEngine
    {
        private static readonly Lazy<AlfredEngine> lazy = new Lazy<AlfredEngine>(() => new AlfredEngine());
        public static AlfredEngine Instance { get { return lazy.Value; } }
        public List<IPage> PagesList { get; private set; } = new List<IPage>();
        public AlfredResponse Response { get; } = new AlfredResponse();
        private GoogleSearchEngine m_GoogleSearchEngine = new GoogleSearchEngine();
        private readonly Dictionary<eWebSite, string> r_WebSitesUrls;

        public event Action<IPage> OnPageAdded;
        public event Action OnTimeoutExpired;
        private const int timeoutDurationInSeconds = 20; // shouldn't be const
        private eStatus m_Status;
        private Timer m_TimeoutTimer = new Timer();

        private enum eWebSite
        {
            Stackoverflow,
            Microsoft
        }
        private enum eStatus
        {
            Searching,
            Finished,
            TimeoutExpired
        }

        private AlfredEngine()
        {
            r_WebSitesUrls = new Dictionary<eWebSite, string>
            {
                // when removing /questions ParseDataAsync can never end! - must check it
                { eWebSite.Stackoverflow, "stackoverflow.com/questions/" },
                { eWebSite.Microsoft, "docs.microsoft.com" }
            };

            m_TimeoutTimer.Elapsed += new ElapsedEventHandler(timeoutExpired);
            m_TimeoutTimer.Interval = timeoutDurationInSeconds * 1000;
        }

        public async Task<AlfredResponse> SearchAsync(IAlfredInput i_Input)
        {
            // if i_Input.ErrorCode or Description is null... (in UI too)

            m_Status = eStatus.Searching;
            m_TimeoutTimer.Enabled = true;
            PagesList.Clear();

            m_GoogleSearchEngine.ClearResults();

            // should be checked on projects of various kinds and on projects like Alfred (1: c# 2: extension)
            string activeProjectType = Utils.GetProjectTypeAsString();
            if (activeProjectType != null)
            {
                await m_GoogleSearchEngine.AddSearchResultsFromQueryAsync("site:" +
                r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_Input.Description + " " + activeProjectType);
            }
            await m_GoogleSearchEngine.AddSearchResultsFromQueryAsync("site:" +
                r_WebSitesUrls[eWebSite.Stackoverflow] + " " + i_Input.Description);

            await m_GoogleSearchEngine.AddSearchResultsFromQueryAsync("site:" +
                r_WebSitesUrls[eWebSite.Microsoft] + " " + i_Input.ErrorCode);

            // TODO: this call can never end! https://stackoverflow.com/questions/10134310/how-to-cancel-a-task-in-await
            await CreateWebDataListFromGoogleResultsAsync(); // TODO: await is needed here?

            return Response;
        }

        public async Task CreateWebDataListFromGoogleResultsAsync()
        {
            foreach (GoogleSearchResult googleResult in m_GoogleSearchEngine.SearchResults)
            {
                bool isPageInList = PagesList.Any(page => googleResult.Url.Contains(page.Url));

                if (!isPageInList)
                {
                    IWebDataSource dataSource = WebDataSourceFactory.CreateWebDataSource(googleResult.Url);
                    if (dataSource != null)
                    {
                        await dataSource.ParseDataAsync();

                        // Should stop during the parse as well
                        if (m_Status == eStatus.Searching)
                        {
                            PagesList.Add(dataSource.Page);
                            OnPageAdded(dataSource.Page);
                        }
                    }
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
