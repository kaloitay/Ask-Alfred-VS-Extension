using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ask_Alfred.Infrastructure.Interfaces;
using RavinduL.SEStandard;
using RavinduL.SEStandard.Models;
using Scopes = RavinduL.SEStandard.Models.Scopes;
using System.Web;
using System.Text.RegularExpressions;
using Ask_Alfred.Infrastructure;

namespace Ask_Alfred.Objects
{
    public class StackoverflowDataSource : IWebDataSource
    {
        private const string k_Key = "HmH2cbjLg5RcFFcPCq*gWg((";
        private const int k_ClientID = 15865;
        private readonly int m_ID;
        public IPage Page { get; private set; }

        // TODO: using for reflection supported languages from UI
        public static List<ESupportedProgrramingLanguages> SupportedProgrramingLanguages = new List<ESupportedProgrramingLanguages>(
            new ESupportedProgrramingLanguages[] {
            ESupportedProgrramingLanguages.CSharp,
            ESupportedProgrramingLanguages.CPlusPlus,
            ESupportedProgrramingLanguages.C});

        public StackoverflowDataSource(string i_Url)
        {
            string[] splitedUrl = i_Url.Split('/');
            if (splitedUrl.Length > 4)
                int.TryParse(splitedUrl[4], out m_ID);
        }

        public static bool IsValidUrl(string i_Url)
        {
            Regex regex = new Regex(@"stackoverflow.com\/questions\/\d+");
            return regex.Match(i_Url).Success;
        }

        public async Task ParseDataAsync()
        {
            var client = new StackExchangeClient(k_ClientID, k_Key, Scopes.NoExpiry);
            IList<int> ids = new List<int> { m_ID };

            // TODO: this call can never end!
            // https://stackoverflow.com/questions/10134310/how-to-cancel-a-task-in-await
            var query = await client.Questions.GetAsync(
                site: "stackoverflow",
                filter: "!)5d3yYdB0grnr6gO7E9oMFDnlZMk",
                ids: ids);

            // TODO: Throw exception if query.QuotaRemaining = query.QuotaMax?

            if (query.Items != null)
            {
                Question question = query.Items.First();
                this.Page = new StackoverflowPage
                {
                    Url = "https://stackoverflow.com/questions/" + m_ID,
                    Subject = HttpUtility.HtmlDecode(question.Title),
                    Date = Utils.UnixTimeStampToDateTime(question.CreationDate),
                    Score = question.Score,
                    FavoriteCount = question.FavoriteCount,
                    ViewCount = question.ViewCount,
                    IsAnswered = question.IsAnswered,
                    AnswerCount = question.AnswerCount
                };
            }
            // TODO: else?
        }
    }
}
