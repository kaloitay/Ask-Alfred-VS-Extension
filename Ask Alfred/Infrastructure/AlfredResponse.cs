using System.Collections.Generic;
using Ask_Alfred.Infrastructure.Interfaces;

namespace Ask_Alfred.Infrastructure
{
    public class AlfredResponse
    {
        public List<IPage> Results = new List<IPage>();
    }
}