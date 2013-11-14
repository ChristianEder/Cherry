using Cherry.IoC.Contracts.Portable;
using Cherry.IoC.Unity;

namespace Cherry.IoC.Tests
{
    public partial class RegistryTests
    {
        private IServiceRegistry CreateRegistry()
        {
            return new UnityServiceRegistry();
        }
    }
}
