using Cherry.BDD.Contracts.Portable;
using Cherry.IoC.Contracts.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cherry.BDD.Cherry.Portable
{
    internal class AfterFunctionalityTrigger<TFunctionality> : ITrigger
       where TFunctionality : IFunctionality
    {
        [TextualDescriptionAttribute]
        private static string Describe(IFunctionalityTextualDescriptor d)
        {
            return "After " + d.Describe(typeof(TFunctionality), false);
        }
    }

    internal class BeforeFunctionalityTrigger<TFunctionality> : ITrigger
      where TFunctionality : IFunctionality
    {

        [TextualDescriptionAttribute]
        private static string Describe(IFunctionalityTextualDescriptor d)
        {
            return "Before " + d.Describe(typeof(TFunctionality), false);
        }
    }

    internal class NotPrecondition<TNegated> : IPrecondition
        where TNegated : IPrecondition
    {
        private TNegated _negated;
        public NotPrecondition(TNegated negated)
        {
            _negated = negated;
        }

        public async Task<bool> IsFulfilled()
        {
            var n = await _negated.IsFulfilled();
            return !n;
        }

        [TextualDescriptionAttribute]
        private static string Describe(IPreconditionTextualDescriptor d)
        {
            var x = "Given not " + d.Describe(typeof(TNegated), false);
            return "Given not " + d.Describe(typeof(TNegated), false);
        }
    }

    internal class AndPrecondition<TFirst, TSecond> : IPrecondition
        where TFirst : IPrecondition
        where TSecond : IPrecondition
    {
        private TSecond _second;
        private TFirst _first;
        public AndPrecondition(TFirst first, TSecond second)
        {
            _first = first;
            _second = second;
        }

        public async Task<bool> IsFulfilled()
        {
            var f = await _first.IsFulfilled();
            if (!f)
            {
                return f;
            }

            var s = await _second.IsFulfilled();
            return s;
        }

        [TextualDescriptionAttribute]
        private static string Describe(IPreconditionTextualDescriptor d)
        {
            return d.Describe(typeof(TFirst), true) + " and " + d.Describe(typeof(TSecond), false);
        }
    }

    internal class OrPrecondition<TFirst, TSecond> : IPrecondition
        where TFirst : IPrecondition
        where TSecond : IPrecondition
    {
        private TSecond _second;
        private TFirst _first;
        public OrPrecondition(TFirst first, TSecond second)
        {
            _first = first;
            _second = second;
        }

        public async Task<bool> IsFulfilled()
        {
            var f = await _first.IsFulfilled();
            if (f)
            {
                return f;
            }

            var s = await _second.IsFulfilled();
            return s;
        }

        [TextualDescriptionAttribute]
        private static string Describe(IPreconditionTextualDescriptor d)
        {
            return d.Describe(typeof(TFirst), true) + " or " + d.Describe(typeof(TSecond), false);
        }
    }

    internal class FunctionalityChain<TFirst, TSecond> : IFunctionality
        where TFirst : IFunctionality
        where TSecond : IFunctionality
    {
        private TSecond _second;
        private TFirst _first;
        public FunctionalityChain(TFirst first, TSecond second)
        {
            _first = first;
            _second = second;
        }

        public async Task Run()
        {
            await _first.Run();
            await _second.Run();
        }

        [TextualDescriptionAttribute]
        private static string Describe(IFunctionalityTextualDescriptor d)
        {
            return d.Describe(typeof(TFirst), true) + Environment.NewLine + d.Describe(typeof(TSecond), true);
        }
    }

    public interface ITargetedFeatureBuilder : IFeatureBuilder
    {
        FeatureSet Target { get; set; }
    }

    public class BehaviorDescriptorBase
    {
        private IServiceLocator _serviceLocator;
        public BehaviorDescriptorBase(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        protected string DescribeType(Type type, string pretext)
        {
            var classDescriptor = type.GetTypeInfo().GetCustomAttribute<TextualDescriptionAttribute>();

            if (classDescriptor != null)
            {
                return classDescriptor.Description;
            }

            var methodDescriptor = type.GetTypeInfo().DeclaredMethods
                .Select(m => new { Method = m, Descriptor = m.GetCustomAttribute<TextualDescriptionAttribute>() })
                .FirstOrDefault(m => m.Descriptor != null);

            if (methodDescriptor != null)
            {
                var parameters = methodDescriptor.Method.GetParameters().Select(p => _serviceLocator.Get(p.ParameterType)).ToArray();
                var d = methodDescriptor.Method.Invoke(null, parameters).ToString();
                return d;
            }

            return pretext + type.Name;
        }
    }

    public class UserTextualDescriptor : BehaviorDescriptorBase, IUserTextualDescriptor
    {
        public UserTextualDescriptor(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {

        }

        public string Describe(Type user, bool includePretext)
        {
            return DescribeType(user, (includePretext ? "As a " : string.Empty));
        }
    }

    public class PreconditionTextualDescriptor : BehaviorDescriptorBase, IPreconditionTextualDescriptor
    {
        public PreconditionTextualDescriptor(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {

        }

        public string Describe(Type precondition, bool includePretext)
        {
            return DescribeType(precondition, (includePretext ? "Given that " : string.Empty));
        }
    }

    public class TriggerTextualDescriptor : BehaviorDescriptorBase, ITriggerTextualDescriptor
    {
        public TriggerTextualDescriptor(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {

        }

        public string Describe(Type trigger, bool includePretext)
        {
            return DescribeType(trigger, (includePretext ? "When " : string.Empty));
        }
    }

    public class FunctionalityTextualDescriptor : BehaviorDescriptorBase, IFunctionalityTextualDescriptor
    {
        public FunctionalityTextualDescriptor(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {

        }

        public string Describe(Type functionality, bool includePretext)
        {
            return DescribeType(functionality, (includePretext ? "Then " : string.Empty));
        }
    }

    public class FeatureTextualDescriptor : IFeatureTextualDescriptor
    {
        private IFunctionalityTextualDescriptor _functionality;
        private ITriggerTextualDescriptor _trigger;
        private IPreconditionTextualDescriptor _precondition;
        private IUserTextualDescriptor _user;
        public FeatureTextualDescriptor(IUserTextualDescriptor user, IPreconditionTextualDescriptor precondition, ITriggerTextualDescriptor trigger, IFunctionalityTextualDescriptor functionality)
        {
            _user = user;
            _precondition = precondition;
            _trigger = trigger;
            _functionality = functionality;
        }
        public string Describe(IFeature feature)
        {
            var description = new StringBuilder();
            description.AppendLine(feature.Name);
            Append(description, _user.Describe(feature.User, true));
            Append(description, _precondition.Describe(feature.Precondition, true));
            Append(description, _trigger.Describe(feature.Trigger, true));
            Append(description, _functionality.Describe(feature.Functionality, true));
            return description.ToString();
        }

        private void Append(StringBuilder description, string text)
        {
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    description.AppendLine("  " + line);
                }
            }
        }
    }

    public class FeatureSetTextualDescriptor : IFeatureSetTextualDescriptor
    {
        private IFeatureTextualDescriptor _featureTextualDescriptor;
        public FeatureSetTextualDescriptor(IFeatureTextualDescriptor featureTextualDescriptor)
        {
            _featureTextualDescriptor = featureTextualDescriptor;
        }

        public string Describe(IFeatureSet featureSet)
        {
            var description = new StringBuilder();

            description.AppendLine(featureSet.Name);
            foreach (var feature in featureSet.OwnFeatures)
            {
                Append(_featureTextualDescriptor.Describe(feature), description);
            }
            foreach (var subset in featureSet.SubSets)
            {
                Append(Describe(subset), description);
            }

            return description.ToString();
        }

        private void Append(string text, StringBuilder description)
        {
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                description.AppendLine("  " + line);
            }
        }
    }

    public class FeatureSet : IFeatureSet
    {
        private List<IFeature> _ownFeatures;
        private List<IFeatureSet> _subSets;
        private Func<string> _name;
        private IServiceLocator _serviceLocator;

        public FeatureSet(IServiceLocator serviceLocator, Func<string> name)
        {
            _name = name;
            _serviceLocator = serviceLocator;
            _ownFeatures = new List<IFeature>();
            _subSets = new List<IFeatureSet>();
            var targetedBuilder = (ITargetedFeatureBuilder)serviceLocator.Get(typeof(ITargetedFeatureBuilder));
            targetedBuilder.Target = this;
            Builder = targetedBuilder;
        }

        public string Name
        {
            get { return _name(); }
        }

        public IEnumerable<IFeature> OwnFeatures
        {
            get { return _ownFeatures; }
        }

        public IEnumerable<IFeature> AllFeatures
        {
            get { return _ownFeatures.Concat(_subSets.SelectMany(s => s.AllFeatures)); }
        }

        public IEnumerable<IFeatureSet> SubSets
        {
            get { return _subSets; }
        }

        public IFeatureSet CreateSubSet(Func<string> subsetName)
        {
            var subset = _serviceLocator.Get<IFeatureSet>(new InjectionParameter("name", subsetName));
            _subSets.Add(subset);
            return subset;
        }

        public IFeatureSet AddSubset(IFeatureSet subset)
        {
            _subSets.Add(subset);
            return this;
        }

        public IFeatureBuilder Builder
        {
            get;
            private set;
        }

        public IFeatureSet AddFeature(IFeature feature)
        {
            _ownFeatures.Add(feature);
            return this;
        }
    }

    public abstract class Application
    {
        private Func<string> _name;
        protected Application(Func<string> name)
        {
            _name = name;
        }

        public void Run()
        {
            ServiceRegistry = InitializeServiceRegistry();
            ServiceRegistry.Load<Module>();
            Features = new FeatureSet(ServiceRegistry.Locator, _name);
            InitializeFeatures();
        }

        public IServiceRegistry ServiceRegistry { get; private set; }

        public FeatureSet Features { get; private set; }

        protected abstract IServiceRegistry InitializeServiceRegistry();

        protected abstract void InitializeFeatures();
    }

    public class TargetedFeatureBuilder : ITargetedFeatureBuilder
    {
        public FeatureSet Target
        {
            get;
            set;
        }

        public IFeatureBuilderWithUser AsA<TUser>() where TUser : IUser
        {
            return new TargetedFeatureBuilderWithUser(Target, typeof(TUser));
        }
    }

    internal class TargetedFeatureBuilderWithUser : IFeatureBuilderWithUser
    {
        private readonly FeatureSet _target;
        private Type _userType;

        public TargetedFeatureBuilderWithUser(FeatureSet target, Type userType)
        {
            _target = target;
            _userType = userType;
        }

        public IFeatureBuilderWithUserAndPreconditions GivenThat<TPrecondition>() where TPrecondition : IPrecondition
        {
            return new TargetedFeatureBuilderWithUserAndPreconditions(_target, _userType, typeof(TPrecondition));
        }
    }

    internal class TargetedFeatureBuilderWithUserAndPreconditions : IFeatureBuilderWithUserAndPreconditions
    {
        private readonly FeatureSet _target;
        private Type _userType;
        private Type _preconditionType;

        public TargetedFeatureBuilderWithUserAndPreconditions(FeatureSet target, Type userType, Type preconditionType)
        {
            _target = target;
            _userType = userType;
            _preconditionType = preconditionType;
        }

        public IFeatureBuilderWithUserAndPreconditionsAndTriggers When<TTrigger>() where TTrigger : ITrigger
        {
            return new TargetedFeatureBuilderWithUserAndPreconditionsAndTriggers(_target, _userType, _preconditionType, typeof(TTrigger));
        }


        public IFeatureBuilderWithUserAndPreconditionsAndTriggers After<TFunctionality>() where TFunctionality : IFunctionality
        {
            return When<AfterFunctionalityTrigger<TFunctionality>>();
        }

        public IFeatureBuilderWithUserAndPreconditionsAndTriggers Before<TFunctionality>() where TFunctionality : IFunctionality
        {
            return When<BeforeFunctionalityTrigger<TFunctionality>>();
        }

        public IFeatureBuilderWithUserAndPreconditions And<TPrecondition>() where TPrecondition : IPrecondition
        {
            return new TargetedFeatureBuilderWithUserAndPreconditions(_target, _userType, typeof(AndPrecondition<,>).MakeGenericType(_preconditionType, typeof(TPrecondition)));
        }

        public IFeatureBuilderWithUserAndPreconditions AndNot<TPrecondition>() where TPrecondition : IPrecondition
        {
            return new TargetedFeatureBuilderWithUserAndPreconditions(_target, _userType, typeof(AndPrecondition<,>).MakeGenericType(_preconditionType, typeof(NotPrecondition<>).MakeGenericType(typeof(TPrecondition))));
        }

        public IFeatureBuilderWithUserAndPreconditions Or<TPrecondition>() where TPrecondition : IPrecondition
        {
            return new TargetedFeatureBuilderWithUserAndPreconditions(_target, _userType, typeof(OrPrecondition<,>).MakeGenericType(_preconditionType, typeof(TPrecondition)));
        }

        public IFeatureBuilderWithUserAndPreconditions OrNot<TPrecondition>() where TPrecondition : IPrecondition
        {
            return new TargetedFeatureBuilderWithUserAndPreconditions(_target, _userType, typeof(OrPrecondition<,>).MakeGenericType(_preconditionType, typeof(NotPrecondition<>).MakeGenericType(typeof(TPrecondition))));
        }

        public IFeatureBuilderWithUserAndPreconditions Not()
        {
            return new TargetedFeatureBuilderWithUserAndPreconditions(_target, _userType, typeof(NotPrecondition<>).MakeGenericType(_preconditionType));
        }
    }

    internal class TargetedFeatureBuilderWithUserAndPreconditionsAndTriggers : IFeatureBuilderWithUserAndPreconditionsAndTriggers
    {
        private FeatureSet _target;
        private Type _userType;
        private Type _preconditionType;
        private Type _triggerType;

        public TargetedFeatureBuilderWithUserAndPreconditionsAndTriggers(FeatureSet target, Type userType, Type preconditionType, Type triggerType)
        {
            _target = target;
            _userType = userType;
            _preconditionType = preconditionType;
            _triggerType = triggerType;
        }

        public IFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities Then<TFunctionality>() where TFunctionality : IFunctionality
        {
            return new TargetedFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities(_target, _userType, _preconditionType, _triggerType, typeof(TFunctionality));
        }
    }

    internal class TargetedFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities : IFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities
    {
        private FeatureSet _target;
        private Type _userType;
        private Type _preconditionType;
        private Type _triggerType;
        private Type _functionalityType;

        public TargetedFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities(FeatureSet target, Type userType, Type preconditionType, Type triggerType, Type functionalityType)
        {
            _target = target;
            _userType = userType;
            _preconditionType = preconditionType;
            _triggerType = triggerType;
            _functionalityType = functionalityType;
        }

        public IFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities Then<TFunctionality>() where TFunctionality : IFunctionality
        {
            return new TargetedFeatureBuilderWithUserAndPreconditionsAndTriggersAndFunctionalities(_target, _userType, _preconditionType, _triggerType, typeof(FunctionalityChain<,>).MakeGenericType(_functionalityType, typeof(TFunctionality)));
        }

        public IFeature Build(Func<string> featureName)
        {
            var feature = new Feature(featureName, _userType, _preconditionType, _triggerType, _functionalityType);
            _target.AddFeature(feature);
            return feature;
        }
    }

    internal class Feature : IFeature
    {
        private Func<string> _name;

        private Type _userType;

        private Type _preconditionType;

        private Type _triggerType;

        private Type _functionalityType;

        public Feature(Func<string> name, Type userType, Type preconditionType, Type triggerType, Type functionalityType)
        {
            _name = name;
            _userType = userType;
            _preconditionType = preconditionType;
            _triggerType = triggerType;
            _functionalityType = functionalityType;
        }

        public string Name
        {
            get { return _name(); }
        }

        public Type User
        {
            get { return _userType; }
        }

        public Type Precondition
        {
            get { return _preconditionType; }
        }

        public Type Trigger
        {
            get { return _triggerType; }
        }

        public Type Functionality
        {
            get { return _functionalityType; }
        }
    }


    public class Module : IModule
    {
        public void Load(IServiceRegistry registry)
        {
            registry.Register<IAnyUser, AnyUser>(false);

            registry.Register<IFeatureBuilder, TargetedFeatureBuilder>(false);
            registry.Register<ITargetedFeatureBuilder, TargetedFeatureBuilder>(false);
            registry.Register<IFeatureSet, FeatureSet>(false);

            registry.Register<IUserTextualDescriptor, UserTextualDescriptor>(true);
            registry.Register<IPreconditionTextualDescriptor, PreconditionTextualDescriptor>(true);
            registry.Register<ITriggerTextualDescriptor, TriggerTextualDescriptor>(true);
            registry.Register<IFunctionalityTextualDescriptor, FunctionalityTextualDescriptor>(true);
            registry.Register<IFeatureTextualDescriptor, FeatureTextualDescriptor>(true);
            registry.Register<IFeatureSetTextualDescriptor, FeatureSetTextualDescriptor>(true);
        }
    }
}
