using Ask_Alfred.Infrasructure.Interfaces;
using System;

namespace Ask_Alfred.Objects
{
    public class StackoverflowPage : IPage
    {
        public string Url { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public int FavoriteCount { get; set; }
        public int ViewCount { get; set; }
        public bool IsAnswered { get; set; }
        public int AnswerCount { get; set; }
    }
}
