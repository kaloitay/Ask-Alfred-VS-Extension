﻿using Ask_Alfred.Infrastructure.Interfaces;
using System;

namespace Ask_Alfred.Objects
{
    public class StackoverflowPage : IPage
    {
        // TODO: change to significant names is needed here!

        // Stackoverflow was founded on July 2008
        private readonly DateTime m_LaunchedDate = new DateTime(2008, 07, 01);
        private const int m_DateMaximumWeight = 10;

        private const int m_HighestViews = 8100000;
        private const int m_ViewsMaximumWeight = 100;

        private const int m_HighestScore = 25000;
        private const int m_ScoreMaximumWeight = 1000;

        private const int m_IsAnsweredWeight = 10000;

        public string WebsiteName { get; set; }
        public string Url { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public int FavoriteCount { get; set; }
        public int ViewCount { get; set; }
        public bool IsAnswered { get; set; }
        public int AnswerCount { get; set; }

        public double Rank
        {
            get
            {
                return getIsAnsweredRank() + getScoreRank() + getViewsRank() + getDateRank();
            }
        }
        // there is similar logic between the following functions
        // so make a "util" function?

        private double getIsAnsweredRank()
        {
            return IsAnswered == true ? m_IsAnsweredWeight : 0;
        }

        private double getDateRank()
        {
            DateTime currentTime = DateTime.Now;

            TimeSpan elapsedTimeFromSendingTheQuestion = currentTime - Date;
            int elapsedDaysFromSendingTheQuestion = (int)elapsedTimeFromSendingTheQuestion.TotalDays;

            TimeSpan elapsedTimeFromLaunchedDate = currentTime - m_LaunchedDate;
            int elapsedDaysFromLaunchedDate = (int)elapsedTimeFromLaunchedDate.TotalDays;

            // TODO: must casting to double?
            // time rank is number between 0 to m_DateMaximumWeight
            double timeRank = (1 - ((double)elapsedDaysFromLaunchedDate / elapsedDaysFromLaunchedDate)) * m_DateMaximumWeight;

            return timeRank;
        }
        private double getViewsRank()
        {
            // views rank is number between 0 to m_ViewsMaximumWeight
            double viewsRank = ((double)ViewCount / m_HighestViews) * m_ViewsMaximumWeight;

            return (viewsRank <= m_ViewsMaximumWeight) ? viewsRank : m_ViewsMaximumWeight;
        }
        private double getScoreRank()
        {
            // score rank is number between 0 to m_ScoreMaximumWeight

            double scoreRank = ((double)Score / m_HighestScore) * m_ScoreMaximumWeight;

            return (scoreRank <= m_ScoreMaximumWeight) ? scoreRank : m_ScoreMaximumWeight;
        }

    }
}
