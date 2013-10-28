using System;
using System.Threading;
using System.Threading.Tasks;
using Cherry.Dispatching.Contracts.Portable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.Dispatching.Tests
{
    [TestClass]
    public partial class AppDispatcherTests
    {
        private IDispatcher _dispatcher;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _dispatcher = null;
            _dispatcher = CreateDispatcher();
            Assert.IsNotNull(_dispatcher, "Please implement the method CreateDispatcher()");
        }


        [TestMethod]
        public void TestSyncAction()
        {
            DateTime start = DateTime.Now;
            DateTime? calledAt = null;

            _dispatcher.Sync(() =>
            {
                Thread.Sleep(1000);
                calledAt = DateTime.Now;
            });

            var afterDispatch = DateTime.Now;

            Assert.IsTrue(calledAt.HasValue);
            Assert.IsTrue((afterDispatch - start).TotalMilliseconds > 800, "IDispatcher.Sync() seems to have worked asynchronously.");
        }

        [TestMethod]
        public void TestSyncFunc()
        {
            DateTime start = DateTime.Now;
            DateTime? calledAt = null;
            int result = _dispatcher.Sync(() =>
            {
                Thread.Sleep(1000);
                calledAt = DateTime.Now;
                return 17;
            });

            var afterDispatch = DateTime.Now;

            Assert.IsTrue(calledAt.HasValue);
            Assert.AreEqual(17, result);
            Assert.IsTrue((afterDispatch - start).TotalMilliseconds > 800, "IDispatcher.Sync() seems to have worked asynchronously.");
        }

        [TestMethod]
        public async Task TestAsyncAction()
        {
            DateTime start = DateTime.Now;
            DateTime? calledAt = null;
            var callTask = new TaskCompletionSource<object>();

            _dispatcher.Async(() =>
            {
                Thread.Sleep(1000);
                calledAt = DateTime.Now;
                callTask.SetResult(null);
            });

            Assert.IsFalse(calledAt.HasValue);
            var afterDispatch = DateTime.Now;

            // TODO: find a way to test async completion in a unit test scenario

            //await Task.Delay(20000);
            //callTask.Task.Wait(1500);

            //var afterCompletion = DateTime.Now;

            //Assert.IsTrue(calledAt.HasValue);
            Assert.IsTrue((afterDispatch - start).TotalMilliseconds < 200, "IDispatcher.Async() seems to have worked synchronously.");
            //Assert.IsTrue((afterCompletion - start).TotalMilliseconds > 800);
   
        }
    }
}
