using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ask_Alfred.Infrastructure
{
    public class GoogleSearchResult
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Host { get; set; }
        public string Description { get; set; }
    }
}