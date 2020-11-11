//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from ClingoParser.g4 by ANTLR 4.7

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
[System.CLSCompliant(false)]
public partial class ClingoParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		START=1, ANY=2, COMMA=3, INTEGER_CONSTANT=4, NEW_LINE=5, IDENTIFIER=6, 
		STRING_CONSTANT=7, TERMS_BEGIN=8, TERMS_END=9, WHITE_SPACE=10;
	public const int
		RULE_answer_set = 0, RULE_model = 1, RULE_output = 2, RULE_predicate_atom = 3, 
		RULE_term = 4;
	public static readonly string[] ruleNames = {
		"answer_set", "model", "output", "predicate_atom", "term"
	};

	private static readonly string[] _LiteralNames = {
		null, null, null, "','", null, null, null, null, "'('", "')'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "START", "ANY", "COMMA", "INTEGER_CONSTANT", "NEW_LINE", "IDENTIFIER", 
		"STRING_CONSTANT", "TERMS_BEGIN", "TERMS_END", "WHITE_SPACE"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "ClingoParser.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static ClingoParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public ClingoParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public ClingoParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}
	public partial class Answer_setContext : ParserRuleContext {
		public ITerminalNode START() { return GetToken(ClingoParser.START, 0); }
		public ModelContext model() {
			return GetRuleContext<ModelContext>(0);
		}
		public Answer_setContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_answer_set; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IClingoParserVisitor<TResult> typedVisitor = visitor as IClingoParserVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitAnswer_set(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Answer_setContext answer_set() {
		Answer_setContext _localctx = new Answer_setContext(Context, State);
		EnterRule(_localctx, 0, RULE_answer_set);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 10; Match(START);
			State = 11; model();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ModelContext : ParserRuleContext {
		public ITerminalNode NEW_LINE() { return GetToken(ClingoParser.NEW_LINE, 0); }
		public Predicate_atomContext[] predicate_atom() {
			return GetRuleContexts<Predicate_atomContext>();
		}
		public Predicate_atomContext predicate_atom(int i) {
			return GetRuleContext<Predicate_atomContext>(i);
		}
		public ModelContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_model; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IClingoParserVisitor<TResult> typedVisitor = visitor as IClingoParserVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitModel(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ModelContext model() {
		ModelContext _localctx = new ModelContext(Context, State);
		EnterRule(_localctx, 2, RULE_model);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 16;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while (_la==IDENTIFIER) {
				{
				{
				State = 13; predicate_atom();
				}
				}
				State = 18;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			State = 19; Match(NEW_LINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class OutputContext : ParserRuleContext {
		public Answer_setContext[] answer_set() {
			return GetRuleContexts<Answer_setContext>();
		}
		public Answer_setContext answer_set(int i) {
			return GetRuleContext<Answer_setContext>(i);
		}
		public OutputContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_output; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IClingoParserVisitor<TResult> typedVisitor = visitor as IClingoParserVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitOutput(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public OutputContext output() {
		OutputContext _localctx = new OutputContext(Context, State);
		EnterRule(_localctx, 4, RULE_output);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 24;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while (_la==START) {
				{
				{
				State = 21; answer_set();
				}
				}
				State = 26;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Predicate_atomContext : ParserRuleContext {
		public ITerminalNode IDENTIFIER() { return GetToken(ClingoParser.IDENTIFIER, 0); }
		public ITerminalNode TERMS_BEGIN() { return GetToken(ClingoParser.TERMS_BEGIN, 0); }
		public TermContext[] term() {
			return GetRuleContexts<TermContext>();
		}
		public TermContext term(int i) {
			return GetRuleContext<TermContext>(i);
		}
		public ITerminalNode TERMS_END() { return GetToken(ClingoParser.TERMS_END, 0); }
		public ITerminalNode[] COMMA() { return GetTokens(ClingoParser.COMMA); }
		public ITerminalNode COMMA(int i) {
			return GetToken(ClingoParser.COMMA, i);
		}
		public Predicate_atomContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_predicate_atom; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IClingoParserVisitor<TResult> typedVisitor = visitor as IClingoParserVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitPredicate_atom(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Predicate_atomContext predicate_atom() {
		Predicate_atomContext _localctx = new Predicate_atomContext(Context, State);
		EnterRule(_localctx, 6, RULE_predicate_atom);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 27; Match(IDENTIFIER);
			State = 39;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==TERMS_BEGIN) {
				{
				State = 28; Match(TERMS_BEGIN);
				State = 29; term();
				State = 34;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==COMMA) {
					{
					{
					State = 30; Match(COMMA);
					State = 31; term();
					}
					}
					State = 36;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				State = 37; Match(TERMS_END);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class TermContext : ParserRuleContext {
		public ITerminalNode IDENTIFIER() { return GetToken(ClingoParser.IDENTIFIER, 0); }
		public ITerminalNode INTEGER_CONSTANT() { return GetToken(ClingoParser.INTEGER_CONSTANT, 0); }
		public Predicate_atomContext predicate_atom() {
			return GetRuleContext<Predicate_atomContext>(0);
		}
		public ITerminalNode STRING_CONSTANT() { return GetToken(ClingoParser.STRING_CONSTANT, 0); }
		public TermContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_term; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IClingoParserVisitor<TResult> typedVisitor = visitor as IClingoParserVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitTerm(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public TermContext term() {
		TermContext _localctx = new TermContext(Context, State);
		EnterRule(_localctx, 8, RULE_term);
		try {
			State = 45;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,4,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 41; Match(IDENTIFIER);
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 42; Match(INTEGER_CONSTANT);
				}
				break;
			case 3:
				EnterOuterAlt(_localctx, 3);
				{
				State = 43; predicate_atom();
				}
				break;
			case 4:
				EnterOuterAlt(_localctx, 4);
				{
				State = 44; Match(STRING_CONSTANT);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x3', '\f', '\x32', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', 
		'\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x4', 
		'\x6', '\t', '\x6', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', 
		'\a', '\x3', '\x11', '\n', '\x3', '\f', '\x3', '\xE', '\x3', '\x14', '\v', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x4', '\a', '\x4', '\x19', 
		'\n', '\x4', '\f', '\x4', '\xE', '\x4', '\x1C', '\v', '\x4', '\x3', '\x5', 
		'\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\a', '\x5', '#', 
		'\n', '\x5', '\f', '\x5', '\xE', '\x5', '&', '\v', '\x5', '\x3', '\x5', 
		'\x3', '\x5', '\x5', '\x5', '*', '\n', '\x5', '\x3', '\x6', '\x3', '\x6', 
		'\x3', '\x6', '\x3', '\x6', '\x5', '\x6', '\x30', '\n', '\x6', '\x3', 
		'\x6', '\x2', '\x2', '\a', '\x2', '\x4', '\x6', '\b', '\n', '\x2', '\x2', 
		'\x2', '\x33', '\x2', '\f', '\x3', '\x2', '\x2', '\x2', '\x4', '\x12', 
		'\x3', '\x2', '\x2', '\x2', '\x6', '\x1A', '\x3', '\x2', '\x2', '\x2', 
		'\b', '\x1D', '\x3', '\x2', '\x2', '\x2', '\n', '/', '\x3', '\x2', '\x2', 
		'\x2', '\f', '\r', '\a', '\x3', '\x2', '\x2', '\r', '\xE', '\x5', '\x4', 
		'\x3', '\x2', '\xE', '\x3', '\x3', '\x2', '\x2', '\x2', '\xF', '\x11', 
		'\x5', '\b', '\x5', '\x2', '\x10', '\xF', '\x3', '\x2', '\x2', '\x2', 
		'\x11', '\x14', '\x3', '\x2', '\x2', '\x2', '\x12', '\x10', '\x3', '\x2', 
		'\x2', '\x2', '\x12', '\x13', '\x3', '\x2', '\x2', '\x2', '\x13', '\x15', 
		'\x3', '\x2', '\x2', '\x2', '\x14', '\x12', '\x3', '\x2', '\x2', '\x2', 
		'\x15', '\x16', '\a', '\a', '\x2', '\x2', '\x16', '\x5', '\x3', '\x2', 
		'\x2', '\x2', '\x17', '\x19', '\x5', '\x2', '\x2', '\x2', '\x18', '\x17', 
		'\x3', '\x2', '\x2', '\x2', '\x19', '\x1C', '\x3', '\x2', '\x2', '\x2', 
		'\x1A', '\x18', '\x3', '\x2', '\x2', '\x2', '\x1A', '\x1B', '\x3', '\x2', 
		'\x2', '\x2', '\x1B', '\a', '\x3', '\x2', '\x2', '\x2', '\x1C', '\x1A', 
		'\x3', '\x2', '\x2', '\x2', '\x1D', ')', '\a', '\b', '\x2', '\x2', '\x1E', 
		'\x1F', '\a', '\n', '\x2', '\x2', '\x1F', '$', '\x5', '\n', '\x6', '\x2', 
		' ', '!', '\a', '\x5', '\x2', '\x2', '!', '#', '\x5', '\n', '\x6', '\x2', 
		'\"', ' ', '\x3', '\x2', '\x2', '\x2', '#', '&', '\x3', '\x2', '\x2', 
		'\x2', '$', '\"', '\x3', '\x2', '\x2', '\x2', '$', '%', '\x3', '\x2', 
		'\x2', '\x2', '%', '\'', '\x3', '\x2', '\x2', '\x2', '&', '$', '\x3', 
		'\x2', '\x2', '\x2', '\'', '(', '\a', '\v', '\x2', '\x2', '(', '*', '\x3', 
		'\x2', '\x2', '\x2', ')', '\x1E', '\x3', '\x2', '\x2', '\x2', ')', '*', 
		'\x3', '\x2', '\x2', '\x2', '*', '\t', '\x3', '\x2', '\x2', '\x2', '+', 
		'\x30', '\a', '\b', '\x2', '\x2', ',', '\x30', '\a', '\x6', '\x2', '\x2', 
		'-', '\x30', '\x5', '\b', '\x5', '\x2', '.', '\x30', '\a', '\t', '\x2', 
		'\x2', '/', '+', '\x3', '\x2', '\x2', '\x2', '/', ',', '\x3', '\x2', '\x2', 
		'\x2', '/', '-', '\x3', '\x2', '\x2', '\x2', '/', '.', '\x3', '\x2', '\x2', 
		'\x2', '\x30', '\v', '\x3', '\x2', '\x2', '\x2', '\a', '\x12', '\x1A', 
		'$', ')', '/',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
