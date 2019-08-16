using System;

namespace Ask_Alfred.Infrasructure.Interfaces
{
    public interface IPage
    {
        string Url { get; }
        string Subject { get; }
        DateTime Date { get; }
    }
}
