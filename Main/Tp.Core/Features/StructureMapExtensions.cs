using System;
using StructureMap;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Pipeline;

namespace Tp.Core.Features
{
    public static class StructureMapExtensions
    {
        public static FeatureTogglingContext<TService> IfFeatureEnabled<TService>(this CreatePluginFamilyExpression<TService> expr,
            Predicate<ITpFeatureList> condition)
        {
            return new FeatureTogglingContext<TService>(condition, expr);
        }

        public static FeatureTogglingContext<TService> IfFeatureEnabled<TService>(this CreatePluginFamilyExpression<TService> expr,
            TpFeature feature)
        {
            return new FeatureTogglingContext<TService>(features => features.IsEnabled(feature), expr);
        }
    }

    public class FeatureTogglingContext<TService>
    {
        private readonly Predicate<ITpFeatureList> _condition;

        public FeatureTogglingContext(Predicate<ITpFeatureList> condition, CreatePluginFamilyExpression<TService> expression)
        {
            _condition = condition;
            Expression = expression;
        }

        public ElseFeatureTogglingContext<TService> Use<TServiceImpl>() where TServiceImpl : TService
        {
            IfFeatureToggled = condition => condition.If(context =>
            {
                var features = context.GetInstance<ITpFeatureList>();
                return _condition(features);
            }).ThenIt.Is.Type<TServiceImpl>();
            return new ElseFeatureTogglingContext<TService>(this);
        }

        public ElseFeatureTogglingContext<TService> Use<TServiceImpl>(Func<TServiceImpl> creator) where TServiceImpl : TService
        {
            IfFeatureToggled = condition => condition.If(context =>
            {
                var features = context.GetInstance<ITpFeatureList>();
                return _condition(features);
            }).ThenIt.Is.ConstructedBy(() => creator());
            return new ElseFeatureTogglingContext<TService>(this);
        }

        public ElseFeatureTogglingContext<TService> Use<TServiceImpl>(Func<IContext, TServiceImpl> creator) where TServiceImpl : TService
        {
            IfFeatureToggled = condition => condition.If(context =>
            {
                var features = context.GetInstance<ITpFeatureList>();
                return _condition(features);
            }).ThenIt.Is.ConstructedBy(ctx => creator(ctx));
            return new ElseFeatureTogglingContext<TService>(this);
        }

        private Action<ConditionalInstance<TService>.ConditionalInstanceExpression> IfFeatureToggled { get; set; }
        internal Action<ConditionalInstance<TService>.ConditionalInstanceExpression> Default { get; set; }

        internal CreatePluginFamilyExpression<TService> Expression { get; }

        internal void Action(ConditionalInstance<TService>.ConditionalInstanceExpression condition)
        {
            IfFeatureToggled(condition);
            Default?.Invoke(condition);
        }
    }

    public class ElseFeatureTogglingContext<TService>
    {
        private readonly FeatureTogglingContext<TService> _context;

        internal ElseFeatureTogglingContext(FeatureTogglingContext<TService> context)
        {
            _context = context;
        }

        public void ElseUse<TServiceImpl>() where TServiceImpl : TService
        {
            _context.Default = condition => condition.TheDefault.Is.Type<TServiceImpl>();
            _context.Expression.ConditionallyUse(_context.Action);
        }

        public void ElseUse<TServiceImpl>(Func<TServiceImpl> creator) where TServiceImpl : TService
        {
            _context.Default = condition => condition.TheDefault.Is.ConstructedBy(() => creator());
            _context.Expression.ConditionallyUse(_context.Action);
        }

        public void ElseUse<TServiceImpl>(Func<IContext, TServiceImpl> creator) where TServiceImpl : TService
        {
            _context.Default = condition => condition.TheDefault.Is.ConstructedBy(ctx => creator(ctx));
            _context.Expression.ConditionallyUse(_context.Action);
        }
    }
}
