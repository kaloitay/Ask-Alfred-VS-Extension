using Ask_Alfred.Infrastructure.Interfaces;
using System;

namespace Ask_Alfred.Objects
{
    class MicrosoftPage : IPage
    {
        public string WebsiteName { get; set; }
        public string Url { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public double Rank
        {
            get
            {
                // TODO:
                return 100;
            }
        }
    }
}
