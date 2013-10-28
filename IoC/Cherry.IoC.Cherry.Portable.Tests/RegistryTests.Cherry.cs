using Cherry.IoC.Cherry.Portable;
using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Tests
{
    public partial class RegistryTests
    {
        private IServiceRegistry CreateRegistry()
        {
            return new CherryServiceLocatorAndRegistry();
        }
    }
}
