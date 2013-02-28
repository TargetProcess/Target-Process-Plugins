// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Pipeline;

namespace Tp.Core.Features
{
	public static class StructureMapExtensions
	{
		public static FeatureTogglingContext<TService> IfFeatureEnabled<TService>(this CreatePluginFamilyExpression<TService> expr, Predicate<ITpFeatureList> condition)
		{
			return new FeatureTogglingContext<TService>(condition, expr);
		}

		public static FeatureTogglingContext<TService> IfFeatureEnabled<TService>(this CreatePluginFamilyExpression<TService> expr, TpFeature feature)
		{
			return new FeatureTogglingContext<TService>(features => features.IsEnabled(feature), expr);
		}
	}

	public class FeatureTogglingContext<TService>
	{
		private readonly Predicate<ITpFeatureList> _condition;
		private readonly CreatePluginFamilyExpression<TService> _expression;
		public FeatureTogglingContext(Predicate<ITpFeatureList> condition, CreatePluginFamilyExpression<TService> expression)
		{
			_condition = condition;
			_expression = expression;
		}

		public ElseFeatureTogglingContext<TService> Use<TServiceImpl>()  where TServiceImpl : TService
		{
			IfFeatureToggled = condition => condition.If(context =>
			                                             	{
			                                             		var features = context.GetInstance<ITpFeatureList>();
																return _condition(features);
			                                             	}).ThenIt.Is.Type<TServiceImpl>();
			return new ElseFeatureTogglingContext<TService>(this);
		}

		private Action<ConditionalInstance<TService>.ConditionalInstanceExpression> IfFeatureToggled { get; set; }
		internal Action<ConditionalInstance<TService>.ConditionalInstanceExpression> Default { get; set; }

		internal CreatePluginFamilyExpression<TService> Expression
		{
			get { return _expression; }
		}

		internal void Action(ConditionalInstance<TService>.ConditionalInstanceExpression condition)
		{
			IfFeatureToggled(condition);
			if (Default != null)
			{
				Default(condition);
			}
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
			Action<ConditionalInstance<TService>.ConditionalInstanceExpression> action = _context.Action;
			_context.Expression.ConditionallyUse(action);
		}
	}
}