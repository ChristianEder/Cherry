using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var display = new TestableProgressDisplay();
            _registry.Register<IProgressDisplay>(display);

            using (var p = _progressService.CreateCompositeProgress("CompositeProgress"))
            {
                AssertStarted(display, "CompositeProgress");
                AssertCompleted(display);
                AssertProgressIs(display, "CompositeProgress", 0, false);

                p.Title = "Hello";
                p.Description = "World";


                var subTasks = new List<Task<int>>();
                subTasks.Add(DoSomething(p, 1));
                subTasks.Add(DoSomething(p, 2));
                subTasks.Add(DoSomething(p, 3));



                foreach (var subTask in subTasks)
                {
                    var result = await subTask;
                    AssertProgressIs(display, "CompositeProgress", result, true);
                }
            }

            AssertStarted(display, "CompositeProgress");
            AssertProgressIs(display, "CompositeProgress", 3, false);
            AssertCompleted(display, "CompositeProgress");
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

        private static void AssertProgressIs(TestableProgressDisplay display, string key, int expected, bool orHigher)
        {
            var change =
                display.Changes.ToArray().Where(c => c.Key == key).OrderBy(c => c.Current.GetValueOrDefault()).LastOrDefault();

            if (ReferenceEquals(change, null))
            {
                if (expected == 0)
                {
                    return;
                }
                Assert.Fail();
            }

            var value = change.Current.GetValueOrDefault();

            if (orHigher)
            {
                Assert.IsTrue(value >= expected);
            }
            else
            {
                Assert.AreEqual(expected, value);
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