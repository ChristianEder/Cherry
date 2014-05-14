using Cherry.BDD.Contracts.Portable;
using Cherry.BDD.Sample.WPF.Behavior.Users.Contracts;

namespace Cherry.BDD.Sample.WPF.Behavior
{
    public class LoginFeature : IFeatureDescriptor
    {
        public IFeature Describe(IFeatureBuilder builder)
        {
            return builder.AsA<IAnonymousUser>()
                .GivenThat<IPrecondition>()
                .When<ITrigger>()
                .Then<IFunctionality>()
                .Build(() => "");
        }
    }
}
