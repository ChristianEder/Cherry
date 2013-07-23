using System;

namespace Cherry.Tasks
{
    public class SychronousScheduler : IScheduler
    {
        public void Schedule(
            ICancellationToken cancellationToken,
            Callback onCompleted,
            Callback<Exception> onError,
            params Callback[] callbacks)
        {
            try
            {
                foreach (var callback in callbacks)
                {
                    if (!cancellationToken.IsCancelled)
                    {
                        callback();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                onError(ex);
                return;
            }
            onCompleted();
        }

        public void Schedule<TResult>(
            ICancellationToken cancellationToken,
            Callback<TResult> onCompleted,
            Callback<Exception> onError,
            ResultCallback<TResult> callback)
        {
            TResult result;
            try
            {
                if (!cancellationToken.IsCancelled)
                {
                    result = callback();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                onError(ex);
                return;
            }
            onCompleted(result);
        }
    }
}