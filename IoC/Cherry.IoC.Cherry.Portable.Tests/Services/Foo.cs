namespace Cherry.IoC.Tests.Services
{
    public class Foo : IFoo
    {
        public int DisposedCalls { get; private set; }

        public void Bar()
        {
            
        }

        public void Dispose()
        {
            DisposedCalls++;
        }
    }
}
