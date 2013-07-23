using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.Tasks.Tests
{
    [TestClass]
    public class SynchronousTaskSchedulerTests : TaskSchedulerTestsBase
    {
        [TestInitialize]
        public void InitTest()
        {
            Factory = new TaskFactory(new SychronousScheduler());
        }

        [TestMethod]
        public void TestCreateSingleTask()
        {
            DoTestCreateSingleTask();
        }

        [TestMethod]
        public void TestCreateSingleResultTask()
        {
            DoTestCreateSingleResultTask();
        }

        [TestMethod]
        public async Task TestRunAdditionChain()
        {
            await DoTestRunAdditionChain();
        }

        [TestMethod]
        public async Task TestRunParallelAndSubsequent()
        {
            await DoTestRunParallelAndSubsequent();
        }

        [TestMethod]
        public async Task TestSimpleRun()
        {
            await DoTestSimpleRun();
        }

        [TestMethod]
        public async Task TestSimpleResult()
        {
            await DoTestSimpleResult();
        }
    }
}