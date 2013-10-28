using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cherry.IoC.Cherry.Portable;
using Cherry.IoC.Contracts.Portable;
using Cherry.Progress.Contracts.Portable;
using Cherry.Progress.Tests.Display;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.Progress.Tests
{
    [TestClass]
    public partial class ProgressTests
    {
        private IProgressService _progressService;
        private CherryServiceLocatorAndRegistry _registry;

        [TestInitialize]
        public void BeforeEachTest()
        {
            if (_registry != null)
            {
                _registry.Dispose();
            }
            _registry = new CherryServiceLocatorAndRegistry();
            _progressService = null;
            _progressService = CreateProgressService(_registry);
            Assert.IsNotNull(_progressService, "Please implement the method CreateProgressService()");
        }

        [TestMethod]
        public async Task SimpleProgress()
        {
            var display = new TestableProgressDisplay();
            _registry.Register<IProgressDisplay>(display);
            using (var p = _progressService.CreateProgress("SimpleProgress"))
            {
                AssertStarted(display, "SimpleProgress");
                AssertCompleted(display);
                p.Title = "Hello";
                p.Description = "World";
                await DoSomething();
            }


            AssertStarted(display, "SimpleProgress");
            AssertCompleted(display, "SimpleProgress");
        }

  
        [TestMethod]
        public async Task CompositeProgress()
        {
            using (var p = _progressService.CreateCompositeProgress("CompositeProgress"))
            {
                p.Title = "Hello";
                p.Description = "World";

                var subTasks = new List<Task<int>>
                {
                    DoSomething(p, 1),
                    DoSomething(p, 2),
                    DoSomething(p, 3)
                };

                foreach (var subTask in subTasks)
                {
                    await subTask;
                }
            }

        }

        private static void AssertStarted(TestableProgressDisplay display, params string[] keys)
        {
            Assert.AreEqual(keys.Length, display.StartedKeys.Count);

            for (int i = 0; i < keys.Length; i++)
            {
                Assert.AreEqual(keys[i], display.StartedKeys[i]);
            }
        }

        private static void AssertCompleted(TestableProgressDisplay display, params string[] keys)
        {
            Assert.AreEqual(keys.Length, display.CompletedKeys.Count);

            for (int i = 0; i < keys.Length; i++)
            {
                Assert.AreEqual(keys[i], display.CompletedKeys[i]);
            }
        }


        private async Task DoSomething()
        {
            await Task.Delay(50);
        }

        private Task<int> DoSomething(ICompositeProgress progress, int result)
        {
            return Task.Factory.StartNew(() =>
            {
                using (progress.CreateSubProgress("DoSomething" + result))
                {
                    Thread.Sleep(50);
                    return result;
                }
            });
        }
    }
}