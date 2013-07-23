using System.Linq;

namespace Cherry.Tasks
{
    public class TaskFactory : ITaskFactory
    {
        public TaskFactory(IScheduler scheduler)
        {
            Scheduler = scheduler;
        }

        public ITask Create(Callback callback)
        {
            return new Task(
                this,
                null,
                callback);
        }


        public ITask<TResult> Create<TResult>(ResultCallback<TResult> callback)
        {
            return new Task<TResult>(
                this,
                null,
                callback);
        }


        public ITask Create(params Callback[] callbacks)
        {
            return new Task(
                this,
                null,
                callbacks);
        }

        public ITask AddSuccessorTaskTo(
            ITask predessecor,
            Callback successorCallback)
        {
            return new Task(
                this,
                predessecor,
                successorCallback);
        }

        public ITask AddSuccessorTaskTo(
            ITask predessecor,
            params Callback[] successorCallbacks)
        {
            return new Task(
                this,
                predessecor,
                successorCallbacks);
        }

        public ITask<TResult> AddSuccessorTaskTo<TResult>(
            ITask predessecor,
            ResultCallback<TResult> successorCallback)
        {
            return new Task<TResult>(
                this,
                predessecor,
                successorCallback);
        }

        public ITask AddSuccessorTaskTo<TPredessecorResult>(
            ITask<TPredessecorResult> predessecor,
            Callback<TPredessecorResult> successorCallback)
        {
            return new Task(
                this,
                predessecor,
                () => successorCallback(predessecor.Result));
        }

        public ITask AddSuccessorTaskTo<TPredessecorResult>(
            ITask<TPredessecorResult> predessecor,
            params Callback<TPredessecorResult>[] successorCallbacks)
        {
            return new Task(
                this,
                predessecor,
                successorCallbacks.Select(c => (Callback)(() => c(predessecor.Result)))
                                  .ToArray());
        }

        public ITask<TSuccessorResult> AddSuccessorTaskTo<TPredessecorResult, TSuccessorResult>(
            ITask<TPredessecorResult> predessecor,
            ResultCallback<TPredessecorResult, TSuccessorResult> successorCallback)
        {
            return new Task<TSuccessorResult>(
                this,
                predessecor,
                () => successorCallback(predessecor.Result));
        }

        public IScheduler Scheduler { get; private set; }
    }
}