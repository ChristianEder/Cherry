using Cherry.Progress.Contracts.Portable;

namespace Cherry.Progress.Cherry.Portable
{
    public abstract class CherryProgressBase : IProgress
    {
        private readonly IProgressDisplay _display;

        protected CherryProgressBase(string key, IProgressDisplay display)
        {
            _display = display;
            Key = key;
            Display.OnProgressStarted(this);
        }

        public abstract int? Max { get; set; }
        public abstract int? Current { get; set; }
        public string Key { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }

        protected IProgressDisplay Display
        {
            get { return _display; }
        }

        public virtual void Dispose()
        {
            Display.OnProgressCompleted(this);
        }
    }
}