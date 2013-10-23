using Cherry.IoC.Ninject;

namespace Cherry.IoC.Tests
{
    public partial class LocatorTests
    {
        partial void CreateRegistry()
        {
            _registry = new NinjectServiceLocatorAndRegistry();
        }
    }
}
