namespace Cherry.Progress.Tests.Display
{
    public class TestableProgressState
    {
        public TestableProgressState(string key, string title, int? max, int? current)
        {
            Current = current;
            Max = max;
            Title = title;
            Key = key;
        }

        public string Key { get; private set; }
        public string Title { get; private set; }
        public int? Max { get; private set; }
        public int? Current { get; private set; }
    }
}