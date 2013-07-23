namespace Cherry.Tasks
{
    public interface ICancellationToken
    {
        void Cancel();
        bool IsCancelled { get; }
    }
}