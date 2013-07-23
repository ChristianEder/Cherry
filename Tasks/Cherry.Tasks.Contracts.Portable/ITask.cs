using System;

namespace Cherry.Tasks
{
    public interface ITask
    {
        event TaskCompletionHandler Completed;
        bool IsCompleted { get; }
        bool IsCancelled { get; }
        bool IsFaulted { get; }
        Exception Exception { get; }

        void Cancel();
        void Start();
        ITask Then(Callback callback);
        ITask Then(params Callback[] callback);
        ITask<TResult> Then<TResult>(ResultCallback<TResult> callback);
    }

    public interface ITask<out TResult> : ITask
    {
        TResult Result { get; }
        ITask Then(Callback<TResult> callback);
        ITask Then(params Callback<TResult>[] callback);
        ITask<TNewResult> Then<TNewResult>(ResultCallback<TResult, TNewResult> callback);
    }
}