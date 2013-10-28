namespace Cherry.IoC.Cherry.Portable.Resolver
{
    internal class SingletonInstanceResolver : IResolver
    {
        private readonly object _instance;

        public SingletonInstanceResolver(object instance)
        {
            _instance = instance;
        }

        public object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current)
        {
            return _instance;
        }
    }
}