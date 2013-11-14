namespace Cherry.IoC.Contracts.Portable
{
    internal interface IParametrizedServiceLocator : IServiceLocator
    {
        IParametrizedServiceLocator With<TValue>(TValue value);
        IParametrizedServiceLocator With<TValue>(string key, TValue value);
    }
}