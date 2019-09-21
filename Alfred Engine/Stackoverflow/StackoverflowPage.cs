using Ask_Alfred.Infrastructure;
using Ask_Alfred.Infrastructure.Interfaces;
using System;
using System.Diagnostics;
using System.IO;

namespace Ask_Alfred.Objects
{
    public class StackoverflowPage : IPage
    {
        private const int k_GoogleIndexWeight = 20;

        private const int k_ScoreMaximumWeight = 100;
        private const int k_ScoreThreshold = 20000;

        private const int k_ViewsMaximumWeight = 50;
        private const int k_ViewsThreshold = 1000000;

        private const int k_IsAcceptedAnswer = 50;

        // Stackoverflow was founded on July 2008
        private readonly DateTime m_LaunchedDate = new DateTime(2008, 07, 01);
        private const int k_DateMaximumWeight = 1;

        public int GoogleResultIndex { get; set; }
        public string WebsiteName { get; set; }
        public string Url { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public int FavoriteCount { get; set; }
        public int ViewCount { get; set; }
        public bool IsAcceptedAnswer { get; set; }
        public int AnswerCount { get; set; }
       
        public double Rank
        {
            get
            {
                return getGoogleIndexRank() + getAcceptedAnswerRank() + getScoreRank() + getViewsRank() + getDateRank();
            }
        }

        private double getGoogleIndexRank()
        {
            return k_GoogleIndexWeight * (GoogleSearchEngine.k_EntriesPerPage - GoogleResultIndex);
        }

        private double getAcceptedAnswerRank()
        {
            return (IsAcceptedAnswer == true) ? k_IsAcceptedAnswer : 0;
        }
        private double getDateRank()
        {
            DateTime currentTime = DateTime.Now;

            TimeSpan elapsedTimeFromSendingTheQuestion = currentTime - Date;
            int elapsedDaysFromQuestionCreated = (int)elapsedTimeFromSendingTheQuestion.TotalDays;

            TimeSpan elapsedTimeFromLaunchedDate = currentTime - m_LaunchedDate;
            int elapsedDaysFromLaunchedDate = (int)elapsedTimeFromLaunchedDate.TotalDays;

            // return value: number relative to the time question created from now to the Stackoverflow launch date
            return (1 - ((double)elapsedDaysFromQuestionCreated / elapsedDaysFromLaunchedDate)) * k_DateMaximumWeight;
        }
        private double getViewsRank()
        {
            double viewsRank = ((double)ViewCount / k_ViewsThreshold) * k_ViewsMaximumWeight;

            return (viewsRank <= k_ViewsMaximumWeight) ? viewsRank : k_ViewsMaximumWeight;
        }
        private double getScoreRank()
        {
            double scoreRank = ((double)Score / k_ScoreThreshold) * k_ScoreMaximumWeight;

            return (scoreRank <= k_ScoreMaximumWeight) ? scoreRank : k_ScoreMaximumWeight;
        }
    }
}