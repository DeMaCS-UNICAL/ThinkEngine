//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from DLVLexer.g4 by ANTLR 4.7

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
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
[System.CLSCompliant(false)]
public partial class DLVLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		HEADER=1, COLON=2, COST_BEGIN=3, COST_END=4, OPEN_SQUARE_BRACKET=5, CLOSE_SQUARE_BRACKET=6, 
		GROUND_QUERY_BEGIN=7, MODEL_BEGIN=8, MODEL_END=9, WEIGHTED_MODEL_LABEL=10, 
		COMMA=11, IDENTIFIER=12, INTEGER_CONSTANT=13, STRING_CONSTANT=14, TERMS_BEGIN=15, 
		TERMS_END=16, WHITESPACE=17, REASONING=18, DOT=19, BOOLEAN=20, WHITESPACE_IN_GROUND_QUERY=21, 
		WITNESS_LABEL=22;
	public const int
		IN_GROUND_QUERY=1;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE", "IN_GROUND_QUERY"
	};

	public static readonly string[] ruleNames = {
		"HEADER", "COLON", "COST_BEGIN", "COST_END", "OPEN_SQUARE_BRACKET", "CLOSE_SQUARE_BRACKET", 
		"GROUND_QUERY_BEGIN", "MODEL_BEGIN", "MODEL_END", "WEIGHTED_MODEL_LABEL", 
		"COMMA", "IDENTIFIER", "INTEGER_CONSTANT", "STRING_CONSTANT", "TERMS_BEGIN", 
		"TERMS_END", "WHITESPACE", "REASONING", "DOT", "BOOLEAN", "WHITESPACE_IN_GROUND_QUERY", 
		"WITNESS_LABEL", "WS"
	};


	public DLVLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public DLVLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, null, "':'", "'Cost ([Weight:Level]): <'", "'>'", "'['", "']'", 
		"' is '", "'{'", "'}'", "'Best model:'", "','", null, null, null, "'('", 
		"')'", null, null, "'.'", null, null, "', evidenced by'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "HEADER", "COLON", "COST_BEGIN", "COST_END", "OPEN_SQUARE_BRACKET", 
		"CLOSE_SQUARE_BRACKET", "GROUND_QUERY_BEGIN", "MODEL_BEGIN", "MODEL_END", 
		"WEIGHTED_MODEL_LABEL", "COMMA", "IDENTIFIER", "INTEGER_CONSTANT", "STRING_CONSTANT", 
		"TERMS_BEGIN", "TERMS_END", "WHITESPACE", "REASONING", "DOT", "BOOLEAN", 
		"WHITESPACE_IN_GROUND_QUERY", "WITNESS_LABEL"
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

	public override string GrammarFileName { get { return "DLVLexer.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static DLVLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x18', '\xD7', '\b', '\x1', '\b', '\x1', '\x4', '\x2', 
		'\t', '\x2', '\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', 
		'\x5', '\t', '\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', 
		'\x4', '\b', '\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', 
		'\x4', '\v', '\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', 
		'\x4', '\xE', '\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', '\x10', '\t', 
		'\x10', '\x4', '\x11', '\t', '\x11', '\x4', '\x12', '\t', '\x12', '\x4', 
		'\x13', '\t', '\x13', '\x4', '\x14', '\t', '\x14', '\x4', '\x15', '\t', 
		'\x15', '\x4', '\x16', '\t', '\x16', '\x4', '\x17', '\t', '\x17', '\x4', 
		'\x18', '\t', '\x18', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\a', '\x2', ':', '\n', 
		'\x2', '\f', '\x2', '\xE', '\x2', '=', '\v', '\x2', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x5', '\x3', '\x5', '\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3', '\a', 
		'\x3', '\b', '\x3', '\b', '\x3', '\b', '\x3', '\b', '\x3', '\b', '\x3', 
		'\b', '\x3', '\b', '\x3', '\t', '\x3', '\t', '\x3', '\n', '\x3', '\n', 
		'\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', 
		'\v', '\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', '\v', 
		'\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', '\r', '\x3', '\r', '\a', 
		'\r', '\x7F', '\n', '\r', '\f', '\r', '\xE', '\r', '\x82', '\v', '\r', 
		'\x3', '\xE', '\x3', '\xE', '\x3', '\xE', '\a', '\xE', '\x87', '\n', '\xE', 
		'\f', '\xE', '\xE', '\xE', '\x8A', '\v', '\xE', '\x5', '\xE', '\x8C', 
		'\n', '\xE', '\x3', '\xF', '\x3', '\xF', '\a', '\xF', '\x90', '\n', '\xF', 
		'\f', '\xF', '\xE', '\xF', '\x93', '\v', '\xF', '\x3', '\xF', '\x3', '\xF', 
		'\x3', '\x10', '\x3', '\x10', '\x3', '\x11', '\x3', '\x11', '\x3', '\x12', 
		'\x3', '\x12', '\x3', '\x12', '\x3', '\x12', '\x3', '\x13', '\x3', '\x13', 
		'\x3', '\x13', '\x3', '\x13', '\x3', '\x13', '\x3', '\x13', '\x3', '\x13', 
		'\x3', '\x13', '\x3', '\x13', '\x3', '\x13', '\x3', '\x13', '\x3', '\x13', 
		'\x3', '\x13', '\x3', '\x13', '\x3', '\x13', '\x3', '\x13', '\x3', '\x13', 
		'\x5', '\x13', '\xB0', '\n', '\x13', '\x3', '\x14', '\x3', '\x14', '\x3', 
		'\x14', '\x3', '\x14', '\x3', '\x15', '\x3', '\x15', '\x3', '\x15', '\x3', 
		'\x15', '\x3', '\x15', '\x3', '\x15', '\x3', '\x15', '\x3', '\x15', '\x3', 
		'\x15', '\x5', '\x15', '\xBF', '\n', '\x15', '\x3', '\x16', '\x3', '\x16', 
		'\x3', '\x16', '\x3', '\x16', '\x3', '\x17', '\x3', '\x17', '\x3', '\x17', 
		'\x3', '\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x17', 
		'\x3', '\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x17', 
		'\x3', '\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x18', 
		'\x3', '\x18', '\x3', ';', '\x2', '\x19', '\x4', '\x3', '\x6', '\x4', 
		'\b', '\x5', '\n', '\x6', '\f', '\a', '\xE', '\b', '\x10', '\t', '\x12', 
		'\n', '\x14', '\v', '\x16', '\f', '\x18', '\r', '\x1A', '\xE', '\x1C', 
		'\xF', '\x1E', '\x10', ' ', '\x11', '\"', '\x12', '$', '\x13', '&', '\x14', 
		'(', '\x15', '*', '\x16', ',', '\x17', '.', '\x18', '\x30', '\x2', '\x4', 
		'\x2', '\x3', '\b', '\x4', '\x2', '\x43', '\\', '\x63', '|', '\x6', '\x2', 
		'\x32', ';', '\x43', '\\', '\x61', '\x61', '\x63', '|', '\x3', '\x2', 
		'\x33', ';', '\x3', '\x2', '\x32', ';', '\x3', '\x2', '$', '$', '\x5', 
		'\x2', '\v', '\f', '\xF', '\xF', '\"', '\"', '\x2', '\xDB', '\x2', '\x4', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x6', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\b', '\x3', '\x2', '\x2', '\x2', '\x2', '\n', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\f', '\x3', '\x2', '\x2', '\x2', '\x2', '\xE', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x10', '\x3', '\x2', '\x2', '\x2', '\x2', '\x12', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x14', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x16', '\x3', '\x2', '\x2', '\x2', '\x2', '\x18', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x1A', '\x3', '\x2', '\x2', '\x2', '\x2', '\x1C', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x1E', '\x3', '\x2', '\x2', '\x2', 
		'\x2', ' ', '\x3', '\x2', '\x2', '\x2', '\x2', '\"', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '$', '\x3', '\x2', '\x2', '\x2', '\x3', '&', '\x3', '\x2', 
		'\x2', '\x2', '\x3', '(', '\x3', '\x2', '\x2', '\x2', '\x3', '*', '\x3', 
		'\x2', '\x2', '\x2', '\x3', ',', '\x3', '\x2', '\x2', '\x2', '\x3', '.', 
		'\x3', '\x2', '\x2', '\x2', '\x4', '\x32', '\x3', '\x2', '\x2', '\x2', 
		'\x6', '\x42', '\x3', '\x2', '\x2', '\x2', '\b', '\x44', '\x3', '\x2', 
		'\x2', '\x2', '\n', ']', '\x3', '\x2', '\x2', '\x2', '\f', '_', '\x3', 
		'\x2', '\x2', '\x2', '\xE', '\x61', '\x3', '\x2', '\x2', '\x2', '\x10', 
		'\x63', '\x3', '\x2', '\x2', '\x2', '\x12', 'j', '\x3', '\x2', '\x2', 
		'\x2', '\x14', 'l', '\x3', '\x2', '\x2', '\x2', '\x16', 'n', '\x3', '\x2', 
		'\x2', '\x2', '\x18', 'z', '\x3', '\x2', '\x2', '\x2', '\x1A', '|', '\x3', 
		'\x2', '\x2', '\x2', '\x1C', '\x8B', '\x3', '\x2', '\x2', '\x2', '\x1E', 
		'\x8D', '\x3', '\x2', '\x2', '\x2', ' ', '\x96', '\x3', '\x2', '\x2', 
		'\x2', '\"', '\x98', '\x3', '\x2', '\x2', '\x2', '$', '\x9A', '\x3', '\x2', 
		'\x2', '\x2', '&', '\xAF', '\x3', '\x2', '\x2', '\x2', '(', '\xB1', '\x3', 
		'\x2', '\x2', '\x2', '*', '\xBE', '\x3', '\x2', '\x2', '\x2', ',', '\xC0', 
		'\x3', '\x2', '\x2', '\x2', '.', '\xC4', '\x3', '\x2', '\x2', '\x2', '\x30', 
		'\xD5', '\x3', '\x2', '\x2', '\x2', '\x32', '\x33', '\a', '\x46', '\x2', 
		'\x2', '\x33', '\x34', '\a', 'N', '\x2', '\x2', '\x34', '\x35', '\a', 
		'X', '\x2', '\x2', '\x35', '\x36', '\a', '\"', '\x2', '\x2', '\x36', '\x37', 
		'\a', ']', '\x2', '\x2', '\x37', ';', '\x3', '\x2', '\x2', '\x2', '\x38', 
		':', '\v', '\x2', '\x2', '\x2', '\x39', '\x38', '\x3', '\x2', '\x2', '\x2', 
		':', '=', '\x3', '\x2', '\x2', '\x2', ';', '<', '\x3', '\x2', '\x2', '\x2', 
		';', '\x39', '\x3', '\x2', '\x2', '\x2', '<', '>', '\x3', '\x2', '\x2', 
		'\x2', '=', ';', '\x3', '\x2', '\x2', '\x2', '>', '?', '\a', '_', '\x2', 
		'\x2', '?', '@', '\x3', '\x2', '\x2', '\x2', '@', '\x41', '\b', '\x2', 
		'\x2', '\x2', '\x41', '\x5', '\x3', '\x2', '\x2', '\x2', '\x42', '\x43', 
		'\a', '<', '\x2', '\x2', '\x43', '\a', '\x3', '\x2', '\x2', '\x2', '\x44', 
		'\x45', '\a', '\x45', '\x2', '\x2', '\x45', '\x46', '\a', 'q', '\x2', 
		'\x2', '\x46', 'G', '\a', 'u', '\x2', '\x2', 'G', 'H', '\a', 'v', '\x2', 
		'\x2', 'H', 'I', '\a', '\"', '\x2', '\x2', 'I', 'J', '\a', '*', '\x2', 
		'\x2', 'J', 'K', '\a', ']', '\x2', '\x2', 'K', 'L', '\a', 'Y', '\x2', 
		'\x2', 'L', 'M', '\a', 'g', '\x2', '\x2', 'M', 'N', '\a', 'k', '\x2', 
		'\x2', 'N', 'O', '\a', 'i', '\x2', '\x2', 'O', 'P', '\a', 'j', '\x2', 
		'\x2', 'P', 'Q', '\a', 'v', '\x2', '\x2', 'Q', 'R', '\a', '<', '\x2', 
		'\x2', 'R', 'S', '\a', 'N', '\x2', '\x2', 'S', 'T', '\a', 'g', '\x2', 
		'\x2', 'T', 'U', '\a', 'x', '\x2', '\x2', 'U', 'V', '\a', 'g', '\x2', 
		'\x2', 'V', 'W', '\a', 'n', '\x2', '\x2', 'W', 'X', '\a', '_', '\x2', 
		'\x2', 'X', 'Y', '\a', '+', '\x2', '\x2', 'Y', 'Z', '\a', '<', '\x2', 
		'\x2', 'Z', '[', '\a', '\"', '\x2', '\x2', '[', '\\', '\a', '>', '\x2', 
		'\x2', '\\', '\t', '\x3', '\x2', '\x2', '\x2', ']', '^', '\a', '@', '\x2', 
		'\x2', '^', '\v', '\x3', '\x2', '\x2', '\x2', '_', '`', '\a', ']', '\x2', 
		'\x2', '`', '\r', '\x3', '\x2', '\x2', '\x2', '\x61', '\x62', '\a', '_', 
		'\x2', '\x2', '\x62', '\xF', '\x3', '\x2', '\x2', '\x2', '\x63', '\x64', 
		'\a', '\"', '\x2', '\x2', '\x64', '\x65', '\a', 'k', '\x2', '\x2', '\x65', 
		'\x66', '\a', 'u', '\x2', '\x2', '\x66', 'g', '\a', '\"', '\x2', '\x2', 
		'g', 'h', '\x3', '\x2', '\x2', '\x2', 'h', 'i', '\b', '\b', '\x3', '\x2', 
		'i', '\x11', '\x3', '\x2', '\x2', '\x2', 'j', 'k', '\a', '}', '\x2', '\x2', 
		'k', '\x13', '\x3', '\x2', '\x2', '\x2', 'l', 'm', '\a', '\x7F', '\x2', 
		'\x2', 'm', '\x15', '\x3', '\x2', '\x2', '\x2', 'n', 'o', '\a', '\x44', 
		'\x2', '\x2', 'o', 'p', '\a', 'g', '\x2', '\x2', 'p', 'q', '\a', 'u', 
		'\x2', '\x2', 'q', 'r', '\a', 'v', '\x2', '\x2', 'r', 's', '\a', '\"', 
		'\x2', '\x2', 's', 't', '\a', 'o', '\x2', '\x2', 't', 'u', '\a', 'q', 
		'\x2', '\x2', 'u', 'v', '\a', '\x66', '\x2', '\x2', 'v', 'w', '\a', 'g', 
		'\x2', '\x2', 'w', 'x', '\a', 'n', '\x2', '\x2', 'x', 'y', '\a', '<', 
		'\x2', '\x2', 'y', '\x17', '\x3', '\x2', '\x2', '\x2', 'z', '{', '\a', 
		'.', '\x2', '\x2', '{', '\x19', '\x3', '\x2', '\x2', '\x2', '|', '\x80', 
		'\t', '\x2', '\x2', '\x2', '}', '\x7F', '\t', '\x3', '\x2', '\x2', '~', 
		'}', '\x3', '\x2', '\x2', '\x2', '\x7F', '\x82', '\x3', '\x2', '\x2', 
		'\x2', '\x80', '~', '\x3', '\x2', '\x2', '\x2', '\x80', '\x81', '\x3', 
		'\x2', '\x2', '\x2', '\x81', '\x1B', '\x3', '\x2', '\x2', '\x2', '\x82', 
		'\x80', '\x3', '\x2', '\x2', '\x2', '\x83', '\x8C', '\a', '\x32', '\x2', 
		'\x2', '\x84', '\x88', '\t', '\x4', '\x2', '\x2', '\x85', '\x87', '\t', 
		'\x5', '\x2', '\x2', '\x86', '\x85', '\x3', '\x2', '\x2', '\x2', '\x87', 
		'\x8A', '\x3', '\x2', '\x2', '\x2', '\x88', '\x86', '\x3', '\x2', '\x2', 
		'\x2', '\x88', '\x89', '\x3', '\x2', '\x2', '\x2', '\x89', '\x8C', '\x3', 
		'\x2', '\x2', '\x2', '\x8A', '\x88', '\x3', '\x2', '\x2', '\x2', '\x8B', 
		'\x83', '\x3', '\x2', '\x2', '\x2', '\x8B', '\x84', '\x3', '\x2', '\x2', 
		'\x2', '\x8C', '\x1D', '\x3', '\x2', '\x2', '\x2', '\x8D', '\x91', '\a', 
		'$', '\x2', '\x2', '\x8E', '\x90', '\n', '\x6', '\x2', '\x2', '\x8F', 
		'\x8E', '\x3', '\x2', '\x2', '\x2', '\x90', '\x93', '\x3', '\x2', '\x2', 
		'\x2', '\x91', '\x8F', '\x3', '\x2', '\x2', '\x2', '\x91', '\x92', '\x3', 
		'\x2', '\x2', '\x2', '\x92', '\x94', '\x3', '\x2', '\x2', '\x2', '\x93', 
		'\x91', '\x3', '\x2', '\x2', '\x2', '\x94', '\x95', '\a', '$', '\x2', 
		'\x2', '\x95', '\x1F', '\x3', '\x2', '\x2', '\x2', '\x96', '\x97', '\a', 
		'*', '\x2', '\x2', '\x97', '!', '\x3', '\x2', '\x2', '\x2', '\x98', '\x99', 
		'\a', '+', '\x2', '\x2', '\x99', '#', '\x3', '\x2', '\x2', '\x2', '\x9A', 
		'\x9B', '\x5', '\x30', '\x18', '\x2', '\x9B', '\x9C', '\x3', '\x2', '\x2', 
		'\x2', '\x9C', '\x9D', '\b', '\x12', '\x2', '\x2', '\x9D', '%', '\x3', 
		'\x2', '\x2', '\x2', '\x9E', '\x9F', '\a', '\x64', '\x2', '\x2', '\x9F', 
		'\xA0', '\a', 't', '\x2', '\x2', '\xA0', '\xA1', '\a', '\x63', '\x2', 
		'\x2', '\xA1', '\xA2', '\a', 'x', '\x2', '\x2', '\xA2', '\xA3', '\a', 
		'g', '\x2', '\x2', '\xA3', '\xA4', '\a', 'n', '\x2', '\x2', '\xA4', '\xB0', 
		'\a', '{', '\x2', '\x2', '\xA5', '\xA6', '\a', '\x65', '\x2', '\x2', '\xA6', 
		'\xA7', '\a', '\x63', '\x2', '\x2', '\xA7', '\xA8', '\a', 'w', '\x2', 
		'\x2', '\xA8', '\xA9', '\a', 'v', '\x2', '\x2', '\xA9', '\xAA', '\a', 
		'k', '\x2', '\x2', '\xAA', '\xAB', '\a', 'q', '\x2', '\x2', '\xAB', '\xAC', 
		'\a', 'w', '\x2', '\x2', '\xAC', '\xAD', '\a', 'u', '\x2', '\x2', '\xAD', 
		'\xAE', '\a', 'n', '\x2', '\x2', '\xAE', '\xB0', '\a', '{', '\x2', '\x2', 
		'\xAF', '\x9E', '\x3', '\x2', '\x2', '\x2', '\xAF', '\xA5', '\x3', '\x2', 
		'\x2', '\x2', '\xB0', '\'', '\x3', '\x2', '\x2', '\x2', '\xB1', '\xB2', 
		'\a', '\x30', '\x2', '\x2', '\xB2', '\xB3', '\x3', '\x2', '\x2', '\x2', 
		'\xB3', '\xB4', '\b', '\x14', '\x4', '\x2', '\xB4', ')', '\x3', '\x2', 
		'\x2', '\x2', '\xB5', '\xB6', '\a', 'h', '\x2', '\x2', '\xB6', '\xB7', 
		'\a', '\x63', '\x2', '\x2', '\xB7', '\xB8', '\a', 'n', '\x2', '\x2', '\xB8', 
		'\xB9', '\a', 'u', '\x2', '\x2', '\xB9', '\xBF', '\a', 'g', '\x2', '\x2', 
		'\xBA', '\xBB', '\a', 'v', '\x2', '\x2', '\xBB', '\xBC', '\a', 't', '\x2', 
		'\x2', '\xBC', '\xBD', '\a', 'w', '\x2', '\x2', '\xBD', '\xBF', '\a', 
		'g', '\x2', '\x2', '\xBE', '\xB5', '\x3', '\x2', '\x2', '\x2', '\xBE', 
		'\xBA', '\x3', '\x2', '\x2', '\x2', '\xBF', '+', '\x3', '\x2', '\x2', 
		'\x2', '\xC0', '\xC1', '\x5', '\x30', '\x18', '\x2', '\xC1', '\xC2', '\x3', 
		'\x2', '\x2', '\x2', '\xC2', '\xC3', '\b', '\x16', '\x2', '\x2', '\xC3', 
		'-', '\x3', '\x2', '\x2', '\x2', '\xC4', '\xC5', '\a', '.', '\x2', '\x2', 
		'\xC5', '\xC6', '\a', '\"', '\x2', '\x2', '\xC6', '\xC7', '\a', 'g', '\x2', 
		'\x2', '\xC7', '\xC8', '\a', 'x', '\x2', '\x2', '\xC8', '\xC9', '\a', 
		'k', '\x2', '\x2', '\xC9', '\xCA', '\a', '\x66', '\x2', '\x2', '\xCA', 
		'\xCB', '\a', 'g', '\x2', '\x2', '\xCB', '\xCC', '\a', 'p', '\x2', '\x2', 
		'\xCC', '\xCD', '\a', '\x65', '\x2', '\x2', '\xCD', '\xCE', '\a', 'g', 
		'\x2', '\x2', '\xCE', '\xCF', '\a', '\x66', '\x2', '\x2', '\xCF', '\xD0', 
		'\a', '\"', '\x2', '\x2', '\xD0', '\xD1', '\a', '\x64', '\x2', '\x2', 
		'\xD1', '\xD2', '\a', '{', '\x2', '\x2', '\xD2', '\xD3', '\x3', '\x2', 
		'\x2', '\x2', '\xD3', '\xD4', '\b', '\x17', '\x4', '\x2', '\xD4', '/', 
		'\x3', '\x2', '\x2', '\x2', '\xD5', '\xD6', '\t', '\a', '\x2', '\x2', 
		'\xD6', '\x31', '\x3', '\x2', '\x2', '\x2', '\v', '\x2', '\x3', ';', '\x80', 
		'\x88', '\x8B', '\x91', '\xAF', '\xBE', '\x5', '\b', '\x2', '\x2', '\x4', 
		'\x3', '\x2', '\x4', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
