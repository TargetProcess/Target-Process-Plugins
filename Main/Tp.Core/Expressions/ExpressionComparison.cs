using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Tp.Core.Expressions
{
	/// <summary>
	/// Provides a visitor that determines whether two expression trees are equal (by content, not
	/// only by reference) by walking down both trees simultaneously and comparing each node.
	/// </summary>
	public sealed class ExpressionComparison : ExpressionVisitor
	{
		private Expression _comparand;
		private readonly Queue<Expression> _comparands;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExpressionComparison"/> class.
		/// </summary>
		/// <param name="firstExpression">The first expression tree to compare.</param>
		/// <param name="secondExpression">The second expression tree to compare.</param>
		public ExpressionComparison(Expression firstExpression, Expression secondExpression)
		{
			ExpressionsAreEqual = true;
			_comparands = new Queue<Expression>(secondExpression.TraversePreOrder());

			Visit(firstExpression);

			if (0 < _comparands.Count)
				ExpressionsAreEqual = false;
		}

		/// <summary>
		/// Gets a value indicating if the two provided expression trees were found to be equal in
		/// content.
		/// </summary>
		public bool ExpressionsAreEqual
		{ get; private set; }

		/// <summary>
		/// Processes the provided <see cref="Expression"/> object by loading the next node coming
		/// from the expression tree being compared against and then dispatching both to more
		/// specialized visitors for more specific comparison.
		/// </summary>
		/// <param name="node">The expression to process and dispatch.</param>
		/// <returns>
		/// The modified expression, assuming the expression was modified; otherwise, returns the
		/// original expression.
		/// </returns>
		public override Expression Visit(Expression node)
		{
			if (null == node || !ExpressionsAreEqual)
				return node;

			if (0 == _comparands.Count)
				return Fail(node);

			_comparand = _comparands.Peek();

			if (_comparand.NodeType != node.NodeType || _comparand.Type != node.Type)
				return Fail(node);

			_comparands.Dequeue();

			return base.Visit(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitBinary(BinaryExpression node)
		{
			BinaryExpression comparand = (BinaryExpression)_comparand;

			if (!AreEqual(node, comparand, x => x.Method))
				return Fail(node);

			return AreEqual(node, comparand, x => x.IsLifted, x => x.IsLiftedToNull)
			       	? base.VisitBinary(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitConstant(ConstantExpression node)
		{
			ConstantExpression comparand = (ConstantExpression)_comparand;

			return AreEqual(node, comparand, x => x.Value)
			       	? base.VisitConstant(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitDebugInfo(DebugInfoExpression node)
		{
			DebugInfoExpression comparand = (DebugInfoExpression)_comparand;

			if (!AreEqual(node, comparand, x => x.IsClear))
				return Fail(node);

			return AreEqual(node,
			                comparand,
			                x => x.EndColumn,
			                x => x.EndLine,
			                x => x.StartLine,
			                x => x.StartColumn)
			       	? base.VisitDebugInfo(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitDynamic(DynamicExpression node)
		{
			DynamicExpression comparand = (DynamicExpression)_comparand;

			if (!AreEqual(node, comparand, x => x.DelegateType))
				return Fail(node);

			return AreEqual(node, comparand, x => x.Binder)
			       	? base.VisitDynamic(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitGoto(GotoExpression node)
		{
			GotoExpression comparand = (GotoExpression)_comparand;

			if (!AreEqual(node, comparand, x => x.Kind))
				return Fail(node);

			return AreEqual(node.Target, comparand.Target)
			       	? base.VisitGoto(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitIndex(IndexExpression node)
		{
			IndexExpression comparand = (IndexExpression)_comparand;

			return AreEqual(node, comparand, x => x.Indexer)
			       	? base.VisitIndex(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitLabel(LabelExpression node)
		{
			LabelExpression comparand = (LabelExpression)_comparand;

			return AreEqual(comparand.Target, node.Target)
			       	? base.VisitLabel(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			LambdaExpression comparand = (LambdaExpression)_comparand;

			// If lambda expression differs in return type, the expression's type property itself
			// is different. Thus, there is no need to compare return types since all expressions
			// have their types compared before anything else.
			if (!AreEqual(node, comparand, x => x.Name))
				return Fail(node);

			return AreEqual(node, comparand, x => x.TailCall)
			       	? base.VisitLambda(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitListInit(ListInitExpression node)
		{
			ListInitExpression comparand = (ListInitExpression)_comparand;

			return AreEqual(node, comparand, x => x.Initializers, AreEqual)
			       	? base.VisitListInit(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitLoop(LoopExpression node)
		{
			LoopExpression comparand = (LoopExpression)_comparand;

			if (!AreEqual(comparand.BreakLabel, node.BreakLabel))
				return Fail(node);

			return AreEqual(comparand.ContinueLabel, node.ContinueLabel)
			       	? base.VisitLoop(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitMember(MemberExpression node)
		{
			MemberExpression comparand = (MemberExpression)_comparand;

			return AreEqual(node, comparand, x => x.Member)
			       	? base.VisitMember(node)
			       	: Fail(node);
		}

		///<inheritdoc/>
		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			MemberInitExpression comparand = (MemberInitExpression)_comparand;

			return AreEqual(node, comparand, x => x.Bindings, AreEqual)
			       	? base.VisitMemberInit(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			MethodCallExpression comparand = (MethodCallExpression)_comparand;

			return AreEqual(node, comparand, x => x.Method)
			       	? base.VisitMethodCall(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitNew(NewExpression node)
		{
			NewExpression comparand = (NewExpression)_comparand;

			if (!AreEqual(node, comparand, x => x.Constructor))
				return Fail(node);

			return AreEqual(node, comparand, x => x.Members)
			       	? base.VisitNew(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitSwitch(SwitchExpression node)
		{
			SwitchExpression comparand = (SwitchExpression)_comparand;

			return AreEqual(node, comparand, x => x.Comparison)
			       	? base.VisitSwitch(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitTry(TryExpression node)
		{
			TryExpression comparand = (TryExpression)_comparand;

			return AreEqual(node, comparand, x => x.Handlers, AreEqual)
			       	? base.VisitTry(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			TypeBinaryExpression comparand = (TypeBinaryExpression)_comparand;

			return AreEqual(node, comparand, x => x.TypeOperand)
			       	? base.VisitTypeBinary(node)
			       	: Fail(node);
		}

		/// <inheritdoc/>
		protected override Expression VisitUnary(UnaryExpression node)
		{
			UnaryExpression comparand = (UnaryExpression)_comparand;

			if (!AreEqual(node, comparand, x => x.Method))
				return Fail(node);

			return AreEqual(node, comparand, x => x.IsLifted, x => x.IsLiftedToNull)
			       	? base.VisitUnary(node)
			       	: Fail(node);
		}

		/// <summary>
		/// Verifies that the provided <see cref="LabelTarget"/> objects are equal.
		/// </summary>
		/// <param name="first">The first <see cref="LabelTarget"/> object to compare.</param>
		/// <param name="second">The second <see cref="LabelTarget"/> object to compare.</param>
		/// <returns>True if <c>first</c> and <c>second</c> are equal; otherwise, false.</returns>
		private static bool AreEqual(LabelTarget first, LabelTarget second)
		{
			// Label targets are commonly exist individually and many times can left at null
			// without invalidating the expression.
			if (null == first || null == second)
			{
				return first == second;
			}

			return first.Name == second.Name && first.Type == second.Type;
		}

		/// <summary>
		/// Verifies that the provided <see cref="CatchBlock"/> objects are equal.
		/// </summary>
		/// <param name="first">The first <see cref="CatchBlock"/> object to compare.</param>
		/// <param name="second">The second <see cref="CatchBlock"/> object to compare.</param>
		/// <returns>True if <c>first</c> and <c>second</c> are equal; otherwise, false.</returns>
		private static bool AreEqual(CatchBlock first, CatchBlock second)
		{
			return first.Test == second.Test;
		}

		/// <summary>
		/// Verifies that the provided <see cref="ElementInit"/> objects are equal.
		/// </summary>
		/// <param name="first">The first <see cref="ElementInit"/> object to compare.</param>
		/// <param name="second">The second <see cref="ElementInit"/> object to compare.</param>
		/// <returns>True if <c>first</c> and <c>second</c> are equal; otherwise, false.</returns>
		private static bool AreEqual(ElementInit first, ElementInit second)
		{
			return first.AddMethod == second.AddMethod;
		}

		/// <summary>
		/// Verifies that the provided <see cref="MemberBinding"/> objects are equal.
		/// </summary>
		/// <param name="first">The first <see cref="MemberBinding"/> object to compare.</param>
		/// <param name="second">The second <see cref="MemberBinding"/> object to compare.</param>
		/// <returns>True if <c>first</c> and <c>second</c> are equal; otherwise, false.</returns>
		private static bool AreEqual(MemberBinding first, MemberBinding second)
		{
			if (first.BindingType != second.BindingType || first.Member != second.Member)
				return false;

			if (MemberBindingType.ListBinding != first.BindingType)
				return true;

			MemberListBinding firstList = (MemberListBinding)first;
			MemberListBinding secondList = (MemberListBinding)second;

			return AreEqual(firstList, secondList, x => x.Initializers, AreEqual);
		}

		/// <summary>
		/// Verifies property values from two items are equal.
		/// </summary>
		/// <typeparam name="T">
		/// The type of item that the properties being compared belong to.
		/// </typeparam>
		/// <typeparam name="TMember">The type of the properties being compared.</typeparam>
		/// <param name="first">The first item having one of its properties compared.</param>
		/// <param name="second">The second item having one of its properties compared.</param>
		/// <param name="reader">
		/// Method able to extract the property values being compared from the provided items.
		/// </param>
		/// <returns>True if the two item property values are equal; otherwise, false.</returns>
		private static bool AreEqual<T, TMember>(T first, T second, params Func<T, TMember>[] reader)
		{
			return reader.All(r => EqualityComparer<TMember>.Default.Equals(r(first), r(second)));
		}

		/// <summary>
		/// Verifies that specified collections coming from the two provided items are equal in
		/// content using the provided reader to retrieve the collections as well as the
		/// provided comparer to perform the comparison.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the items hosting the collections being compared.
		/// </typeparam>
		/// <typeparam name="TMember">
		/// The type of items that make up the collections being compared.
		/// </typeparam>
		/// <param name="first">The first item that has a collection to compare.</param>
		/// <param name="second">The second item that has a collection to compare.</param>
		/// <param name="reader">
		/// Method able to extract the collections from the provided items.
		/// </param>
		/// <param name="comparer">
		/// The comparer to use to compare the content of the collections.
		/// </param>
		/// <returns>
		/// True if the collections are equal according to <c>comparer</c>; otherwise, false.
		/// </returns>
		private static bool AreEqual<T, TMember>(
			T first,
			T second,
			Func<T, ReadOnlyCollection<TMember>> reader,
			Func<TMember, TMember, bool> comparer = null)
		{
			if (null == comparer)
				comparer = (x, y) => EqualityComparer<TMember>.Default.Equals(x, y);

			ReadOnlyCollection<TMember> firstCollection = reader(first);
			ReadOnlyCollection<TMember> secondCollection = reader(second);

			if (null == firstCollection || null == secondCollection)
			{
				return firstCollection == secondCollection;
			}
			if (firstCollection.Count != secondCollection.Count)
				return false;

			return !firstCollection.Where((t, i) => !comparer(t, secondCollection[i])).Any();
		}

		/// <summary>
		/// Fails the comparison operation, setting the outcome of the comparison to be that the
		/// two expression trees are "not equal", as well as returning an <see cref="Expression"/>
		/// value which, if returned by a visitor operation, will result in all operations coming
		/// to a stop.
		/// </summary>
		/// <param name="node">The current node being visited.</param>
		/// <returns>
		/// An <see cref="Expression"/> value which should be returned by the calling visitor
		/// operation.
		/// </returns>
		private Expression Fail(Expression node)
		{
			ExpressionsAreEqual = false;

			return node;
		}
	}
}