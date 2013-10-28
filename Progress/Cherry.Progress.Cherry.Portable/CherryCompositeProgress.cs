using System;
using System.Collections.Generic;
using System.Linq;
using Cherry.Progress.Contracts.Portable;

namespace Cherry.Progress.Cherry.Portable
{
    public class CherryCompositeProgress : CherryProgressBase, ICompositeProgress, IProgressDisplay
    {
        private readonly List<IProgress> _startedSubProgresses = new List<IProgress>();
        private readonly List<IProgress> _completedSubProgresses = new List<IProgress>();
        private readonly object _syncRoot = new object();
        private int? _maxCache;
        private bool _maxCacheValid;
        private int? _currentCache;
        private bool _currentCacheValid;

        public override int? Max
        {
            get
            {
                lock (_syncRoot)
                {
                    if (!_maxCacheValid)
                    {
                        _maxCache = ComputeMax();
                        _maxCacheValid = true;
                    }
                    return _maxCache;
                }
            }
            set
            {
                throw new InvalidOperationException("Cannot set the Max value of a ICompositeProgress. The Max value is determined automatically by its child progresses.");
            }
        }

        public override int? Current
        {
            get
            {
                lock (_syncRoot)
                {
                    if (!_currentCacheValid)
                    {
                        _currentCache = ComputeCurrent();
                        _currentCacheValid = true;
                    }
                    return _currentCache;
                }
            }
            set
            {
                throw new InvalidOperationException("Cannot set the Current value of a ICompositeProgress. The Current value is determined automatically by its child progresses.");
            }
        }

        public CherryCompositeProgress(string key, IProgressDisplay display) : base(key, display)
        {
        }

        public IProgress CreateSubProgress(string key)
        {
            return new CherryProgress(key, this);
        }

        public ICompositeProgress CreateSubCompositeProgress(string key)
        {
            return new CherryCompositeProgress(key, this);
        }

        public void OnProgressStarted(IProgress progress)
        {
            lock (_syncRoot)
            {
                _maxCacheValid = false;
                _currentCacheValid = false;
                _startedSubProgresses.Add(progress);
            }
            Display.OnProgressChanged(this);
        }

        public void OnProgressChanged(IProgress progress)
        {
            lock (_syncRoot)
            {
                _maxCacheValid = false;
                _currentCacheValid = false;
            }
            Display.OnProgressChanged(this);
        }

        public void OnProgressCompleted(IProgress progress)
        {
            lock (_syncRoot)
            {
                _maxCacheValid = false;
                _currentCacheValid = false;

                _startedSubProgresses.Remove(progress);
                _completedSubProgresses.Add(progress);
            }
            Display.OnProgressChanged(this);
        }
     
        private int? ComputeMax()
        {
            var all = SumMax(_startedSubProgresses) + SumMax(_completedSubProgresses);
            if (all == 0)
            {
                return null;
            }
            return all;
        }

        private static int SumMax(IEnumerable<IProgress> progresses)
        {
            return progresses.Sum(p =>
            {
                var max = p.Max;
                if (max.HasValue)
                {
                    return max.Value;
                }
                return 1;
            });
        }

        private int? ComputeCurrent()
        {
            var max = Max;
            if (!max.HasValue)
            {
                return null;
            }

            var stillRunningCompleted = _startedSubProgresses.Sum(p =>
            {
                var c = p.Current;
                return c.HasValue ? c.Value : 0;
            });

            var completed = _completedSubProgresses.Sum(p =>
            {
                var m = p.Max;
                return m.HasValue ? m.Value : 1;
            });

            var total = stillRunningCompleted + completed;
            return Math.Min(max.Value, total);
        }
    }
}