using Ask_Alfred.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private CancellationTokenSource m_CancellationTokenSource = null;

        public event Action<IPage> OnPageAdded;
        public event Action OnTimeoutExpired;
        private const int timeoutDurationInSeconds = 60;
        private eStatus m_Status; // TODO: remove this member?
        private System.Timers.Timer m_TimeoutTimer = new System.Timers.Timer();

        private enum eWebSite
        {
            Stackoverflow,
            Microsoft
        }
        private enum eStatus // TODO: remove this enum?
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

            m_CancellationTokenSource?.Dispose();
            m_CancellationTokenSource = new CancellationTokenSource();

            // should be checked on projects of various kinds and on projects like Alfred (1: c# 2: extension)
            if (i_Input.ProjectType != null)
            {
                await m_GoogleSearchEngine.AddSearchResultsFromQueryAsync(String.Format("site: {0} {1} {2}",
                    r_WebSitesUrls[eWebSite.Stackoverflow], i_Input.Description, i_Input.ProjectType));
            }
            await m_GoogleSearchEngine.AddSearchResultsFromQueryAsync(String.Format("site: {0} {1}",
                r_WebSitesUrls[eWebSite.Stackoverflow], i_Input.Description));

            await m_GoogleSearchEngine.AddSearchResultsFromQueryAsync(String.Format("site: {0} \"{1}\"",
                r_WebSitesUrls[eWebSite.Microsoft], i_Input.ErrorCode));

            await Task.Run(() => CreateWebDataListFromGoogleResultsAsync(m_CancellationTokenSource.Token), m_CancellationTokenSource.Token);

            return Response;
        }

        public void StopSearch()
        {
            // TODO: its possible that m_CancellationTokenSource is null here?
            m_CancellationTokenSource?.Cancel();
            m_Status = eStatus.Finished;
            m_TimeoutTimer.Stop();
        }

        public async Task CreateWebDataListFromGoogleResultsAsync(CancellationToken cancellationToken)
        {
            foreach (GoogleSearchResult googleResult in m_GoogleSearchEngine.SearchResults)
            {
                bool isPageInList = PagesList.Any(page => googleResult.Url.Contains(page.Url));
                cancellationToken.ThrowIfCancellationRequested();

                if (!isPageInList)
                {
                    IWebDataSource dataSource = WebDataSourceFactory.CreateWebDataSource(googleResult.Url);

                    if (dataSource != null)
                    {
                        await dataSource.ParseDataAsync();
                        cancellationToken.ThrowIfCancellationRequested();
                        PagesList.Add(dataSource.Page);
                        OnPageAdded(dataSource.Page);
                    }
                }
            }

            m_Status = eStatus.Finished;
            m_TimeoutTimer.Stop();
        }
        private void timeoutExpired(object source, ElapsedEventArgs e)
        {
            m_CancellationTokenSource.Cancel();
            m_Status = eStatus.TimeoutExpired;
            m_TimeoutTimer.Stop();
            OnTimeoutExpired();
        }
    }
}
