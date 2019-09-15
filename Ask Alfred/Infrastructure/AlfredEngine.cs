using Ask_Alfred.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private MemoryCacheIPage m_PagesMemoryCache = new MemoryCacheIPage();

        public event Action<IPage> OnPageAdded;
        private const int timeoutDurationInSeconds = 60;
        private System.Timers.Timer m_TimeoutTimer = new System.Timers.Timer();

        private enum eWebSite
        {
            Stackoverflow,
            Microsoft
        }

        private AlfredEngine()
        {
            r_WebSitesUrls = new Dictionary<eWebSite, string>
            {
                { eWebSite.Stackoverflow, "stackoverflow.com/questions/" },
                { eWebSite.Microsoft, "docs.microsoft.com" }
            };

            m_TimeoutTimer.Elapsed += new ElapsedEventHandler(timeoutExpired);
            m_TimeoutTimer.Interval = timeoutDurationInSeconds * 1000;
        }

        public async Task<AlfredResponse> SearchAsync(IAlfredInput i_Input)
        {
            // TODO: handle ErrorCode or Description is null

            m_TimeoutTimer.Enabled = true;
            PagesList.Clear();
            m_GoogleSearchEngine.ClearResults();

            m_CancellationTokenSource?.Dispose();
            m_CancellationTokenSource = new CancellationTokenSource();

            try
            {
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
            }
            catch (WebException ex)
            {
                StopSearch();
                throw new WebException("No internet connection");
            }
            catch (OperationCanceledException ex)
            {
                StopSearch();
                throw new OperationCanceledException("Operation canceled - timeout expired");
            }
            catch (Exception ex)
            {
                StopSearch();
                throw new WebException("Uunexpected error");
            }

            return Response;
        }

        public void StopSearch()
        {
            m_CancellationTokenSource?.Cancel();
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
                        IPage page = await m_PagesMemoryCache.GetOrCreate(googleResult.Url,
                            async () => await dataSource.ParseDataAndGetPageAsync());
                        cancellationToken.ThrowIfCancellationRequested();
                        PagesList.Add(page);
                        OnPageAdded(page);
                    }
                }
            }

            m_TimeoutTimer.Stop();
        }
        private void timeoutExpired(object source, ElapsedEventArgs e)
        {
            StopSearch();
        }
    }
}
