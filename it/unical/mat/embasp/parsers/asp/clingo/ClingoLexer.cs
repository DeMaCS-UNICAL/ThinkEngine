//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from ClingoLexer.g4 by ANTLR 4.7

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
public partial class ClingoLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		START=1, ANY=2, COMMA=3, INTEGER_CONSTANT=4, NEW_LINE=5, IDENTIFIER=6, 
		STRING_CONSTANT=7, TERMS_BEGIN=8, TERMS_END=9, WHITE_SPACE=10;
	public const int
		SIGNIFICANT=1;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE", "SIGNIFICANT"
	};

	public static readonly string[] ruleNames = {
		"START", "ANY", "COMMA", "INTEGER_CONSTANT", "NEW_LINE", "IDENTIFIER", 
		"STRING_CONSTANT", "TERMS_BEGIN", "TERMS_END", "WHITE_SPACE", "INT", "NL", 
		"WS"
	};


	public ClingoLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public ClingoLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

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

	public override string GrammarFileName { get { return "ClingoLexer.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static ClingoLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\f', '\x7F', '\b', '\x1', '\b', '\x1', '\x4', '\x2', 
		'\t', '\x2', '\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', 
		'\x5', '\t', '\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', 
		'\x4', '\b', '\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', 
		'\x4', '\v', '\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', 
		'\x4', '\xE', '\t', '\xE', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x3', '\a', '\x3', '.', '\n', '\x3', '\f', '\x3', '\xE', 
		'\x3', '\x31', '\v', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x5', '\x3', '\x5', 
		'\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', 
		'\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', 
		'\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', 
		'\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\a', '\x6', 'M', '\n', '\x6', 
		'\f', '\x6', '\xE', '\x6', 'P', '\v', '\x6', '\x3', '\x6', '\x5', '\x6', 
		'S', '\n', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3', '\a', 
		'\a', '\a', 'Y', '\n', '\a', '\f', '\a', '\xE', '\a', '\\', '\v', '\a', 
		'\x3', '\b', '\x3', '\b', '\a', '\b', '`', '\n', '\b', '\f', '\b', '\xE', 
		'\b', '\x63', '\v', '\b', '\x3', '\b', '\x3', '\b', '\x3', '\t', '\x3', 
		'\t', '\x3', '\n', '\x3', '\n', '\x3', '\v', '\x3', '\v', '\x3', '\v', 
		'\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', '\f', '\a', '\f', 'r', '\n', 
		'\f', '\f', '\f', '\xE', '\f', 'u', '\v', '\f', '\x5', '\f', 'w', '\n', 
		'\f', '\x3', '\r', '\x3', '\r', '\x3', '\r', '\x5', '\r', '|', '\n', '\r', 
		'\x3', '\xE', '\x3', '\xE', '\x3', '/', '\x2', '\xF', '\x4', '\x3', '\x6', 
		'\x4', '\b', '\x5', '\n', '\x6', '\f', '\a', '\xE', '\b', '\x10', '\t', 
		'\x12', '\n', '\x14', '\v', '\x16', '\f', '\x18', '\x2', '\x1A', '\x2', 
		'\x1C', '\x2', '\x4', '\x2', '\x3', '\t', '\x4', '\x2', '\x43', '\\', 
		'\x63', '|', '\x6', '\x2', '\x32', ';', '\x43', '\\', '\x61', '\x61', 
		'\x63', '|', '\x3', '\x2', '$', '$', '\x3', '\x2', '\x33', ';', '\x3', 
		'\x2', '\x32', ';', '\x4', '\x2', '\f', '\f', '\xF', '\xF', '\x4', '\x2', 
		'\v', '\v', '\"', '\"', '\x2', '\x82', '\x2', '\x4', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x6', '\x3', '\x2', '\x2', '\x2', '\x3', '\b', '\x3', '\x2', 
		'\x2', '\x2', '\x3', '\n', '\x3', '\x2', '\x2', '\x2', '\x3', '\f', '\x3', 
		'\x2', '\x2', '\x2', '\x3', '\xE', '\x3', '\x2', '\x2', '\x2', '\x3', 
		'\x10', '\x3', '\x2', '\x2', '\x2', '\x3', '\x12', '\x3', '\x2', '\x2', 
		'\x2', '\x3', '\x14', '\x3', '\x2', '\x2', '\x2', '\x3', '\x16', '\x3', 
		'\x2', '\x2', '\x2', '\x4', '\x1E', '\x3', '\x2', '\x2', '\x2', '\x6', 
		'/', '\x3', '\x2', '\x2', '\x2', '\b', '\x36', '\x3', '\x2', '\x2', '\x2', 
		'\n', '\x38', '\x3', '\x2', '\x2', '\x2', '\f', ':', '\x3', '\x2', '\x2', 
		'\x2', '\xE', 'V', '\x3', '\x2', '\x2', '\x2', '\x10', ']', '\x3', '\x2', 
		'\x2', '\x2', '\x12', '\x66', '\x3', '\x2', '\x2', '\x2', '\x14', 'h', 
		'\x3', '\x2', '\x2', '\x2', '\x16', 'j', '\x3', '\x2', '\x2', '\x2', '\x18', 
		'v', '\x3', '\x2', '\x2', '\x2', '\x1A', '{', '\x3', '\x2', '\x2', '\x2', 
		'\x1C', '}', '\x3', '\x2', '\x2', '\x2', '\x1E', '\x1F', '\a', '\x43', 
		'\x2', '\x2', '\x1F', ' ', '\a', 'p', '\x2', '\x2', ' ', '!', '\a', 'u', 
		'\x2', '\x2', '!', '\"', '\a', 'y', '\x2', '\x2', '\"', '#', '\a', 'g', 
		'\x2', '\x2', '#', '$', '\a', 't', '\x2', '\x2', '$', '%', '\a', '<', 
		'\x2', '\x2', '%', '&', '\a', '\"', '\x2', '\x2', '&', '\'', '\x3', '\x2', 
		'\x2', '\x2', '\'', '(', '\x5', '\x18', '\f', '\x2', '(', ')', '\x5', 
		'\x1A', '\r', '\x2', ')', '*', '\x3', '\x2', '\x2', '\x2', '*', '+', '\b', 
		'\x2', '\x2', '\x2', '+', '\x5', '\x3', '\x2', '\x2', '\x2', ',', '.', 
		'\v', '\x2', '\x2', '\x2', '-', ',', '\x3', '\x2', '\x2', '\x2', '.', 
		'\x31', '\x3', '\x2', '\x2', '\x2', '/', '\x30', '\x3', '\x2', '\x2', 
		'\x2', '/', '-', '\x3', '\x2', '\x2', '\x2', '\x30', '\x32', '\x3', '\x2', 
		'\x2', '\x2', '\x31', '/', '\x3', '\x2', '\x2', '\x2', '\x32', '\x33', 
		'\x5', '\x1A', '\r', '\x2', '\x33', '\x34', '\x3', '\x2', '\x2', '\x2', 
		'\x34', '\x35', '\b', '\x3', '\x3', '\x2', '\x35', '\a', '\x3', '\x2', 
		'\x2', '\x2', '\x36', '\x37', '\a', '.', '\x2', '\x2', '\x37', '\t', '\x3', 
		'\x2', '\x2', '\x2', '\x38', '\x39', '\x5', '\x18', '\f', '\x2', '\x39', 
		'\v', '\x3', '\x2', '\x2', '\x2', ':', 'R', '\x5', '\x1A', '\r', '\x2', 
		';', '<', '\a', 'Q', '\x2', '\x2', '<', '=', '\a', 'r', '\x2', '\x2', 
		'=', '>', '\a', 'v', '\x2', '\x2', '>', '?', '\a', 'k', '\x2', '\x2', 
		'?', '@', '\a', 'o', '\x2', '\x2', '@', '\x41', '\a', 'k', '\x2', '\x2', 
		'\x41', '\x42', '\a', '|', '\x2', '\x2', '\x42', '\x43', '\a', '\x63', 
		'\x2', '\x2', '\x43', '\x44', '\a', 'v', '\x2', '\x2', '\x44', '\x45', 
		'\a', 'k', '\x2', '\x2', '\x45', '\x46', '\a', 'q', '\x2', '\x2', '\x46', 
		'G', '\a', 'p', '\x2', '\x2', 'G', 'H', '\a', '<', '\x2', '\x2', 'H', 
		'N', '\x3', '\x2', '\x2', '\x2', 'I', 'J', '\x5', '\x1C', '\xE', '\x2', 
		'J', 'K', '\x5', '\x18', '\f', '\x2', 'K', 'M', '\x3', '\x2', '\x2', '\x2', 
		'L', 'I', '\x3', '\x2', '\x2', '\x2', 'M', 'P', '\x3', '\x2', '\x2', '\x2', 
		'N', 'L', '\x3', '\x2', '\x2', '\x2', 'N', 'O', '\x3', '\x2', '\x2', '\x2', 
		'O', 'Q', '\x3', '\x2', '\x2', '\x2', 'P', 'N', '\x3', '\x2', '\x2', '\x2', 
		'Q', 'S', '\x5', '\x1A', '\r', '\x2', 'R', ';', '\x3', '\x2', '\x2', '\x2', 
		'R', 'S', '\x3', '\x2', '\x2', '\x2', 'S', 'T', '\x3', '\x2', '\x2', '\x2', 
		'T', 'U', '\b', '\x6', '\x4', '\x2', 'U', '\r', '\x3', '\x2', '\x2', '\x2', 
		'V', 'Z', '\t', '\x2', '\x2', '\x2', 'W', 'Y', '\t', '\x3', '\x2', '\x2', 
		'X', 'W', '\x3', '\x2', '\x2', '\x2', 'Y', '\\', '\x3', '\x2', '\x2', 
		'\x2', 'Z', 'X', '\x3', '\x2', '\x2', '\x2', 'Z', '[', '\x3', '\x2', '\x2', 
		'\x2', '[', '\xF', '\x3', '\x2', '\x2', '\x2', '\\', 'Z', '\x3', '\x2', 
		'\x2', '\x2', ']', '\x61', '\a', '$', '\x2', '\x2', '^', '`', '\n', '\x4', 
		'\x2', '\x2', '_', '^', '\x3', '\x2', '\x2', '\x2', '`', '\x63', '\x3', 
		'\x2', '\x2', '\x2', '\x61', '_', '\x3', '\x2', '\x2', '\x2', '\x61', 
		'\x62', '\x3', '\x2', '\x2', '\x2', '\x62', '\x64', '\x3', '\x2', '\x2', 
		'\x2', '\x63', '\x61', '\x3', '\x2', '\x2', '\x2', '\x64', '\x65', '\a', 
		'$', '\x2', '\x2', '\x65', '\x11', '\x3', '\x2', '\x2', '\x2', '\x66', 
		'g', '\a', '*', '\x2', '\x2', 'g', '\x13', '\x3', '\x2', '\x2', '\x2', 
		'h', 'i', '\a', '+', '\x2', '\x2', 'i', '\x15', '\x3', '\x2', '\x2', '\x2', 
		'j', 'k', '\x5', '\x1C', '\xE', '\x2', 'k', 'l', '\x3', '\x2', '\x2', 
		'\x2', 'l', 'm', '\b', '\v', '\x3', '\x2', 'm', '\x17', '\x3', '\x2', 
		'\x2', '\x2', 'n', 'w', '\a', '\x32', '\x2', '\x2', 'o', 's', '\t', '\x5', 
		'\x2', '\x2', 'p', 'r', '\t', '\x6', '\x2', '\x2', 'q', 'p', '\x3', '\x2', 
		'\x2', '\x2', 'r', 'u', '\x3', '\x2', '\x2', '\x2', 's', 'q', '\x3', '\x2', 
		'\x2', '\x2', 's', 't', '\x3', '\x2', '\x2', '\x2', 't', 'w', '\x3', '\x2', 
		'\x2', '\x2', 'u', 's', '\x3', '\x2', '\x2', '\x2', 'v', 'n', '\x3', '\x2', 
		'\x2', '\x2', 'v', 'o', '\x3', '\x2', '\x2', '\x2', 'w', '\x19', '\x3', 
		'\x2', '\x2', '\x2', 'x', '|', '\t', '\a', '\x2', '\x2', 'y', 'z', '\a', 
		'\xF', '\x2', '\x2', 'z', '|', '\a', '\f', '\x2', '\x2', '{', 'x', '\x3', 
		'\x2', '\x2', '\x2', '{', 'y', '\x3', '\x2', '\x2', '\x2', '|', '\x1B', 
		'\x3', '\x2', '\x2', '\x2', '}', '~', '\t', '\b', '\x2', '\x2', '~', '\x1D', 
		'\x3', '\x2', '\x2', '\x2', '\f', '\x2', '\x3', '/', 'N', 'R', 'Z', '\x61', 
		's', 'v', '{', '\x5', '\x4', '\x3', '\x2', '\b', '\x2', '\x2', '\x4', 
		'\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
