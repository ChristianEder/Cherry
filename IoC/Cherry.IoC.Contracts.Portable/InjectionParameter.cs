namespace Cherry.IoC.Contracts.Portable
{
    public class InjectionParameter
    {
        public InjectionParameter(string key, object value)
        {
            Value = value;
            Key = key;
        }

        public string Key { get; private set; }
        public object Value { get; private set; }
    }
}