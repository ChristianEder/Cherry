namespace Cherry.IoC.Cherry.Portable.Resolver
{
    internal interface IResolver
    {
        object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current);
    }
}