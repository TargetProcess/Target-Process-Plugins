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
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="ExpressionParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public interface IExpressionParserListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProgram([NotNull] ExpressionParser.ProgramContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProgram([NotNull] ExpressionParser.ProgramContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>call</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCall([NotNull] ExpressionParser.CallContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>call</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCall([NotNull] ExpressionParser.CallContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>cast</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCast([NotNull] ExpressionParser.CastContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>cast</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCast([NotNull] ExpressionParser.CastContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>constant</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstant([NotNull] ExpressionParser.ConstantContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>constant</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstant([NotNull] ExpressionParser.ConstantContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>conditional</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditional([NotNull] ExpressionParser.ConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>conditional</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditional([NotNull] ExpressionParser.ConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>fieldAccess</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFieldAccess([NotNull] ExpressionParser.FieldAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>fieldAccess</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFieldAccess([NotNull] ExpressionParser.FieldAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>binary</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBinary([NotNull] ExpressionParser.BinaryContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>binary</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBinary([NotNull] ExpressionParser.BinaryContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>relational</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRelational([NotNull] ExpressionParser.RelationalContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>relational</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRelational([NotNull] ExpressionParser.RelationalContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>unary</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnary([NotNull] ExpressionParser.UnaryContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>unary</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnary([NotNull] ExpressionParser.UnaryContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>indexer</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIndexer([NotNull] ExpressionParser.IndexerContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>indexer</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIndexer([NotNull] ExpressionParser.IndexerContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>parenthesis</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParenthesis([NotNull] ExpressionParser.ParenthesisContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>parenthesis</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParenthesis([NotNull] ExpressionParser.ParenthesisContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>logical</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogical([NotNull] ExpressionParser.LogicalContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>logical</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogical([NotNull] ExpressionParser.LogicalContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>object</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterObject([NotNull] ExpressionParser.ObjectContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>object</c>
	/// labeled alternative in <see cref="ExpressionParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitObject([NotNull] ExpressionParser.ObjectContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.objectExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterObjectExpr([NotNull] ExpressionParser.ObjectExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.objectExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitObjectExpr([NotNull] ExpressionParser.ObjectExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.objectPropertyExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterObjectPropertyExpr([NotNull] ExpressionParser.ObjectPropertyExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.objectPropertyExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitObjectPropertyExpr([NotNull] ExpressionParser.ObjectPropertyExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.literalExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteralExpr([NotNull] ExpressionParser.LiteralExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.literalExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteralExpr([NotNull] ExpressionParser.LiteralExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.arguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArguments([NotNull] ExpressionParser.ArgumentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.arguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArguments([NotNull] ExpressionParser.ArgumentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.fieldNameExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFieldNameExpr([NotNull] ExpressionParser.FieldNameExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.fieldNameExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFieldNameExpr([NotNull] ExpressionParser.FieldNameExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.expressionContainer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpressionContainer([NotNull] ExpressionParser.ExpressionContainerContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.expressionContainer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpressionContainer([NotNull] ExpressionParser.ExpressionContainerContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.orderingProgram"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOrderingProgram([NotNull] ExpressionParser.OrderingProgramContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.orderingProgram"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOrderingProgram([NotNull] ExpressionParser.OrderingProgramContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExpressionParser.ordering"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOrdering([NotNull] ExpressionParser.OrderingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExpressionParser.ordering"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOrdering([NotNull] ExpressionParser.OrderingContext context);
}
} // namespace Tp.Core.Expressions.Parsing.Antlr
