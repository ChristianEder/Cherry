using System.Collections.Generic;
using Cherry.Progress.Contracts.Portable;

namespace Cherry.Progress.Tests.Display
{
    public class TestableProgressDisplay : IProgressDisplay
    {
        public List<string> StartedKeys = new List<string>();
        public List<TestableProgressState> Changes = new List<TestableProgressState>();
        public List<string> CompletedKeys = new List<string>();

        public void OnProgressStarted(IProgress progress)
        {
            StartedKeys.Add(progress.Key);
        }

        public void OnProgressChanged(IProgress progress)
        {
            Changes.Add(new TestableProgressState(progress.Key, progress.Title, progress.Max, progress.Current));
        }

        public void OnProgressCompleted(IProgress progress)
        {
            CompletedKeys.Add(progress.Key);
        }
    }
}
