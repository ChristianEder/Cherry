using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Cherry.Dispatching.Contracts.Portable;

namespace Cherry.Dispatching.Client.Net45
{
    public class AppDispatcher : IDispatcher
    {
        public void Sync(Action action)
        {
            var dispatcher = GetDispatcher();
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(action);
                return;
            }
            action();
        }


        public T Sync<T>(Func<T> func)
        {
            var dispatcher = GetDispatcher();
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                return dispatcher.Invoke(func);
            }
            return func();
        }

        public void Async(Action action)
        {
            var dispatcher = GetDispatcher();
            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(action);
                return;
            }
            Task.Factory.StartNew(action);
        }

        private static Dispatcher GetDispatcher()
        {
            if (Application.Current != null && Application.Current.Dispatcher != null)
            {
                return Application.Current.Dispatcher;
            }
            return Dispatcher.CurrentDispatcher;
        }
    }
}
