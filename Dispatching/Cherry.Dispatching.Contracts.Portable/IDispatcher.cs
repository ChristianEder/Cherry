using System;

namespace Cherry.Dispatching.Contracts.Portable
{
    public interface IDispatcher
    {
        void Sync(Action action);
        T Sync<T>(Func<T> func);
        void Async(Action action);
    }
}
