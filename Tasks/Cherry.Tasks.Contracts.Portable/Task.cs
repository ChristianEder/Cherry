using System;

namespace Cherry.Tasks
{
    internal class Task : ITask,
        ICancellationToken
    {
        protected readonly ITaskFactory Factory;
        private readonly ITask _predessor;
        private readonly Callback[] _callbacks;

        public Task(
            ITaskFactory factory,
            ITask predessor,
            params Callback[] callbacks)
            : this(factory,
                predessor)
        {
            _callbacks = callbacks;
        }

        protected Task(
            ITaskFactory factory,
            ITask predessor)
        {
            Factory = factory;
            _predessor = predessor;
            if (predessor != null)
            {
                predessor.Completed += predessor_Completed;
            }
        }

        private void predessor_Completed(ITask task)
        {
            task.Completed -= predessor_Completed;
            if (task.IsCancelled)
            {
                Run();
            }
        }

        private event TaskCompletionHandler InnerCompleted;

        public event TaskCompletionHandler Completed
        {
            add
            {
                InnerCompleted += value;
                if (IsCompleted)
                {
                    value(this);
                }
            }
            remove { InnerCompleted -= value; }
        }

        public bool IsCompleted { get; private set; }

        public bool IsCancelled { get; private set; }

        public bool IsFaulted { get; private set; }

        public Exception Exception { get; private set; }

        public void Cancel()
        {
            IsCancelled = true;
            OnCompleted();
        }

        protected void SetExcetion(Exception ex)
        {
            Exception = ex;
            IsFaulted = true;
            OnCompleted();
        }

        protected virtual void OnCompleted()
        {
            IsCompleted = true;
            if (InnerCompleted != null)
            {
                InnerCompleted(this);
            }
        }

        public void Start()
        {
            if (_predessor != null)
            {
                _predessor.Start();
            }
            Run();
        }

        protected virtual void Run()
        {
            Factory.Scheduler.Schedule(
                this,
                OnCompleted,
                SetExcetion,
                _callbacks);
        }

        public ITask Then(Callback callback)
        {
            return Factory.AddSuccessorTaskTo(
                this,
                callback);
        }

        public ITask Then(params Callback[] callbacks)
        {
            return Factory.AddSuccessorTaskTo(
                this,
                callbacks);
        }

        public ITask<TResult> Then<TResult>(ResultCallback<TResult> callback)
        {
            return Factory.AddSuccessorTaskTo(
                this,
                callback);
        }
    }

    internal class Task<TResult> : Task,
        ITask<TResult>
    {
        private readonly ResultCallback<TResult> _callback;

        public Task(
            ITaskFactory factory,
            ITask predessor,
            ResultCallback<TResult> callback)
            : base(factory,
                predessor)
        {
            _callback = callback;
        }

        public TResult Result { get; private set; }

        private void SetResult(TResult result)
        {
            Result = result;
            OnCompleted();
        }

        public ITask Then(Callback<TResult> callback)
        {
            return Factory.AddSuccessorTaskTo(
                this,
                callback);
        }

        public ITask Then(params Callback<TResult>[] callbacks)
        {
            return Factory.AddSuccessorTaskTo(
                this,
                callbacks);
        }

        public ITask<TNewResult> Then<TNewResult>(ResultCallback<TResult, TNewResult> callback)
        {
            return Factory.AddSuccessorTaskTo(
                this,
                callback);
        }

        protected override void Run()
        {
            Factory.Scheduler.Schedule(
                this,
                SetResult,
                SetExcetion,
                _callback);
        }
    }
}