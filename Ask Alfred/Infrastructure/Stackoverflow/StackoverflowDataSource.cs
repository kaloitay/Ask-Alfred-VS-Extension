using Ask_Alfred.Infrastructure;
using Ask_Alfred.Infrastructure.Interfaces;
using RavinduL.SEStandard;
using RavinduL.SEStandard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Scopes = RavinduL.SEStandard.Models.Scopes;

namespace Ask_Alfred.Objects
{
    public class StackoverflowDataSource : IWebDataSource
    {
        private const string k_Key = "HmH2cbjLg5RcFFcPCq*gWg((";
        private const int k_ClientID = 15865;
        private readonly int m_ID;
        public IPage Page { get; private set; }

        public StackoverflowDataSource(string i_Url)
        {
            // TODO: move this logic to function
            string[] splitedUrl = i_Url.Split('/');
            if (splitedUrl.Length > 4)
                int.TryParse(splitedUrl[4], out m_ID);
        }
        public static bool IsValidUrl(string i_Url)
        {
            Regex regex = new Regex(@"stackoverflow.com\/questions\/\d+");
            return regex.Match(i_Url).Success;
        }
        public async Task<IPage> ParseDataAndGetPageAsync()
        {
            IPage page = null;
            var client = new StackExchangeClient(k_ClientID, k_Key, Scopes.NoExpiry);
            IList<int> ids = new List<int> { m_ID };

            // TODO: this call can never end!
            // https://stackoverflow.com/questions/10134310/how-to-cancel-a-task-in-await
            var query = await client.Questions.GetAsync(
                site: "stackoverflow",
                filter: "!)5d3yYdB0grnr6gO7E9oMFDnlZMk",
                ids: ids);

            if (query.Items != null)
            {
                Question question = query.Items.First();

                page = new StackoverflowPage
                {
                    WebsiteName = "Stackoverflow",
                    Url = "https://stackoverflow.com/questions/" + m_ID,
                    Subject = HttpUtility.HtmlDecode(question.Title),
                    Date = Utils.UnixTimeStampToDateTime(question.CreationDate),
                    Score = question.Score,
                    FavoriteCount = question.FavoriteCount,
                    ViewCount = question.ViewCount,
                    IsAcceptedAnswer = question.IsAnswered,
                    AnswerCount = question.AnswerCount
                };
            }

            return page;
        }
    }
}
