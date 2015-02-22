using Cherry.IoC.Contracts.Portable;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cherry.BDD.Contracts.Portable
{
    #region Users, Preconditions, Triggers, Functionalities

    public interface IUser
    {
        Task<bool> Is();
    }

    public interface IAnyUser : IUser
    {

    }

    public class AnyUser : IAnyUser
    {
        public Task<bool> Is() { return Task.FromResult(true); }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    public interface IPrecondition
    {
        Task<bool> IsFulfilled();
    }

    [TextualDescriptionAttribute(Description="")]
    public class FulfilledPrecondition : IPrecondition
    {
        public Task<bool> IsFulfilled() { return Task.FromResult(true); }
    }

    [TextualDescriptionAttribute(Description = "Never")]
    public class UnfulfilledPrecondition : IPrecondition
    {
        public Task<bool> IsFulfilled() { return Task.FromResult(false); }
    }

    public interface ITrigger
    {
    }

    public interface IFunctionality
    {
        Task Run();
    }

    #endregion

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TextualDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
    }

    public interface IFeatureSetTextualDescriptor 
    {
        string Describe(IFeatureSet featureSet);
    }

    public interface IFeatureTextualDescriptor
    {
        string Describe(IFeature feature);
    }

    public interface IUserTextualDescriptor
    {
        string Describe(Type user, bool includePretext);
    }

    public interface IPreconditionTextualDescriptor
    {
        string Describe(Type precondition, bool includePretext);
    }

    public interface ITriggerTextualDescriptor
    {
        string Describe(Type trigger, bool includePretext);
    }

    public interface IFunctionalityTextualDescriptor
    {
        string Describe(Type functionality, bool includePretext);
    }

    public interface IFeatureExecutionContext
    {
        IFeature Feature { get; set; }
        IServiceLocator ServiceLocator { get; set; }
    }

    public interface IFeature
    {
        string Name { get; }
        Type User { get; }
        Type Precondition { get; }
        Type Trigger { get; }
        Type Functionality { get; }
    }

    public interface IFeatureSet
    {
        string Name { get; }
        IEnumerable<IFeature> OwnFeatures { get; }
        IEnumerable<IFeature> AllFeatures { get; }
        IEnumerable<IFeatureSet> SubSets { get; }
        IFeatureSet CreateSubSet(Func<string> subsetName);
        IFeatureSet AddSubset(IFeatureSet subset);
        IFeatureSet AddFeature(IFeature feature);
        IFeatureBuilder Builder { get; }
    }

    public static class FeatureBuilderConvenience
    {
        public static IFeatureBuilderWithUserAndPreconditions GivenThat<TPrecondition>(this IFeatureBuilder builder)
            where TPrecondition : IPrecondition
        {
            return builder.AsA<IAnyUser>().GivenThat<TPrecondition>();
        }

        public static IFeatureBuilderWithUserAndPreconditions NotGivenThat<TPrecondition>(this IFeatureBuilder builder)
            where TPrecondition : IPrecondition
        {
            return builder.AsA<IAnyUser>().NotGivenThat<TPrecondition>();
        }

        public static IFeatureBuilderWithUserAndPreconditionsAndTriggers When<TTrigger>(this IFeatureBuilder builder)
            where TTrigger : ITrigger
        {
            return builder.AsA<IAnyUser>().GivenThat<FulfilledPrecondition>().When<TTrigger>();
        }

        public static IFeatureBuilderWithUserAndPreconditionsAndTriggers After<TFunctionality>(this IFeatureBuilder builder)
           where TFunctionality : IFunctionality
        {
            return builder.AsA<IAnyUser>().GivenThat<FulfilledPrecondition>().After<TFunctionality>();
        }

        public static IFeatureBuilderWithUserAndPreconditionsAndTriggers Before<TFunctionality>(this IFeatureBuilder builder)
           where TFunctionality : IFunctionality
        {
            return builder.AsA<IAnyUser>().GivenThat<FulfilledPrecondition>().Before<TFunctionality>();
        }

        public static IFeatureBuilderWithUserAndPreconditions NotGivenThat<TPrecondition>(this IFeatureBuilderWithUser builder)
          where TPrecondition : IPrecondition
        {
            return builder.GivenThat<TPrecondition>().Not();
        }

        public static IFeatureBuilderWithUserAndPreconditionsAndTriggers When<TTrigger>(this IFeatureBuilderWithUser builder)
           where TTrigger : ITrigger
        {
            return builder.GivenThat<FulfilledPrecondition>().When<TTrigger>();
        }

        public static IFeatureBuilderWithUserAndPreconditionsAndTriggers After<TFunctionality>(this IFeatureBuilderWithUser builder)
            where TFunctionality : IFunctionality
        {
            return builder.GivenThat<FulfilledPrecondition>().After<TFunctionality>();
        }

        public static IFeatureBuilderWithUserAndPreconditionsAndTriggers Before<TFunctionality>(this IFeatureBuilderWithUser builder)
            where TFunctionality : IFunctionality
        {
            return builder.GivenThat<FulfilledPrecondition>().Before<TFunctionality>();
        }
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
        IFeatureBuilderWithUserAndPreconditions Not();

        IFeatureBuilderWithUserAndPreconditions And<TPrecondition>() where TPrecondition : IPrecondition;
        IFeatureBuilderWithUserAndPreconditions AndNot<TPrecondition>() where TPrecondition : IPrecondition;
        IFeatureBuilderWithUserAndPreconditions Or<TPrecondition>() where TPrecondition : IPrecondition;
        IFeatureBuilderWithUserAndPreconditions OrNot<TPrecondition>() where TPrecondition : IPrecondition;

        IFeatureBuilderWithUserAndPreconditionsAndTriggers When<TTrigger>() where TTrigger : ITrigger;
        IFeatureBuilderWithUserAndPreconditionsAndTriggers After<TFunctionality>() where TFunctionality : IFunctionality;
        IFeatureBuilderWithUserAndPreconditionsAndTriggers Before<TFunctionality>() where TFunctionality : IFunctionality;
    }

    public interface IFeatureBuilderWithUserAndPreconditionsAndTriggers
    {
        IFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities Then<TFunctionality>() where TFunctionality : IFunctionality;
    }

    public interface IFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities
    {
        IFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities Then<TFunctionality>() where TFunctionality : IFunctionality;

        IFeature Build(Func<string> featureName);
    }

    public class LearningTest
    {
        public LearningTest(IFeatureBuilder builder)
        {
            var feature = builder
                .AsA<IUser>()
                .GivenThat<IPrecondition>()
                .When<ITrigger>()
                .Then<IFunctionality>()
                .Build(() => "");
        }

        public LearningTest(IFeatureSet application)
        {
            var login = application.CreateSubSet(() => "Login");
            var music = application.CreateSubSet(() => "Music");

            var feature = login.Builder
                .AsA<IUser>()
                .GivenThat<IPrecondition>()
                .When<ITrigger>()
                .Then<IFunctionality>()
                .Build(() => "");
        }
    }
}
