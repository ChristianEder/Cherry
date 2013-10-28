namespace Cherry.Progress.Contracts.Portable
{
    public interface ICompositeProgress : IProgress
    {
        IProgress CreateSubProgress(string key);
        ICompositeProgress CreateSubCompositeProgress(string key);
    }
}
