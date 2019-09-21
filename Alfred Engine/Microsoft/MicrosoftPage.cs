using Ask_Alfred.Infrastructure.Interfaces;
using System;

namespace Ask_Alfred.Objects
{
    class MicrosoftPage : IPage
    {
        public int GoogleResultIndex { get; set; }
        public string WebsiteName { get; set; }
        public string Url { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }

        public double Rank
        {
            get
            {
                // Just for POC, no implementation
                return 1;
            }

        }
    }
}
