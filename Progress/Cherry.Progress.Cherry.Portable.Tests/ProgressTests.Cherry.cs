using Cherry.IoC.Contracts.Portable;
using Cherry.Progress.Cherry.Portable;
using Cherry.Progress.Contracts.Portable;

namespace Cherry.Progress.Tests
{
    public partial class ProgressTests
    {
        private IProgressService CreateProgressService(IServiceLocator locator)
        {
            return locator.Get<CherryProgressService>();
        }
    }
}
