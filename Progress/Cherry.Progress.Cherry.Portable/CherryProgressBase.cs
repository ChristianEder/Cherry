using Cherry.Progress.Contracts.Portable;

namespace Cherry.Progress.Cherry.Portable
{
    public abstract class CherryProgressBase : IProgress
    {
        private readonly IProgressDisplay _display;
        private string _title;
        private string _description;

        protected CherryProgressBase(string key, IProgressDisplay display)
        {
            _display = display;
            Key = key;
            Display.OnProgressStarted(this);
        }

        public abstract int? Max { get; set; }
        public abstract int? Current { get; set; }
        public string Key { get; private set; }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                Display.OnProgressChanged(this);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                Display.OnProgressChanged(this);
            }
        }

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