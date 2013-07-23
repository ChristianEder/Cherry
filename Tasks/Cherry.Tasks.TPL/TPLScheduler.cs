using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cherry.Tasks.TPL
{
    public class TPLScheduler : IScheduler
    {
        public void Schedule(ICancellationToken cancellationToken, Callback onCompleted, Callback<Exception> onError, params Callback[] callbacks)
        {
            var tasks = callbacks.Select(c => Task.Factory.StartNew(() => c()));
            Task.WhenAll(tasks)
                .ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                        {
                            onError(t.Exception);
                        }
                        else
                        {
                            onCompleted();
                        }
                    });
        }

        public void Schedule<TResult>(ICancellationToken cancellationToken, Callback<TResult> onCompleted, Callback<Exception> onError, ResultCallback<TResult> callback)
        {
            Task<TResult> task = Task.Factory.StartNew(() => callback());
            task.ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                        {
                            onError(t.Exception);
                        }
                        else
                        {
                            onCompleted(t.Result);
                        }
                    });
        }
    }
}
