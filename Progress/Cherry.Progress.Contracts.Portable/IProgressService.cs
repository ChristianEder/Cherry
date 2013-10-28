namespace Cherry.Progress.Contracts.Portable
{
    public interface IProgressService
    {
        IProgress CreateProgress(string key);
        ICompositeProgress CreateCompositeProgress(string key);
    }
}