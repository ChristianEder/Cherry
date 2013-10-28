namespace Cherry.Progress.Contracts.Portable
{
    public interface IProgressDisplay
    {
        void OnProgressStarted(IProgress progress);
        void OnProgressChanged(IProgress progress);
        void OnProgressCompleted(IProgress progress);
    }
}