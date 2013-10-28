using System;
using Cherry.Progress.Contracts.Portable;

namespace Cherry.Progress.Cherry.Portable
{
    public class CherryProgress : CherryProgressBase
    {
        private int? _max;
        private int? _current;

        public override int? Max
        {
            get { return _max; }
            set
            {
                if (value.HasValue && Current.HasValue && Current.Value > value.Value)
                {
                    throw new InvalidOperationException("The IProgress.Max cannot be lower than its IProgress.Current");
                }
                _max = value;
                Display.OnProgressChanged(this);
            }
        }

        public override int? Current
        {
            get { return _current; }
            set
            {
                if (value.HasValue && Max.HasValue && Max.Value < value.Value)
                {
                    throw new InvalidOperationException("The IProgress.Current cannot be greater than its IProgress.Max");
                }
                _current = value;
                Display.OnProgressChanged(this);
            }
        }

        public CherryProgress(string key, IProgressDisplay display)
            : base(key, display)
        {
        }

    }
}