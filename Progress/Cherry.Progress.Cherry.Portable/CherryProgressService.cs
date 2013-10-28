using Cherry.IoC.Contracts.Portable;
using Cherry.Progress.Contracts.Portable;

namespace Cherry.Progress.Cherry.Portable
{
    public class CherryProgressService : IProgressService
    {
        private readonly IServiceLocator _serviceLocator;

        public CherryProgressService(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public IProgress CreateProgress(string key)
        {
            var display = _serviceLocator.TryGet<IProgressDisplay>() ?? new DebugOutputProgressDisplay();
            return new CherryProgress(key, display);
        }

        public ICompositeProgress CreateCompositeProgress(string key)
        {
            var display = _serviceLocator.TryGet<IProgressDisplay>() ?? new DebugOutputProgressDisplay();
            return new CherryCompositeProgress(key, display);
        }
    }
}
