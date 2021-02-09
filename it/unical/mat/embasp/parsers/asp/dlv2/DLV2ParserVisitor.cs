//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from DLV2Parser.g4 by ANTLR 4.7

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="DLV2Parser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
[System.CLSCompliant(false)]
public interface IDLV2ParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="DLV2Parser.answer_set"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAnswer_set([NotNull] DLV2Parser.Answer_setContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="DLV2Parser.cost"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCost([NotNull] DLV2Parser.CostContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="DLV2Parser.level"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLevel([NotNull] DLV2Parser.LevelContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="DLV2Parser.model"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitModel([NotNull] DLV2Parser.ModelContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="DLV2Parser.output"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOutput([NotNull] DLV2Parser.OutputContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="DLV2Parser.predicate_atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPredicate_atom([NotNull] DLV2Parser.Predicate_atomContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="DLV2Parser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTerm([NotNull] DLV2Parser.TermContext context);
}