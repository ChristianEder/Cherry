namespace Cherry.Progress.Contracts.Portable
{
    public static class ProgressExtensions
    {
        public static T Increment<T>(this T progress) where T : IProgress
        {
            return progress.Increment(1);
        }

        public static T Increment<T>(this T progress, int amount) where T : IProgress
        {
            var current = progress.Current.GetValueOrDefault();
            progress.Current = current + amount;
            return progress;
        }
    }
}