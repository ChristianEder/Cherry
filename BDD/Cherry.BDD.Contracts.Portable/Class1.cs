using System;
using System.Collections.Generic;

namespace Cherry.BDD.Contracts.Portable
{
    public interface IUser
    {
        bool Is { get; }
    }

    public interface IAnyUser : IUser
    {
        
    }

    public class AnyUser : IAnyUser
    {
        public bool Is { get { return true; } }
    }

    public interface IPrecondition
    {
    }

    public interface ITrigger
    {
    }

    public interface IFunctionality
    {
        
    }

    public interface IFeature
    {
        string Name { get; }
        IEnumerable<IUser> Users { get; }
        IEnumerable<IPrecondition> Preconditions { get; }
        IEnumerable<ITrigger> Triggers { get; }
        IEnumerable<IFunctionality> Functionalities { get; } 
    }

    public interface IFeatureDescriptor
    {
        IFeature Describe(IFeatureBuilder builder);
    }

    public interface IFeatureBuilder
    {
        IFeatureBuilderWithUser AsA<TUser>() where TUser : IUser;
    }

    public interface IFeatureBuilderWithUser
    {
        IFeatureBuilderWithUserAndPreconditions GivenThat<TPrecondition>() where TPrecondition : IPrecondition;
    }

    public interface IFeatureBuilderWithUserAndPreconditions
    {
        IFeatureBuilderWithUserAndPreconditions And<TPrecondition>() where TPrecondition : IPrecondition;
        IFeatureBuilderWithUserAndPreconditionsAndTriggers When<TTrigger>() where TTrigger : ITrigger;
    }

    public interface IFeatureBuilderWithUserAndPreconditionsAndTriggers
    {
        IFeatureBuilderWithUserAndPreconditionsAndTriggers Or<TTrigger>() where TTrigger : ITrigger;
        IFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities Then<TFunctionality>() where TFunctionality : IFunctionality;
    }

    public interface IFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities
    {
        IFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities And<TFunctionality>() where TFunctionality : IFunctionality;
        IFeature Build(Func<string> featureName);
    }

    public class LearningTest
    {
        public LearningTest(IFeatureBuilder builder)
        {
            var feature = builder
                .AsA<IUser>()
                .GivenThat<IPrecondition>()
                .And<IPrecondition>()
                .When<ITrigger>()
                .Or<ITrigger>()
                .Then<IFunctionality>()
                .And<IFunctionality>()
                .Build(() => "");
        }
    }
}
