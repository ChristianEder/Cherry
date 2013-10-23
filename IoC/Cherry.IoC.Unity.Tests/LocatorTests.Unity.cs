using Cherry.IoC.Unity;

namespace Cherry.IoC.Tests
{
    public partial class LocatorTests
    {
        partial void CreateRegistry()
        {
            _registry = new UnityServiceLocatorAndRegistry();
        }
    }
}
