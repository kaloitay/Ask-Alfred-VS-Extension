using Ask_Alfred.Objects;
using Ask_Alfred.Infrastructure.Interfaces;
using System;

namespace Ask_Alfred.Infrastructure
{
    public static class WebDataSourceFactory
    {
        // TODO: mabye the input should be eWebSite instead of string?
        public static IWebDataSource CreateWebDataSource(string i_Link)
        {
            IWebDataSource webDataSource = null;
            Uri myUri = new Uri(i_Link);

            switch (myUri.Host)
            {
                // TODO: two cases is needed here?
                case "www.stackoverflow.com":
                case "stackoverflow.com":
                    webDataSource = TryCreateStackoverflowWebDataSource(i_Link);
                    break;
                case "docs.microsoft.com":
                    //webDataSource = TryCreateMicrosoftWebDataSource(i_Link);
                    break;
            }

            return webDataSource;
        }

        public static StackoverflowDataSource TryCreateStackoverflowWebDataSource(string i_Link)
        {
            StackoverflowDataSource dataSource = null;

            if (StackoverflowDataSource.IsValidUrl(i_Link) == true)
                dataSource = new StackoverflowDataSource(i_Link);

            return dataSource;
        }
    }
}
