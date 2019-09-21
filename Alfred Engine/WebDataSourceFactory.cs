using Ask_Alfred.Infrastructure.Interfaces;
using Ask_Alfred.Objects;
using System;

namespace Ask_Alfred.Infrastructure
{
    public static class WebDataSourceFactory
    {
        public static IWebDataSource CreateWebDataSource(GoogleSearchResult i_Result)
        {
            IWebDataSource webDataSource = null;
            Uri myUri = new Uri(i_Result.Url);

            switch (myUri.Host)
            {
                case "www.stackoverflow.com":
                case "stackoverflow.com":
                    webDataSource = TryCreateStackoverflowWebDataSource(i_Result);
                    break;
                case "docs.microsoft.com":
                    webDataSource = TryCreateMicrosoftWebDataSource(i_Result);
                    break;
            }

            return webDataSource;
        }

        public static StackoverflowDataSource TryCreateStackoverflowWebDataSource(GoogleSearchResult i_Result)
        {
            StackoverflowDataSource dataSource = null;

            if (StackoverflowDataSource.IsValidUrl(i_Result.Url) == true)
                dataSource = new StackoverflowDataSource(i_Result);

            return dataSource;
        }
        public static MicrosoftDataSource TryCreateMicrosoftWebDataSource(GoogleSearchResult i_Result)
        {
            MicrosoftDataSource dataSource = null;

            if (MicrosoftDataSource.IsValidUrl(i_Result.Url) == true)
                dataSource = new MicrosoftDataSource(i_Result);

            return dataSource;
        }
    }
}
