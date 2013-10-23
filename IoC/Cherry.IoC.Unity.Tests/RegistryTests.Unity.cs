using Cherry.IoC.Unity;

namespace Cherry.IoC.Tests
{
    public partial class RegistryTests
    {
        partial void CreateRegistry()
        {
            _registry = new UnityServiceLocatorAndRegistry();
        }
    }
}
