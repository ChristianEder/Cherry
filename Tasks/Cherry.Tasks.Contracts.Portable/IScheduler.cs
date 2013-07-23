using System;

namespace Cherry.Tasks
{
    public interface IScheduler
    {
        void Schedule(
            ICancellationToken cancellationToken,
            Callback onCompleted,
            Callback<Exception> onError,
            params Callback[] callbacks);

        void Schedule<TResult>(
            ICancellationToken cancellationToken,
            Callback<TResult> onCompleted,
            Callback<Exception> onError,
            ResultCallback<TResult> callback);
    }
}