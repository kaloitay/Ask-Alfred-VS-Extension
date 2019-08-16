using Ask_Alfred.Objects;
using Ask_Alfred.Infrasructure.Interfaces;
using System;

namespace Ask_Alfred.Infrasructure
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
                    webDataSource = CreateStackoverflowWebDataSource(i_Link);
                    break;
                case "docs.microsoft.com":
                    //webDataSource = CreateMicrosoftWebDataSource(i_Link);
                    break;
                //default:
            }

            return webDataSource;
        }

        public static StackoverflowDataSource CreateStackoverflowWebDataSource(string i_Link)
        {
            return new StackoverflowDataSource(i_Link);
        }
    }
}
