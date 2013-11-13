using System;
using Cherry.Progress.Contracts.Portable;

namespace Cherry.Progress.Cherry.Portable
{
    public class CherryProgressService : IProgressService
    {
        private readonly Func<IProgressDisplay> _progressDisplayFactory;

        public CherryProgressService(Func<IProgressDisplay> progressDisplayFactory)
        {
            _progressDisplayFactory = progressDisplayFactory;
        }

        public IProgress CreateProgress(string key)
        {
            var display = _progressDisplayFactory();
            return new CherryProgress(key, display);
        }

        public ICompositeProgress CreateCompositeProgress(string key)
        {
            var display = _progressDisplayFactory();
            return new CherryCompositeProgress(key, display);
        }
    }
}
