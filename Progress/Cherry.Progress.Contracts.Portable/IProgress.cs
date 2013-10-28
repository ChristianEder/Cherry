using System;

namespace Cherry.Progress.Contracts.Portable
{
    public interface IProgress : IDisposable
    {
        int? Max { get; set; }
        int? Current { get; set; }
        string Key { get; }
        string Title { get; set; }
        string Description { get; set; }
    }
}