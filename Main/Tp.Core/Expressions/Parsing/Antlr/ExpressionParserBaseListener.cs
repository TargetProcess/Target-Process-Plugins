//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from .\ExpressionParser.g4 by ANTLR 4.8

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Tp.Core.Expressions.Parsing.Antlr {

using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IExpressionParserListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public partial class ExpressionParserBaseListener : IExpressionParserListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.program"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterProgram([NotNull] ExpressionParser.ProgramContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.program"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitProgram([NotNull] ExpressionParser.ProgramContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>call</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCall([NotNull] ExpressionParser.CallContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>call</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCall([NotNull] ExpressionParser.CallContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>cast</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCast([NotNull] ExpressionParser.CastContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>cast</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCast([NotNull] ExpressionParser.CastContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>constant</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConstant([NotNull] ExpressionParser.ConstantContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>constant</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConstant([NotNull] ExpressionParser.ConstantContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>conditional</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConditional([NotNull] ExpressionParser.ConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>conditional</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConditional([NotNull] ExpressionParser.ConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>fieldAccess</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFieldAccess([NotNull] ExpressionParser.FieldAccessContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>fieldAccess</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFieldAccess([NotNull] ExpressionParser.FieldAccessContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>binary</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBinary([NotNull] ExpressionParser.BinaryContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>binary</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBinary([NotNull] ExpressionParser.BinaryContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>relational</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRelational([NotNull] ExpressionParser.RelationalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>relational</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRelational([NotNull] ExpressionParser.RelationalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>unary</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnary([NotNull] ExpressionParser.UnaryContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>unary</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnary([NotNull] ExpressionParser.UnaryContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>indexer</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIndexer([NotNull] ExpressionParser.IndexerContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>indexer</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIndexer([NotNull] ExpressionParser.IndexerContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>parenthesis</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParenthesis([NotNull] ExpressionParser.ParenthesisContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>parenthesis</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParenthesis([NotNull] ExpressionParser.ParenthesisContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>logical</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLogical([NotNull] ExpressionParser.LogicalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>logical</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLogical([NotNull] ExpressionParser.LogicalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>object</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterObject([NotNull] ExpressionParser.ObjectContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>object</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitObject([NotNull] ExpressionParser.ObjectContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.objectExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterObjectExpr([NotNull] ExpressionParser.ObjectExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.objectExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitObjectExpr([NotNull] ExpressionParser.ObjectExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.objectPropertyExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterObjectPropertyExpr([NotNull] ExpressionParser.ObjectPropertyExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.objectPropertyExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitObjectPropertyExpr([NotNull] ExpressionParser.ObjectPropertyExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.literalExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLiteralExpr([NotNull] ExpressionParser.LiteralExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.literalExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLiteralExpr([NotNull] ExpressionParser.LiteralExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArguments([NotNull] ExpressionParser.ArgumentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArguments([NotNull] ExpressionParser.ArgumentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.fieldNameExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFieldNameExpr([NotNull] ExpressionParser.FieldNameExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.fieldNameExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFieldNameExpr([NotNull] ExpressionParser.FieldNameExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.expressionContainer"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpressionContainer([NotNull] ExpressionParser.ExpressionContainerContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.expressionContainer"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpressionContainer([NotNull] ExpressionParser.ExpressionContainerContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.orderingProgram"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOrderingProgram([NotNull] ExpressionParser.OrderingProgramContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.orderingProgram"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOrderingProgram([NotNull] ExpressionParser.OrderingProgramContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.ordering"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOrdering([NotNull] ExpressionParser.OrderingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.ordering"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOrdering([NotNull] ExpressionParser.OrderingContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
} // namespace Tp.Core.Expressions.Parsing.Antlr
