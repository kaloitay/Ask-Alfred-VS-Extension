using Ask_Alfred.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace Ask_Alfred.Infrastructure
{
    public class AlfredResponse
    {
        public List<IPage> Results = new List<IPage>();
    }
}