// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq.Expressions;

namespace Tp.Core.Expressions
{
	internal class ExpressionVisitorEx : ExpressionVisitor
	{
		private readonly bool _processByDefault;

		public ExpressionVisitorEx(bool processByDefault = true)
		{
			_processByDefault = processByDefault;
		}

		private ExpressionEventArg<T> ExpressionEventArg<T>(T node) where T : Expression
		{
			var args = new ExpressionEventArg<T>(node, _processByDefault);
			return args;
		}

		public event EventHandler<ExpressionEventArg<Expression>> OnExpression;

		public override Expression Visit(Expression node)
		{
			var args = ExpressionEventArg(node);
			OnExpression.Raise(this, args);
			return args.Processed ? args.Result : base.Visit(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<LambdaExpression>> OnVisitLambdaExpression;

		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			ExpressionEventArg<LambdaExpression> args = ExpressionEventArg<LambdaExpression>(node);
			OnVisitLambdaExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitLambda((Expression<T>) args.Expression);
		}


		public event EventHandler<ExpressionEventArg<BinaryExpression>> OnBinaryExpression;

		protected override Expression VisitBinary(BinaryExpression node)
		{
			var args = ExpressionEventArg(node);
			OnBinaryExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitBinary(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<BlockExpression>> OnBlockExpression;

		protected override Expression VisitBlock(BlockExpression node)
		{
			var args = ExpressionEventArg(node);
			OnBlockExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitBlock(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<ConditionalExpression>> OnConditionalExpression;

		protected override Expression VisitConditional(ConditionalExpression node)
		{
			var args = ExpressionEventArg(node);
			OnConditionalExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitConditional(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<ConstantExpression>> OnConstantExpression;

		protected override Expression VisitConstant(ConstantExpression node)
		{
			var args = ExpressionEventArg(node);
			OnConstantExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitConstant(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<DebugInfoExpression>> OnDebugInfoExpression;

		protected override Expression VisitDebugInfo(DebugInfoExpression node)
		{
			var args = ExpressionEventArg(node);
			OnDebugInfoExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitDebugInfo(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<DynamicExpression>> OnDynamicExpression;

		protected override Expression VisitDynamic(DynamicExpression node)
		{
			var args = ExpressionEventArg(node);
			OnDynamicExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitDynamic(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<DefaultExpression>> OnDefaultExpression;

		protected override Expression VisitDefault(DefaultExpression node)
		{
			var args = ExpressionEventArg(node);
			OnDefaultExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitDefault(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<Expression>> OnExtensionExpression;

		protected override Expression VisitExtension(Expression node)
		{
			var args = ExpressionEventArg(node);
			OnExtensionExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitExtension(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<GotoExpression>> OnGotoExpression;

		protected override Expression VisitGoto(GotoExpression node)
		{
			var args = ExpressionEventArg(node);
			OnGotoExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitGoto(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<InvocationExpression>> OnInvocationExpression;

		protected override Expression VisitInvocation(InvocationExpression node)
		{
			var args = ExpressionEventArg(node);
			OnInvocationExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitInvocation(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<LabelExpression>> OnLabelExpression;

		protected override Expression VisitLabel(LabelExpression node)
		{
			var args = ExpressionEventArg(node);
			OnLabelExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitLabel(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<LoopExpression>> OnLoopExpression;

		protected override Expression VisitLoop(LoopExpression node)
		{
			var args = ExpressionEventArg(node);
			OnLoopExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitLoop(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<MemberExpression>> OnMemberExpression;

		protected override Expression VisitMember(MemberExpression node)
		{
			var args = ExpressionEventArg(node);
			OnMemberExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitMember(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<IndexExpression>> OnIndexExpression;

		protected override Expression VisitIndex(IndexExpression node)
		{
			var args = ExpressionEventArg(node);
			OnIndexExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitIndex(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<MethodCallExpression>> OnMethodCallExpression;

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			var args = ExpressionEventArg(node);
			OnMethodCallExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitMethodCall(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<NewArrayExpression>> OnNewArrayExpression;

		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			var args = ExpressionEventArg(node);
			OnNewArrayExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitNewArray(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<NewExpression>> OnNewExpression;

		protected override Expression VisitNew(NewExpression node)
		{
			var args = ExpressionEventArg(node);
			OnNewExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitNew(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<ParameterExpression>> OnParameterExpression;

		protected override Expression VisitParameter(ParameterExpression node)
		{
			var args = ExpressionEventArg(node);
			OnParameterExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitParameter(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<RuntimeVariablesExpression>> OnRuntimeVariablesExpression;

		protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			var args = ExpressionEventArg(node);
			OnRuntimeVariablesExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitRuntimeVariables(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<SwitchExpression>> OnSwitchExpression;

		protected override Expression VisitSwitch(SwitchExpression node)
		{
			var args = ExpressionEventArg(node);
			OnSwitchExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitSwitch(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<TryExpression>> OnTryExpression;

		protected override Expression VisitTry(TryExpression node)
		{
			var args = ExpressionEventArg(node);
			OnTryExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitTry(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<TypeBinaryExpression>> OnTypeBinaryExpression;

		protected override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			var args = ExpressionEventArg(node);
			OnTypeBinaryExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitTypeBinary(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<UnaryExpression>> OnUnaryExpression;

		protected override Expression VisitUnary(UnaryExpression node)
		{
			var args = ExpressionEventArg(node);
			OnUnaryExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitUnary(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<MemberInitExpression>> OnMemberInitExpression;

		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			var args = ExpressionEventArg(node);
			OnMemberInitExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitMemberInit(args.Expression);
		}

		public event EventHandler<ExpressionEventArg<ListInitExpression>> OnListInitExpression;

		protected override Expression VisitListInit(ListInitExpression node)
		{
			var args = ExpressionEventArg(node);
			OnListInitExpression.Raise(this, args);
			return args.Processed ? args.Result : base.VisitListInit(args.Expression);
		}
	}
}