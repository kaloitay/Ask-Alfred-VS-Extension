using Ask_Alfred.Infrastructure.Interfaces;
using Ask_Alfred.Objects;
using System;

namespace Ask_Alfred.Infrastructure
{
    public static class WebDataSourceFactory
    {
        // TODO: mabye the input should be eWebSite instead of string?
        public static IWebDataSource CreateWebDataSource(string i_Url)
        {
            IWebDataSource webDataSource = null;
            Uri myUri = new Uri(i_Url);

            switch (myUri.Host)
            {
                // TODO: two cases is needed here?
                case "www.stackoverflow.com":
                case "stackoverflow.com":
                    webDataSource = TryCreateStackoverflowWebDataSource(i_Url);
                    break;
                case "docs.microsoft.com":
                    webDataSource = TryCreateMicrosoftWebDataSource(i_Url);
                    break;
            }

            return webDataSource;
        }

        public static StackoverflowDataSource TryCreateStackoverflowWebDataSource(string i_Url)
        {
            StackoverflowDataSource dataSource = null;

            if (StackoverflowDataSource.IsValidUrl(i_Url) == true)
                dataSource = new StackoverflowDataSource(i_Url);

            return dataSource;
        }
        public static MicrosoftDataSource TryCreateMicrosoftWebDataSource(string i_Url)
        {
            return new MicrosoftDataSource(i_Url);
        }

    }
}
