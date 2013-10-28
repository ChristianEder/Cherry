using System.Diagnostics;
using Cherry.Progress.Contracts.Portable;

namespace Cherry.Progress.Cherry.Portable
{
    public class DebugOutputProgressDisplay : IProgressDisplay
    {
        public void OnProgressStarted(IProgress progress)
        {
            Debug.WriteLine("Progress started: {0}", progress.Key);
        }

        public void OnProgressChanged(IProgress progress)
        {
            Debug.WriteLine("Progress changed: {0} - {1} - {3} / {2}",
                progress.Key,
                progress.Title,
                progress.Max,
                progress.Current);
        }

        public void OnProgressCompleted(IProgress progress)
        {
            Debug.WriteLine("Progress completed: {0}", progress.Key);
        }
    }
}