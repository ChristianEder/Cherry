using Cherry.Dispatching.Client.Net45;
using Cherry.Dispatching.Contracts.Portable;

namespace Cherry.Dispatching.Tests
{
    public partial class AppDispatcherTests
    {
        private IDispatcher CreateDispatcher()
        {
            return new AppDispatcher();
        }
    }
}
