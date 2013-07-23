using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.Tasks.Tests
{
    public abstract class TaskSchedulerTestsBase
    {
        protected ITaskFactory Factory;


        protected void DoTestCreateSingleTask()
        {
            ITask task = Factory.Create(() => { });

            AssertIsNotStarted(task);
        }

        protected void DoTestCreateSingleResultTask()
        {
            ITask task = Factory.Create(() => 3);
            AssertIsNotStarted(task);
        }

        protected async Task DoTestSimpleRun()
        {
            int count = 1;
            var task = Factory.Create(() => { count += 1; });
            task.Start();
            await Await(task);
            Assert.AreEqual(2, count);
        }

        protected async Task DoTestSimpleResult()
        {
            var task = Factory.Create(() => 17);
            task.Start();
            int i = await Await(task);
            Assert.AreEqual(17, i);
        }

        protected async Task DoTestRunAdditionChain()
        {
            var task = Factory.Create(() => 10)
                               .Then(i => i + 5)
                               .Then(i => i + 2);
            AssertIsNotStarted(task);
            task.Start();
            var result = await Await(task);
            AssertIsCompletedSuccessfully(task);
            Assert.AreEqual(
                17,
               result);
        }

        protected async Task DoTestRunParallelAndSubsequent()
        {
            int i = 0;
            var task = Factory.Create(() => { i += 10; })
                               .Then(
                                   () => { i += 1; },
                                   () => { i += 1; },
                                   () => { i += 1; },
                                   () => { i += 1; },
                                   () => { i += 1; })
                               .Then(() => { i *= 2; });
            AssertIsNotStarted(task);
            task.Start();
            await Await(task);
            AssertIsCompletedSuccessfully(task);
            Assert.AreEqual(
                30,
                i);
        }

        private static void AssertIsNotStarted(ITask task)
        {
            Assert.IsNotNull(task);
            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsCancelled);
            Assert.IsFalse(task.IsFaulted);
        }

        private static void AssertIsCompletedSuccessfully(ITask task)
        {
            Assert.IsNotNull(task);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsFalse(task.IsCancelled);
            Assert.IsFalse(task.IsFaulted);
        }

        private Task Await(ITask task)
        {
            var awaiter = new TaskCompletionSource<object>();
            task.Completed += t => CopyResult(
                t,
                awaiter);
            return awaiter.Task;
        }

        private Task<TResult> Await<TResult>(ITask<TResult> task)
        {
            var awaiter = new TaskCompletionSource<TResult>();
            task.Completed += t => CopyResult(
                t,
                awaiter);
            return awaiter.Task;
        }

        private void CopyResult<TResult>(ITask t, TaskCompletionSource<TResult> awaiter)
        {
            if (t.IsCancelled)
            {
                awaiter.SetCanceled();
                return;
            }

            if (t.IsFaulted)
            {
                awaiter.SetException(t.Exception);
                return;
            }

            var resultTask = t as ITask<TResult>;
            if (resultTask != null)
            {
                awaiter.SetResult(resultTask.Result);
            }
            else
            {
                awaiter.SetResult(default(TResult));
            }
        }
    }
}
