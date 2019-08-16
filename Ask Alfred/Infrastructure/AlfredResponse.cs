using System.Collections.Generic;
using Ask_Alfred.Infrasructure.Interfaces;

namespace Ask_Alfred.Infrasructure
{
    public class AlfredResponse
    {
        public List<IPage> Results = new List<IPage>();
    }
}