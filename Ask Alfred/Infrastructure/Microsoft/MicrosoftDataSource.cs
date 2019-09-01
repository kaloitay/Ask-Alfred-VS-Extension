using Ask_Alfred.Infrastructure.Interfaces;
using HtmlAgilityPack;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Ask_Alfred.Objects
{
    public class MicrosoftDataSource : IWebDataSource
    {
        private readonly string m_Url;

        public IPage Page { get; private set; }

        public MicrosoftDataSource(string i_Url)
        {
            m_Url = i_Url;
        }

        private async Task<string> getDocumentFromUrlAsync(string url)
        {
            string xmlStr;

            // catch errors
            using (var wc = new WebClient())
            {
                xmlStr = await wc.DownloadStringTaskAsync(m_Url);
            }

            return xmlStr;
        }

        // async?
        public async Task ParseDataAsync()
        {
            string xmlStr = await getDocumentFromUrlAsync(m_Url);
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(xmlStr);
            HtmlNode subjectNode = html.DocumentNode.SelectSingleNode("//*[@id='main']/h1");
            HtmlNode timeNode = html.DocumentNode.SelectSingleNode("//*[@id='main']/ul/li[1]/time");
            DateTime dateTime = DateTime.ParseExact(timeNode.InnerText, "MM/dd/yyyy",
                System.Globalization.CultureInfo.InvariantCulture);

            this.Page = new MicrosoftPage
            {
                WebsiteName = "Microsoft",
                Url = m_Url,
                Subject = subjectNode.InnerText,
                Date = dateTime
            };
        }
    }
}