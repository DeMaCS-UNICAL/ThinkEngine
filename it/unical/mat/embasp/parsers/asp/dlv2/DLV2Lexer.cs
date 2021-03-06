//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from DLV2Lexer.g4 by ANTLR 4.7

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
public partial class DLV2Lexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		START=1, COST_LABEL=2, ANY=3, IGNORE=4, AT=5, INTEGER=6, NEW_LINE=7, BLANK_SPACE=8, 
		COMMA=9, INTEGER_CONSTANT=10, IDENTIFIER=11, MODEL_END=12, STRING_CONSTANT=13, 
		TERMS_BEGIN=14, TERMS_END=15, WHITE_SPACE=16;
	public const int
		COST=1, MODEL=2;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE", "COST", "MODEL"
	};

	public static readonly string[] ruleNames = {
		"START", "COST_LABEL", "ANY", "IGNORE", "AT", "INTEGER", "NEW_LINE", "BLANK_SPACE", 
		"COMMA", "INTEGER_CONSTANT", "IDENTIFIER", "MODEL_END", "STRING_CONSTANT", 
		"TERMS_BEGIN", "TERMS_END", "WHITE_SPACE", "INT", "NL", "WS"
	};


	public DLV2Lexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public DLV2Lexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'{'", null, null, null, "'@'", null, null, null, "','", null, null, 
		"'}'", null, "'('", "')'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "START", "COST_LABEL", "ANY", "IGNORE", "AT", "INTEGER", "NEW_LINE", 
		"BLANK_SPACE", "COMMA", "INTEGER_CONSTANT", "IDENTIFIER", "MODEL_END", 
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

	public override string GrammarFileName { get { return "DLV2Lexer.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static DLV2Lexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x12', '\x84', '\b', '\x1', '\b', '\x1', '\b', '\x1', 
		'\x4', '\x2', '\t', '\x2', '\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', 
		'\x4', '\x4', '\x5', '\t', '\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', 
		'\t', '\a', '\x4', '\b', '\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', 
		'\t', '\n', '\x4', '\v', '\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', 
		'\t', '\r', '\x4', '\xE', '\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', 
		'\x10', '\t', '\x10', '\x4', '\x11', '\t', '\x11', '\x4', '\x12', '\t', 
		'\x12', '\x4', '\x13', '\t', '\x13', '\x4', '\x14', '\t', '\x14', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x4', '\x6', '\x4', '=', '\n', '\x4', '\r', '\x4', '\xE', 
		'\x4', '>', '\x3', '\x4', '\x3', '\x4', '\x3', '\x5', '\x3', '\x5', '\x5', 
		'\x5', '\x45', '\n', '\x5', '\x3', '\x5', '\x3', '\x5', '\x3', '\x6', 
		'\x3', '\x6', '\x3', '\a', '\x3', '\a', '\x3', '\b', '\x3', '\b', '\x3', 
		'\b', '\x3', '\b', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', '\t', 
		'\x3', '\n', '\x3', '\n', '\x3', '\v', '\x3', '\v', '\x3', '\f', '\x3', 
		'\f', '\a', '\f', '[', '\n', '\f', '\f', '\f', '\xE', '\f', '^', '\v', 
		'\f', '\x3', '\r', '\x3', '\r', '\x3', '\r', '\x3', '\r', '\x3', '\xE', 
		'\x3', '\xE', '\a', '\xE', '\x66', '\n', '\xE', '\f', '\xE', '\xE', '\xE', 
		'i', '\v', '\xE', '\x3', '\xE', '\x3', '\xE', '\x3', '\xF', '\x3', '\xF', 
		'\x3', '\x10', '\x3', '\x10', '\x3', '\x11', '\x3', '\x11', '\x5', '\x11', 
		's', '\n', '\x11', '\x3', '\x11', '\x3', '\x11', '\x3', '\x12', '\x3', 
		'\x12', '\x3', '\x12', '\a', '\x12', 'z', '\n', '\x12', '\f', '\x12', 
		'\xE', '\x12', '}', '\v', '\x12', '\x5', '\x12', '\x7F', '\n', '\x12', 
		'\x3', '\x13', '\x3', '\x13', '\x3', '\x14', '\x3', '\x14', '\x3', '>', 
		'\x2', '\x15', '\x5', '\x3', '\a', '\x4', '\t', '\x5', '\v', '\x6', '\r', 
		'\a', '\xF', '\b', '\x11', '\t', '\x13', '\n', '\x15', '\v', '\x17', '\f', 
		'\x19', '\r', '\x1B', '\xE', '\x1D', '\xF', '\x1F', '\x10', '!', '\x11', 
		'#', '\x12', '%', '\x2', '\'', '\x2', ')', '\x2', '\x5', '\x2', '\x3', 
		'\x4', '\t', '\x4', '\x2', '\x43', '\\', '\x63', '|', '\x6', '\x2', '\x32', 
		';', '\x43', '\\', '\x61', '\x61', '\x63', '|', '\x3', '\x2', '$', '$', 
		'\x3', '\x2', '\x33', ';', '\x3', '\x2', '\x32', ';', '\x4', '\x2', '\f', 
		'\f', '\xF', '\xF', '\x4', '\x2', '\v', '\v', '\"', '\"', '\x2', '\x85', 
		'\x2', '\x5', '\x3', '\x2', '\x2', '\x2', '\x2', '\a', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\t', '\x3', '\x2', '\x2', '\x2', '\x2', '\v', '\x3', '\x2', 
		'\x2', '\x2', '\x3', '\r', '\x3', '\x2', '\x2', '\x2', '\x3', '\xF', '\x3', 
		'\x2', '\x2', '\x2', '\x3', '\x11', '\x3', '\x2', '\x2', '\x2', '\x3', 
		'\x13', '\x3', '\x2', '\x2', '\x2', '\x4', '\x15', '\x3', '\x2', '\x2', 
		'\x2', '\x4', '\x17', '\x3', '\x2', '\x2', '\x2', '\x4', '\x19', '\x3', 
		'\x2', '\x2', '\x2', '\x4', '\x1B', '\x3', '\x2', '\x2', '\x2', '\x4', 
		'\x1D', '\x3', '\x2', '\x2', '\x2', '\x4', '\x1F', '\x3', '\x2', '\x2', 
		'\x2', '\x4', '!', '\x3', '\x2', '\x2', '\x2', '\x4', '#', '\x3', '\x2', 
		'\x2', '\x2', '\x5', '+', '\x3', '\x2', '\x2', '\x2', '\a', '/', '\x3', 
		'\x2', '\x2', '\x2', '\t', '<', '\x3', '\x2', '\x2', '\x2', '\v', '\x44', 
		'\x3', '\x2', '\x2', '\x2', '\r', 'H', '\x3', '\x2', '\x2', '\x2', '\xF', 
		'J', '\x3', '\x2', '\x2', '\x2', '\x11', 'L', '\x3', '\x2', '\x2', '\x2', 
		'\x13', 'P', '\x3', '\x2', '\x2', '\x2', '\x15', 'T', '\x3', '\x2', '\x2', 
		'\x2', '\x17', 'V', '\x3', '\x2', '\x2', '\x2', '\x19', 'X', '\x3', '\x2', 
		'\x2', '\x2', '\x1B', '_', '\x3', '\x2', '\x2', '\x2', '\x1D', '\x63', 
		'\x3', '\x2', '\x2', '\x2', '\x1F', 'l', '\x3', '\x2', '\x2', '\x2', '!', 
		'n', '\x3', '\x2', '\x2', '\x2', '#', 'r', '\x3', '\x2', '\x2', '\x2', 
		'%', '~', '\x3', '\x2', '\x2', '\x2', '\'', '\x80', '\x3', '\x2', '\x2', 
		'\x2', ')', '\x82', '\x3', '\x2', '\x2', '\x2', '+', ',', '\a', '}', '\x2', 
		'\x2', ',', '-', '\x3', '\x2', '\x2', '\x2', '-', '.', '\b', '\x2', '\x2', 
		'\x2', '.', '\x6', '\x3', '\x2', '\x2', '\x2', '/', '\x30', '\a', '\x45', 
		'\x2', '\x2', '\x30', '\x31', '\a', 'Q', '\x2', '\x2', '\x31', '\x32', 
		'\a', 'U', '\x2', '\x2', '\x32', '\x33', '\a', 'V', '\x2', '\x2', '\x33', 
		'\x34', '\a', '\"', '\x2', '\x2', '\x34', '\x35', '\x3', '\x2', '\x2', 
		'\x2', '\x35', '\x36', '\x5', '%', '\x12', '\x2', '\x36', '\x37', '\a', 
		'\x42', '\x2', '\x2', '\x37', '\x38', '\x5', '%', '\x12', '\x2', '\x38', 
		'\x39', '\x3', '\x2', '\x2', '\x2', '\x39', ':', '\b', '\x3', '\x3', '\x2', 
		':', '\b', '\x3', '\x2', '\x2', '\x2', ';', '=', '\v', '\x2', '\x2', '\x2', 
		'<', ';', '\x3', '\x2', '\x2', '\x2', '=', '>', '\x3', '\x2', '\x2', '\x2', 
		'>', '?', '\x3', '\x2', '\x2', '\x2', '>', '<', '\x3', '\x2', '\x2', '\x2', 
		'?', '@', '\x3', '\x2', '\x2', '\x2', '@', '\x41', '\b', '\x4', '\x4', 
		'\x2', '\x41', '\n', '\x3', '\x2', '\x2', '\x2', '\x42', '\x45', '\x5', 
		'\'', '\x13', '\x2', '\x43', '\x45', '\x5', ')', '\x14', '\x2', '\x44', 
		'\x42', '\x3', '\x2', '\x2', '\x2', '\x44', '\x43', '\x3', '\x2', '\x2', 
		'\x2', '\x45', '\x46', '\x3', '\x2', '\x2', '\x2', '\x46', 'G', '\b', 
		'\x5', '\x4', '\x2', 'G', '\f', '\x3', '\x2', '\x2', '\x2', 'H', 'I', 
		'\a', '\x42', '\x2', '\x2', 'I', '\xE', '\x3', '\x2', '\x2', '\x2', 'J', 
		'K', '\x5', '%', '\x12', '\x2', 'K', '\x10', '\x3', '\x2', '\x2', '\x2', 
		'L', 'M', '\x5', '\'', '\x13', '\x2', 'M', 'N', '\x3', '\x2', '\x2', '\x2', 
		'N', 'O', '\b', '\b', '\x5', '\x2', 'O', '\x12', '\x3', '\x2', '\x2', 
		'\x2', 'P', 'Q', '\x5', ')', '\x14', '\x2', 'Q', 'R', '\x3', '\x2', '\x2', 
		'\x2', 'R', 'S', '\b', '\t', '\x4', '\x2', 'S', '\x14', '\x3', '\x2', 
		'\x2', '\x2', 'T', 'U', '\a', '.', '\x2', '\x2', 'U', '\x16', '\x3', '\x2', 
		'\x2', '\x2', 'V', 'W', '\x5', '%', '\x12', '\x2', 'W', '\x18', '\x3', 
		'\x2', '\x2', '\x2', 'X', '\\', '\t', '\x2', '\x2', '\x2', 'Y', '[', '\t', 
		'\x3', '\x2', '\x2', 'Z', 'Y', '\x3', '\x2', '\x2', '\x2', '[', '^', '\x3', 
		'\x2', '\x2', '\x2', '\\', 'Z', '\x3', '\x2', '\x2', '\x2', '\\', ']', 
		'\x3', '\x2', '\x2', '\x2', ']', '\x1A', '\x3', '\x2', '\x2', '\x2', '^', 
		'\\', '\x3', '\x2', '\x2', '\x2', '_', '`', '\a', '\x7F', '\x2', '\x2', 
		'`', '\x61', '\x3', '\x2', '\x2', '\x2', '\x61', '\x62', '\b', '\r', '\x5', 
		'\x2', '\x62', '\x1C', '\x3', '\x2', '\x2', '\x2', '\x63', 'g', '\a', 
		'$', '\x2', '\x2', '\x64', '\x66', '\n', '\x4', '\x2', '\x2', '\x65', 
		'\x64', '\x3', '\x2', '\x2', '\x2', '\x66', 'i', '\x3', '\x2', '\x2', 
		'\x2', 'g', '\x65', '\x3', '\x2', '\x2', '\x2', 'g', 'h', '\x3', '\x2', 
		'\x2', '\x2', 'h', 'j', '\x3', '\x2', '\x2', '\x2', 'i', 'g', '\x3', '\x2', 
		'\x2', '\x2', 'j', 'k', '\a', '$', '\x2', '\x2', 'k', '\x1E', '\x3', '\x2', 
		'\x2', '\x2', 'l', 'm', '\a', '*', '\x2', '\x2', 'm', ' ', '\x3', '\x2', 
		'\x2', '\x2', 'n', 'o', '\a', '+', '\x2', '\x2', 'o', '\"', '\x3', '\x2', 
		'\x2', '\x2', 'p', 's', '\x5', ')', '\x14', '\x2', 'q', 's', '\x5', '\'', 
		'\x13', '\x2', 'r', 'p', '\x3', '\x2', '\x2', '\x2', 'r', 'q', '\x3', 
		'\x2', '\x2', '\x2', 's', 't', '\x3', '\x2', '\x2', '\x2', 't', 'u', '\b', 
		'\x11', '\x4', '\x2', 'u', '$', '\x3', '\x2', '\x2', '\x2', 'v', '\x7F', 
		'\a', '\x32', '\x2', '\x2', 'w', '{', '\t', '\x5', '\x2', '\x2', 'x', 
		'z', '\t', '\x6', '\x2', '\x2', 'y', 'x', '\x3', '\x2', '\x2', '\x2', 
		'z', '}', '\x3', '\x2', '\x2', '\x2', '{', 'y', '\x3', '\x2', '\x2', '\x2', 
		'{', '|', '\x3', '\x2', '\x2', '\x2', '|', '\x7F', '\x3', '\x2', '\x2', 
		'\x2', '}', '{', '\x3', '\x2', '\x2', '\x2', '~', 'v', '\x3', '\x2', '\x2', 
		'\x2', '~', 'w', '\x3', '\x2', '\x2', '\x2', '\x7F', '&', '\x3', '\x2', 
		'\x2', '\x2', '\x80', '\x81', '\t', '\a', '\x2', '\x2', '\x81', '(', '\x3', 
		'\x2', '\x2', '\x2', '\x82', '\x83', '\t', '\b', '\x2', '\x2', '\x83', 
		'*', '\x3', '\x2', '\x2', '\x2', '\f', '\x2', '\x3', '\x4', '>', '\x44', 
		'\\', 'g', 'r', '{', '~', '\x6', '\x4', '\x4', '\x2', '\x4', '\x3', '\x2', 
		'\b', '\x2', '\x2', '\x4', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
