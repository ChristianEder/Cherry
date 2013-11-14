using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Tests.Services
{
    public class Bar : IBar
    {
        public void Foo()
        {
        }
    }

    public class BarUsingSomething : IBar
    {
        public ISomethingUsingTheLocatorAndRegistry Something { get; private set; }
        public IServiceLocator Locator { get; private set; }

        public BarUsingSomething(ISomethingUsingTheLocatorAndRegistry something, IServiceLocator locator)
        {
            Something = something;
            Locator = locator;
        }

        public void Foo()
        {
        }
    }

    public class BarWithStringParameter : IBar
    {
        public string Parameter { get; set; }

        public BarWithStringParameter(string parameter)
        {
            Parameter = parameter;
        }

        public void Foo()
        {
            
        }
    }

    public class BarWithMultipleParameters : IBar
    {
        public IFoo Parameter1 { get; private set; }
        public string Parameter2 { get; private set; }
        public string Parameter3 { get; private set; }
        public int Parameter4 { get; private set; }

        public BarWithMultipleParameters(IFoo parameter1, string parameter2, string parameter3, int parameter4)
        {
            Parameter1 = parameter1;
            Parameter2 = parameter2;
            Parameter3 = parameter3;
            Parameter4 = parameter4;
        }

        public void Foo()
        {
            
        }
    }
}