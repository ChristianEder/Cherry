namespace Cherry.Tasks
{
    public interface ITaskFactory
    {
        IScheduler Scheduler { get; }
        ITask Create(Callback callback);
        ITask Create(params Callback[] callbacks);
        ITask<TResult> Create<TResult>(ResultCallback<TResult> callback);

        ITask AddSuccessorTaskTo(
            ITask predessecor,
            Callback successorCallback);

        ITask AddSuccessorTaskTo(
            ITask predessecor,
            params Callback[] successorCallbacks);

        ITask<TResult> AddSuccessorTaskTo<TResult>(
            ITask predessecor,
            ResultCallback<TResult> successorCallback);

        ITask AddSuccessorTaskTo<TPredessecorResult>(
            ITask<TPredessecorResult> predessecor,
            Callback<TPredessecorResult> successorCallback);

        ITask AddSuccessorTaskTo<TPredessecorResult>(
            ITask<TPredessecorResult> predessecor,
            params Callback<TPredessecorResult>[] successorCallbacks);

        ITask<TSuccessorResult> AddSuccessorTaskTo<TPredessecorResult, TSuccessorResult>(
            ITask<TPredessecorResult> predessecor,
            ResultCallback<TPredessecorResult, TSuccessorResult> successorCallback);
    }
}