namespace Cherry.IoC.Cherry.Portable.Resolver
{
    public interface IResolver
    {
        object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current);
    }
}