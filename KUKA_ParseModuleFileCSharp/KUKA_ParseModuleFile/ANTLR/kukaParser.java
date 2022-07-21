// Generated from C:\Users\lholetzke\Documents\Visual Studio 2012\Projects\KUKA_ParseModuleFileCSharp\KUKA_ParseModuleFile\ANTLR\kuka.g4 by ANTLR 4.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class kukaParser extends Parser {
	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		COMMENT=1, FILEATTRIBUTES=2, NEWLINE=3, DEFDAT=4, ENDDAT=5, DECL=6, GLOBAL=7, 
		PUBLIC=8, EXT=9, IN=10, OUT=11, SIGNAL=12, TO=13, ENUMD=14, STRUCD=15, 
		XCONST=16, ASSIGMENT=17, RBracketOpen=18, RBracketClose=19, BracketOpen=20, 
		BracketClose=21, BraceOpen=22, BraceClose=23, COMMA=24, COLON=25, WS=26, 
		BOOL=27, ID=28, ENUM=29, INT=30, FLOAT=31, BITArr=32, STRING=33;
	public static final String[] tokenNames = {
		"<INVALID>", "COMMENT", "FILEATTRIBUTES", "NEWLINE", "'DEFDAT'", "'ENDDAT'", 
		"'DECL'", "'GLOBAL'", "'PUBLIC'", "'EXT'", "'IN'", "'OUT'", "'SIGNAL'", 
		"'TO'", "'ENUM'", "'STRUC'", "'CONST'", "'='", "'('", "')'", "'['", "']'", 
		"'{'", "'}'", "','", "':'", "WS", "BOOL", "ID", "ENUM", "INT", "FLOAT", 
		"BITArr", "STRING"
	};
	public static final int
		RULE_prog = 0, RULE_defdat = 1, RULE_line = 2, RULE_data = 3, RULE_noDecl = 4, 
		RULE_assign = 5, RULE_decl = 6, RULE_xconst = 7, RULE_ext = 8, RULE_signal = 9, 
		RULE_myenum = 10, RULE_mystruc = 11, RULE_structure = 12, RULE_parameterList = 13, 
		RULE_params = 14, RULE_sparamList = 15, RULE_parameters = 16, RULE_signalRange = 17, 
		RULE_array = 18, RULE_idList = 19, RULE_value = 20;
	public static final String[] ruleNames = {
		"prog", "defdat", "line", "data", "noDecl", "assign", "decl", "xconst", 
		"ext", "signal", "myenum", "mystruc", "structure", "parameterList", "params", 
		"sparamList", "parameters", "signalRange", "array", "idList", "value"
	};

	@Override
	public String getGrammarFileName() { return "kuka.g4"; }

	@Override
	public String[] getTokenNames() { return tokenNames; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public kukaParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class ProgContext extends ParserRuleContext {
		public List<TerminalNode> NEWLINE() { return getTokens(kukaParser.NEWLINE); }
		public TerminalNode EOF() { return getToken(kukaParser.EOF, 0); }
		public DefdatContext defdat() {
			return getRuleContext(DefdatContext.class,0);
		}
		public LineContext line(int i) {
			return getRuleContext(LineContext.class,i);
		}
		public TerminalNode NEWLINE(int i) {
			return getToken(kukaParser.NEWLINE, i);
		}
		public TerminalNode COMMENT(int i) {
			return getToken(kukaParser.COMMENT, i);
		}
		public TerminalNode FILEATTRIBUTES(int i) {
			return getToken(kukaParser.FILEATTRIBUTES, i);
		}
		public List<TerminalNode> FILEATTRIBUTES() { return getTokens(kukaParser.FILEATTRIBUTES); }
		public List<TerminalNode> COMMENT() { return getTokens(kukaParser.COMMENT); }
		public List<LineContext> line() {
			return getRuleContexts(LineContext.class);
		}
		public TerminalNode ENDDAT() { return getToken(kukaParser.ENDDAT, 0); }
		public ProgContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_prog; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterProg(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitProg(this);
		}
	}

	public final ProgContext prog() throws RecognitionException {
		ProgContext _localctx = new ProgContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_prog);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(45);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==FILEATTRIBUTES) {
				{
				{
				setState(42); match(FILEATTRIBUTES);
				}
				}
				setState(47);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(48); defdat();
			setState(49); match(NEWLINE);
			setState(53);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << COMMENT) | (1L << NEWLINE) | (1L << DECL) | (1L << GLOBAL) | (1L << EXT) | (1L << SIGNAL) | (1L << ENUMD) | (1L << STRUCD) | (1L << XCONST) | (1L << ID))) != 0)) {
				{
				{
				setState(50); line();
				}
				}
				setState(55);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(56); match(ENDDAT);
			setState(58);
			switch ( getInterpreter().adaptivePredict(_input,2,_ctx) ) {
			case 1:
				{
				setState(57); match(COMMENT);
				}
				break;
			}
			setState(61);
			switch ( getInterpreter().adaptivePredict(_input,3,_ctx) ) {
			case 1:
				{
				setState(60); match(NEWLINE);
				}
				break;
			}
			setState(69);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMENT || _la==NEWLINE) {
				{
				{
				setState(64);
				_la = _input.LA(1);
				if (_la==COMMENT) {
					{
					setState(63); match(COMMENT);
					}
				}

				setState(66); match(NEWLINE);
				}
				}
				setState(71);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(72); match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class DefdatContext extends ParserRuleContext {
		public TerminalNode PUBLIC() { return getToken(kukaParser.PUBLIC, 0); }
		public TerminalNode DEFDAT() { return getToken(kukaParser.DEFDAT, 0); }
		public TerminalNode ID() { return getToken(kukaParser.ID, 0); }
		public DefdatContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_defdat; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterDefdat(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitDefdat(this);
		}
	}

	public final DefdatContext defdat() throws RecognitionException {
		DefdatContext _localctx = new DefdatContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_defdat);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(74); match(DEFDAT);
			setState(75); match(ID);
			setState(77);
			_la = _input.LA(1);
			if (_la==PUBLIC) {
				{
				setState(76); match(PUBLIC);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LineContext extends ParserRuleContext {
		public TerminalNode NEWLINE() { return getToken(kukaParser.NEWLINE, 0); }
		public TerminalNode COMMENT(int i) {
			return getToken(kukaParser.COMMENT, i);
		}
		public DataContext data() {
			return getRuleContext(DataContext.class,0);
		}
		public List<TerminalNode> COMMENT() { return getTokens(kukaParser.COMMENT); }
		public MystrucContext mystruc() {
			return getRuleContext(MystrucContext.class,0);
		}
		public MyenumContext myenum() {
			return getRuleContext(MyenumContext.class,0);
		}
		public SignalContext signal() {
			return getRuleContext(SignalContext.class,0);
		}
		public ExtContext ext() {
			return getRuleContext(ExtContext.class,0);
		}
		public LineContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_line; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterLine(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitLine(this);
		}
	}

	public final LineContext line() throws RecognitionException {
		LineContext _localctx = new LineContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_line);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(87);
			switch ( getInterpreter().adaptivePredict(_input,8,_ctx) ) {
			case 1:
				{
				setState(80);
				switch ( getInterpreter().adaptivePredict(_input,7,_ctx) ) {
				case 1:
					{
					setState(79); match(COMMENT);
					}
					break;
				}
				}
				break;

			case 2:
				{
				setState(82); data();
				}
				break;

			case 3:
				{
				setState(83); myenum();
				}
				break;

			case 4:
				{
				setState(84); mystruc();
				}
				break;

			case 5:
				{
				setState(85); signal();
				}
				break;

			case 6:
				{
				setState(86); ext();
				}
				break;
			}
			setState(90);
			_la = _input.LA(1);
			if (_la==COMMENT) {
				{
				setState(89); match(COMMENT);
				}
			}

			setState(92); match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class DataContext extends ParserRuleContext {
		public XconstContext xconst() {
			return getRuleContext(XconstContext.class,0);
		}
		public AssignContext assign() {
			return getRuleContext(AssignContext.class,0);
		}
		public DeclContext decl() {
			return getRuleContext(DeclContext.class,0);
		}
		public NoDeclContext noDecl() {
			return getRuleContext(NoDeclContext.class,0);
		}
		public DataContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_data; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterData(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitData(this);
		}
	}

	public final DataContext data() throws RecognitionException {
		DataContext _localctx = new DataContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_data);
		try {
			setState(98);
			switch ( getInterpreter().adaptivePredict(_input,10,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(94); noDecl();
				}
				break;

			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(95); assign();
				}
				break;

			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(96); decl();
				}
				break;

			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(97); xconst();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class NoDeclContext extends ParserRuleContext {
		public TerminalNode GLOBAL() { return getToken(kukaParser.GLOBAL, 0); }
		public List<TerminalNode> COMMA() { return getTokens(kukaParser.COMMA); }
		public List<AssignContext> assign() {
			return getRuleContexts(AssignContext.class);
		}
		public TerminalNode ID() { return getToken(kukaParser.ID, 0); }
		public AssignContext assign(int i) {
			return getRuleContext(AssignContext.class,i);
		}
		public TerminalNode COMMA(int i) {
			return getToken(kukaParser.COMMA, i);
		}
		public ArrayContext array() {
			return getRuleContext(ArrayContext.class,0);
		}
		public NoDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_noDecl; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterNoDecl(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitNoDecl(this);
		}
	}

	public final NoDeclContext noDecl() throws RecognitionException {
		NoDeclContext _localctx = new NoDeclContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_noDecl);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(101);
			_la = _input.LA(1);
			if (_la==GLOBAL) {
				{
				setState(100); match(GLOBAL);
				}
			}

			setState(103); match(ID);
			setState(105);
			_la = _input.LA(1);
			if (_la==BracketOpen) {
				{
				setState(104); array();
				}
			}

			setState(107); assign();
			setState(114);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(108); match(COMMA);
				setState(110);
				_la = _input.LA(1);
				if (_la==ID) {
					{
					setState(109); assign();
					}
				}

				}
				}
				setState(116);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AssignContext extends ParserRuleContext {
		public ValueContext value() {
			return getRuleContext(ValueContext.class,0);
		}
		public TerminalNode ID() { return getToken(kukaParser.ID, 0); }
		public TerminalNode ASSIGMENT() { return getToken(kukaParser.ASSIGMENT, 0); }
		public ArrayContext array() {
			return getRuleContext(ArrayContext.class,0);
		}
		public AssignContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_assign; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterAssign(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitAssign(this);
		}
	}

	public final AssignContext assign() throws RecognitionException {
		AssignContext _localctx = new AssignContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_assign);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(117); match(ID);
			setState(119);
			_la = _input.LA(1);
			if (_la==BracketOpen) {
				{
				setState(118); array();
				}
			}

			setState(123);
			_la = _input.LA(1);
			if (_la==ASSIGMENT) {
				{
				setState(121); match(ASSIGMENT);
				setState(122); value();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class DeclContext extends ParserRuleContext {
		public TerminalNode DECL() { return getToken(kukaParser.DECL, 0); }
		public List<TerminalNode> GLOBAL() { return getTokens(kukaParser.GLOBAL); }
		public AssignContext assign() {
			return getRuleContext(AssignContext.class,0);
		}
		public TerminalNode ID() { return getToken(kukaParser.ID, 0); }
		public TerminalNode GLOBAL(int i) {
			return getToken(kukaParser.GLOBAL, i);
		}
		public DeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_decl; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterDecl(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitDecl(this);
		}
	}

	public final DeclContext decl() throws RecognitionException {
		DeclContext _localctx = new DeclContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_decl);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(126);
			_la = _input.LA(1);
			if (_la==GLOBAL) {
				{
				setState(125); match(GLOBAL);
				}
			}

			setState(128); match(DECL);
			setState(130);
			_la = _input.LA(1);
			if (_la==GLOBAL) {
				{
				setState(129); match(GLOBAL);
				}
			}

			setState(132); match(ID);
			setState(133); assign();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class XconstContext extends ParserRuleContext {
		public TerminalNode XCONST() { return getToken(kukaParser.XCONST, 0); }
		public TerminalNode GLOBAL() { return getToken(kukaParser.GLOBAL, 0); }
		public AssignContext assign() {
			return getRuleContext(AssignContext.class,0);
		}
		public TerminalNode ID() { return getToken(kukaParser.ID, 0); }
		public XconstContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_xconst; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterXconst(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitXconst(this);
		}
	}

	public final XconstContext xconst() throws RecognitionException {
		XconstContext _localctx = new XconstContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_xconst);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(136);
			_la = _input.LA(1);
			if (_la==GLOBAL) {
				{
				setState(135); match(GLOBAL);
				}
			}

			setState(138); match(XCONST);
			setState(139); match(ID);
			setState(140); assign();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ExtContext extends ParserRuleContext {
		public TerminalNode EXT() { return getToken(kukaParser.EXT, 0); }
		public TerminalNode ID() { return getToken(kukaParser.ID, 0); }
		public TerminalNode RBracketOpen() { return getToken(kukaParser.RBracketOpen, 0); }
		public ParameterListContext parameterList() {
			return getRuleContext(ParameterListContext.class,0);
		}
		public TerminalNode RBracketClose() { return getToken(kukaParser.RBracketClose, 0); }
		public ExtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ext; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterExt(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitExt(this);
		}
	}

	public final ExtContext ext() throws RecognitionException {
		ExtContext _localctx = new ExtContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_ext);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(142); match(EXT);
			setState(143); match(ID);
			setState(144); match(RBracketOpen);
			setState(146);
			_la = _input.LA(1);
			if (_la==ID) {
				{
				setState(145); parameterList();
				}
			}

			setState(148); match(RBracketClose);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class SignalContext extends ParserRuleContext {
		public TerminalNode GLOBAL() { return getToken(kukaParser.GLOBAL, 0); }
		public TerminalNode ID() { return getToken(kukaParser.ID, 0); }
		public TerminalNode SIGNAL() { return getToken(kukaParser.SIGNAL, 0); }
		public SignalRangeContext signalRange() {
			return getRuleContext(SignalRangeContext.class,0);
		}
		public SignalContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_signal; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterSignal(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitSignal(this);
		}
	}

	public final SignalContext signal() throws RecognitionException {
		SignalContext _localctx = new SignalContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_signal);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(151);
			_la = _input.LA(1);
			if (_la==GLOBAL) {
				{
				setState(150); match(GLOBAL);
				}
			}

			setState(153); match(SIGNAL);
			setState(154); match(ID);
			setState(155); signalRange();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class MyenumContext extends ParserRuleContext {
		public TerminalNode GLOBAL() { return getToken(kukaParser.GLOBAL, 0); }
		public IdListContext idList() {
			return getRuleContext(IdListContext.class,0);
		}
		public TerminalNode ID() { return getToken(kukaParser.ID, 0); }
		public TerminalNode ENUMD() { return getToken(kukaParser.ENUMD, 0); }
		public MyenumContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_myenum; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterMyenum(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitMyenum(this);
		}
	}

	public final MyenumContext myenum() throws RecognitionException {
		MyenumContext _localctx = new MyenumContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_myenum);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(158);
			_la = _input.LA(1);
			if (_la==GLOBAL) {
				{
				setState(157); match(GLOBAL);
				}
			}

			setState(160); match(ENUMD);
			setState(161); match(ID);
			setState(162); idList();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class MystrucContext extends ParserRuleContext {
		public TerminalNode GLOBAL() { return getToken(kukaParser.GLOBAL, 0); }
		public TerminalNode ID() { return getToken(kukaParser.ID, 0); }
		public TerminalNode STRUCD() { return getToken(kukaParser.STRUCD, 0); }
		public SparamListContext sparamList() {
			return getRuleContext(SparamListContext.class,0);
		}
		public MystrucContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_mystruc; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterMystruc(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitMystruc(this);
		}
	}

	public final MystrucContext mystruc() throws RecognitionException {
		MystrucContext _localctx = new MystrucContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_mystruc);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(165);
			_la = _input.LA(1);
			if (_la==GLOBAL) {
				{
				setState(164); match(GLOBAL);
				}
			}

			setState(167); match(STRUCD);
			setState(168); match(ID);
			setState(169); sparamList();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class StructureContext extends ParserRuleContext {
		public ValueContext value(int i) {
			return getRuleContext(ValueContext.class,i);
		}
		public List<ValueContext> value() {
			return getRuleContexts(ValueContext.class);
		}
		public ArrayContext array(int i) {
			return getRuleContext(ArrayContext.class,i);
		}
		public TerminalNode BraceOpen() { return getToken(kukaParser.BraceOpen, 0); }
		public List<TerminalNode> COMMA() { return getTokens(kukaParser.COMMA); }
		public List<TerminalNode> ID() { return getTokens(kukaParser.ID); }
		public TerminalNode BraceClose() { return getToken(kukaParser.BraceClose, 0); }
		public TerminalNode COMMA(int i) {
			return getToken(kukaParser.COMMA, i);
		}
		public List<ArrayContext> array() {
			return getRuleContexts(ArrayContext.class);
		}
		public TerminalNode ID(int i) {
			return getToken(kukaParser.ID, i);
		}
		public StructureContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_structure; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterStructure(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitStructure(this);
		}
	}

	public final StructureContext structure() throws RecognitionException {
		StructureContext _localctx = new StructureContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_structure);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(171); match(BraceOpen);
			setState(172); match(ID);
			setState(174);
			_la = _input.LA(1);
			if (_la==BracketOpen) {
				{
				setState(173); array();
				}
			}

			setState(176); value();
			setState(185);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(177); match(COMMA);
				setState(178); match(ID);
				setState(180);
				_la = _input.LA(1);
				if (_la==BracketOpen) {
					{
					setState(179); array();
					}
				}

				setState(182); value();
				}
				}
				setState(187);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(188); match(BraceClose);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParameterListContext extends ParserRuleContext {
		public List<TerminalNode> COMMA() { return getTokens(kukaParser.COMMA); }
		public ParametersContext parameters(int i) {
			return getRuleContext(ParametersContext.class,i);
		}
		public List<ParametersContext> parameters() {
			return getRuleContexts(ParametersContext.class);
		}
		public TerminalNode COMMA(int i) {
			return getToken(kukaParser.COMMA, i);
		}
		public ParameterListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameterList; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterParameterList(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitParameterList(this);
		}
	}

	public final ParameterListContext parameterList() throws RecognitionException {
		ParameterListContext _localctx = new ParameterListContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_parameterList);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(190); parameters();
			setState(195);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(191); match(COMMA);
				setState(192); parameters();
				}
				}
				setState(197);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParamsContext extends ParserRuleContext {
		public ArrayContext array(int i) {
			return getRuleContext(ArrayContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(kukaParser.COMMA); }
		public List<TerminalNode> ID() { return getTokens(kukaParser.ID); }
		public TerminalNode COMMA(int i) {
			return getToken(kukaParser.COMMA, i);
		}
		public List<ArrayContext> array() {
			return getRuleContexts(ArrayContext.class);
		}
		public TerminalNode ID(int i) {
			return getToken(kukaParser.ID, i);
		}
		public ParamsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_params; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterParams(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitParams(this);
		}
	}

	public final ParamsContext params() throws RecognitionException {
		ParamsContext _localctx = new ParamsContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_params);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(198); match(ID);
			setState(199); match(ID);
			setState(201);
			_la = _input.LA(1);
			if (_la==BracketOpen) {
				{
				setState(200); array();
				}
			}

			setState(210);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,30,_ctx);
			while ( _alt!=2 && _alt!=-1 ) {
				if ( _alt==1 ) {
					{
					{
					setState(203); match(COMMA);
					setState(204); match(ID);
					setState(206);
					_la = _input.LA(1);
					if (_la==BracketOpen) {
						{
						setState(205); array();
						}
					}

					}
					} 
				}
				setState(212);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,30,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class SparamListContext extends ParserRuleContext {
		public List<ParamsContext> params() {
			return getRuleContexts(ParamsContext.class);
		}
		public List<TerminalNode> COMMA() { return getTokens(kukaParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(kukaParser.COMMA, i);
		}
		public ParamsContext params(int i) {
			return getRuleContext(ParamsContext.class,i);
		}
		public SparamListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_sparamList; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterSparamList(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitSparamList(this);
		}
	}

	public final SparamListContext sparamList() throws RecognitionException {
		SparamListContext _localctx = new SparamListContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_sparamList);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(213); params();
			setState(218);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(214); match(COMMA);
				setState(215); params();
				}
				}
				setState(220);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParametersContext extends ParserRuleContext {
		public TerminalNode IN() { return getToken(kukaParser.IN, 0); }
		public ArrayContext array(int i) {
			return getRuleContext(ArrayContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(kukaParser.COMMA); }
		public List<TerminalNode> ID() { return getTokens(kukaParser.ID); }
		public TerminalNode OUT() { return getToken(kukaParser.OUT, 0); }
		public TerminalNode COLON() { return getToken(kukaParser.COLON, 0); }
		public TerminalNode COMMA(int i) {
			return getToken(kukaParser.COMMA, i);
		}
		public List<ArrayContext> array() {
			return getRuleContexts(ArrayContext.class);
		}
		public TerminalNode ID(int i) {
			return getToken(kukaParser.ID, i);
		}
		public ParametersContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameters; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterParameters(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitParameters(this);
		}
	}

	public final ParametersContext parameters() throws RecognitionException {
		ParametersContext _localctx = new ParametersContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_parameters);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(221); match(ID);
			setState(223);
			_la = _input.LA(1);
			if (_la==BracketOpen) {
				{
				setState(222); array();
				}
			}

			setState(232);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(225); match(COMMA);
				setState(226); match(ID);
				setState(228);
				_la = _input.LA(1);
				if (_la==BracketOpen) {
					{
					setState(227); array();
					}
				}

				}
				}
				setState(234);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(235); match(COLON);
			setState(236);
			_la = _input.LA(1);
			if ( !(_la==IN || _la==OUT) ) {
			_errHandler.recoverInline(this);
			}
			consume();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class SignalRangeContext extends ParserRuleContext {
		public ArrayContext array(int i) {
			return getRuleContext(ArrayContext.class,i);
		}
		public List<TerminalNode> ID() { return getTokens(kukaParser.ID); }
		public TerminalNode TO() { return getToken(kukaParser.TO, 0); }
		public List<ArrayContext> array() {
			return getRuleContexts(ArrayContext.class);
		}
		public TerminalNode ID(int i) {
			return getToken(kukaParser.ID, i);
		}
		public SignalRangeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_signalRange; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterSignalRange(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitSignalRange(this);
		}
	}

	public final SignalRangeContext signalRange() throws RecognitionException {
		SignalRangeContext _localctx = new SignalRangeContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_signalRange);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(238); match(ID);
			setState(239); array();
			setState(243);
			_la = _input.LA(1);
			if (_la==TO) {
				{
				setState(240); match(TO);
				setState(241); match(ID);
				setState(242); array();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ArrayContext extends ParserRuleContext {
		public TerminalNode BracketClose() { return getToken(kukaParser.BracketClose, 0); }
		public List<TerminalNode> INT() { return getTokens(kukaParser.INT); }
		public TerminalNode BracketOpen() { return getToken(kukaParser.BracketOpen, 0); }
		public List<TerminalNode> COMMA() { return getTokens(kukaParser.COMMA); }
		public TerminalNode INT(int i) {
			return getToken(kukaParser.INT, i);
		}
		public TerminalNode COMMA(int i) {
			return getToken(kukaParser.COMMA, i);
		}
		public ArrayContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_array; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterArray(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitArray(this);
		}
	}

	public final ArrayContext array() throws RecognitionException {
		ArrayContext _localctx = new ArrayContext(_ctx, getState());
		enterRule(_localctx, 36, RULE_array);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(245); match(BracketOpen);
			setState(256);
			_la = _input.LA(1);
			if (_la==INT) {
				{
				setState(246); match(INT);
				setState(253);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==COMMA) {
					{
					{
					setState(247); match(COMMA);
					setState(249);
					_la = _input.LA(1);
					if (_la==INT) {
						{
						setState(248); match(INT);
						}
					}

					}
					}
					setState(255);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
			}

			setState(258); match(BracketClose);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IdListContext extends ParserRuleContext {
		public List<TerminalNode> COMMA() { return getTokens(kukaParser.COMMA); }
		public List<TerminalNode> ID() { return getTokens(kukaParser.ID); }
		public TerminalNode COMMA(int i) {
			return getToken(kukaParser.COMMA, i);
		}
		public TerminalNode ID(int i) {
			return getToken(kukaParser.ID, i);
		}
		public IdListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_idList; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterIdList(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitIdList(this);
		}
	}

	public final IdListContext idList() throws RecognitionException {
		IdListContext _localctx = new IdListContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_idList);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(260); match(ID);
			setState(265);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(261); match(COMMA);
				setState(262); match(ID);
				}
				}
				setState(267);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ValueContext extends ParserRuleContext {
		public TerminalNode BOOL() { return getToken(kukaParser.BOOL, 0); }
		public TerminalNode ENUM() { return getToken(kukaParser.ENUM, 0); }
		public TerminalNode FLOAT() { return getToken(kukaParser.FLOAT, 0); }
		public TerminalNode INT() { return getToken(kukaParser.INT, 0); }
		public StructureContext structure() {
			return getRuleContext(StructureContext.class,0);
		}
		public TerminalNode STRING() { return getToken(kukaParser.STRING, 0); }
		public TerminalNode BITArr() { return getToken(kukaParser.BITArr, 0); }
		public ValueContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_value; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).enterValue(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof kukaListener ) ((kukaListener)listener).exitValue(this);
		}
	}

	public final ValueContext value() throws RecognitionException {
		ValueContext _localctx = new ValueContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_value);
		int _la;
		try {
			setState(270);
			switch (_input.LA(1)) {
			case BraceOpen:
				enterOuterAlt(_localctx, 1);
				{
				setState(268); structure();
				}
				break;
			case BOOL:
			case ENUM:
			case INT:
			case FLOAT:
			case BITArr:
			case STRING:
				enterOuterAlt(_localctx, 2);
				{
				setState(269);
				_la = _input.LA(1);
				if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << BOOL) | (1L << ENUM) | (1L << INT) | (1L << FLOAT) | (1L << BITArr) | (1L << STRING))) != 0)) ) {
				_errHandler.recoverInline(this);
				}
				consume();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static final String _serializedATN =
		"\3\uacf5\uee8c\u4f5d\u8b0d\u4a45\u78bd\u1b2f\u3378\3#\u0113\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t"+
		"\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\3\2\7\2.\n\2\f\2\16\2\61\13\2"+
		"\3\2\3\2\3\2\7\2\66\n\2\f\2\16\29\13\2\3\2\3\2\5\2=\n\2\3\2\5\2@\n\2\3"+
		"\2\5\2C\n\2\3\2\7\2F\n\2\f\2\16\2I\13\2\3\2\3\2\3\3\3\3\3\3\5\3P\n\3\3"+
		"\4\5\4S\n\4\3\4\3\4\3\4\3\4\3\4\5\4Z\n\4\3\4\5\4]\n\4\3\4\3\4\3\5\3\5"+
		"\3\5\3\5\5\5e\n\5\3\6\5\6h\n\6\3\6\3\6\5\6l\n\6\3\6\3\6\3\6\5\6q\n\6\7"+
		"\6s\n\6\f\6\16\6v\13\6\3\7\3\7\5\7z\n\7\3\7\3\7\5\7~\n\7\3\b\5\b\u0081"+
		"\n\b\3\b\3\b\5\b\u0085\n\b\3\b\3\b\3\b\3\t\5\t\u008b\n\t\3\t\3\t\3\t\3"+
		"\t\3\n\3\n\3\n\3\n\5\n\u0095\n\n\3\n\3\n\3\13\5\13\u009a\n\13\3\13\3\13"+
		"\3\13\3\13\3\f\5\f\u00a1\n\f\3\f\3\f\3\f\3\f\3\r\5\r\u00a8\n\r\3\r\3\r"+
		"\3\r\3\r\3\16\3\16\3\16\5\16\u00b1\n\16\3\16\3\16\3\16\3\16\5\16\u00b7"+
		"\n\16\3\16\7\16\u00ba\n\16\f\16\16\16\u00bd\13\16\3\16\3\16\3\17\3\17"+
		"\3\17\7\17\u00c4\n\17\f\17\16\17\u00c7\13\17\3\20\3\20\3\20\5\20\u00cc"+
		"\n\20\3\20\3\20\3\20\5\20\u00d1\n\20\7\20\u00d3\n\20\f\20\16\20\u00d6"+
		"\13\20\3\21\3\21\3\21\7\21\u00db\n\21\f\21\16\21\u00de\13\21\3\22\3\22"+
		"\5\22\u00e2\n\22\3\22\3\22\3\22\5\22\u00e7\n\22\7\22\u00e9\n\22\f\22\16"+
		"\22\u00ec\13\22\3\22\3\22\3\22\3\23\3\23\3\23\3\23\3\23\5\23\u00f6\n\23"+
		"\3\24\3\24\3\24\3\24\5\24\u00fc\n\24\7\24\u00fe\n\24\f\24\16\24\u0101"+
		"\13\24\5\24\u0103\n\24\3\24\3\24\3\25\3\25\3\25\7\25\u010a\n\25\f\25\16"+
		"\25\u010d\13\25\3\26\3\26\5\26\u0111\n\26\3\26\2\27\2\4\6\b\n\f\16\20"+
		"\22\24\26\30\32\34\36 \"$&(*\2\4\3\2\f\r\4\2\35\35\37#\u012c\2/\3\2\2"+
		"\2\4L\3\2\2\2\6Y\3\2\2\2\bd\3\2\2\2\ng\3\2\2\2\fw\3\2\2\2\16\u0080\3\2"+
		"\2\2\20\u008a\3\2\2\2\22\u0090\3\2\2\2\24\u0099\3\2\2\2\26\u00a0\3\2\2"+
		"\2\30\u00a7\3\2\2\2\32\u00ad\3\2\2\2\34\u00c0\3\2\2\2\36\u00c8\3\2\2\2"+
		" \u00d7\3\2\2\2\"\u00df\3\2\2\2$\u00f0\3\2\2\2&\u00f7\3\2\2\2(\u0106\3"+
		"\2\2\2*\u0110\3\2\2\2,.\7\4\2\2-,\3\2\2\2.\61\3\2\2\2/-\3\2\2\2/\60\3"+
		"\2\2\2\60\62\3\2\2\2\61/\3\2\2\2\62\63\5\4\3\2\63\67\7\5\2\2\64\66\5\6"+
		"\4\2\65\64\3\2\2\2\669\3\2\2\2\67\65\3\2\2\2\678\3\2\2\28:\3\2\2\29\67"+
		"\3\2\2\2:<\7\7\2\2;=\7\3\2\2<;\3\2\2\2<=\3\2\2\2=?\3\2\2\2>@\7\5\2\2?"+
		">\3\2\2\2?@\3\2\2\2@G\3\2\2\2AC\7\3\2\2BA\3\2\2\2BC\3\2\2\2CD\3\2\2\2"+
		"DF\7\5\2\2EB\3\2\2\2FI\3\2\2\2GE\3\2\2\2GH\3\2\2\2HJ\3\2\2\2IG\3\2\2\2"+
		"JK\7\2\2\3K\3\3\2\2\2LM\7\6\2\2MO\7\36\2\2NP\7\n\2\2ON\3\2\2\2OP\3\2\2"+
		"\2P\5\3\2\2\2QS\7\3\2\2RQ\3\2\2\2RS\3\2\2\2SZ\3\2\2\2TZ\5\b\5\2UZ\5\26"+
		"\f\2VZ\5\30\r\2WZ\5\24\13\2XZ\5\22\n\2YR\3\2\2\2YT\3\2\2\2YU\3\2\2\2Y"+
		"V\3\2\2\2YW\3\2\2\2YX\3\2\2\2Z\\\3\2\2\2[]\7\3\2\2\\[\3\2\2\2\\]\3\2\2"+
		"\2]^\3\2\2\2^_\7\5\2\2_\7\3\2\2\2`e\5\n\6\2ae\5\f\7\2be\5\16\b\2ce\5\20"+
		"\t\2d`\3\2\2\2da\3\2\2\2db\3\2\2\2dc\3\2\2\2e\t\3\2\2\2fh\7\t\2\2gf\3"+
		"\2\2\2gh\3\2\2\2hi\3\2\2\2ik\7\36\2\2jl\5&\24\2kj\3\2\2\2kl\3\2\2\2lm"+
		"\3\2\2\2mt\5\f\7\2np\7\32\2\2oq\5\f\7\2po\3\2\2\2pq\3\2\2\2qs\3\2\2\2"+
		"rn\3\2\2\2sv\3\2\2\2tr\3\2\2\2tu\3\2\2\2u\13\3\2\2\2vt\3\2\2\2wy\7\36"+
		"\2\2xz\5&\24\2yx\3\2\2\2yz\3\2\2\2z}\3\2\2\2{|\7\23\2\2|~\5*\26\2}{\3"+
		"\2\2\2}~\3\2\2\2~\r\3\2\2\2\177\u0081\7\t\2\2\u0080\177\3\2\2\2\u0080"+
		"\u0081\3\2\2\2\u0081\u0082\3\2\2\2\u0082\u0084\7\b\2\2\u0083\u0085\7\t"+
		"\2\2\u0084\u0083\3\2\2\2\u0084\u0085\3\2\2\2\u0085\u0086\3\2\2\2\u0086"+
		"\u0087\7\36\2\2\u0087\u0088\5\f\7\2\u0088\17\3\2\2\2\u0089\u008b\7\t\2"+
		"\2\u008a\u0089\3\2\2\2\u008a\u008b\3\2\2\2\u008b\u008c\3\2\2\2\u008c\u008d"+
		"\7\22\2\2\u008d\u008e\7\36\2\2\u008e\u008f\5\f\7\2\u008f\21\3\2\2\2\u0090"+
		"\u0091\7\13\2\2\u0091\u0092\7\36\2\2\u0092\u0094\7\24\2\2\u0093\u0095"+
		"\5\34\17\2\u0094\u0093\3\2\2\2\u0094\u0095\3\2\2\2\u0095\u0096\3\2\2\2"+
		"\u0096\u0097\7\25\2\2\u0097\23\3\2\2\2\u0098\u009a\7\t\2\2\u0099\u0098"+
		"\3\2\2\2\u0099\u009a\3\2\2\2\u009a\u009b\3\2\2\2\u009b\u009c\7\16\2\2"+
		"\u009c\u009d\7\36\2\2\u009d\u009e\5$\23\2\u009e\25\3\2\2\2\u009f\u00a1"+
		"\7\t\2\2\u00a0\u009f\3\2\2\2\u00a0\u00a1\3\2\2\2\u00a1\u00a2\3\2\2\2\u00a2"+
		"\u00a3\7\20\2\2\u00a3\u00a4\7\36\2\2\u00a4\u00a5\5(\25\2\u00a5\27\3\2"+
		"\2\2\u00a6\u00a8\7\t\2\2\u00a7\u00a6\3\2\2\2\u00a7\u00a8\3\2\2\2\u00a8"+
		"\u00a9\3\2\2\2\u00a9\u00aa\7\21\2\2\u00aa\u00ab\7\36\2\2\u00ab\u00ac\5"+
		" \21\2\u00ac\31\3\2\2\2\u00ad\u00ae\7\30\2\2\u00ae\u00b0\7\36\2\2\u00af"+
		"\u00b1\5&\24\2\u00b0\u00af\3\2\2\2\u00b0\u00b1\3\2\2\2\u00b1\u00b2\3\2"+
		"\2\2\u00b2\u00bb\5*\26\2\u00b3\u00b4\7\32\2\2\u00b4\u00b6\7\36\2\2\u00b5"+
		"\u00b7\5&\24\2\u00b6\u00b5\3\2\2\2\u00b6\u00b7\3\2\2\2\u00b7\u00b8\3\2"+
		"\2\2\u00b8\u00ba\5*\26\2\u00b9\u00b3\3\2\2\2\u00ba\u00bd\3\2\2\2\u00bb"+
		"\u00b9\3\2\2\2\u00bb\u00bc\3\2\2\2\u00bc\u00be\3\2\2\2\u00bd\u00bb\3\2"+
		"\2\2\u00be\u00bf\7\31\2\2\u00bf\33\3\2\2\2\u00c0\u00c5\5\"\22\2\u00c1"+
		"\u00c2\7\32\2\2\u00c2\u00c4\5\"\22\2\u00c3\u00c1\3\2\2\2\u00c4\u00c7\3"+
		"\2\2\2\u00c5\u00c3\3\2\2\2\u00c5\u00c6\3\2\2\2\u00c6\35\3\2\2\2\u00c7"+
		"\u00c5\3\2\2\2\u00c8\u00c9\7\36\2\2\u00c9\u00cb\7\36\2\2\u00ca\u00cc\5"+
		"&\24\2\u00cb\u00ca\3\2\2\2\u00cb\u00cc\3\2\2\2\u00cc\u00d4\3\2\2\2\u00cd"+
		"\u00ce\7\32\2\2\u00ce\u00d0\7\36\2\2\u00cf\u00d1\5&\24\2\u00d0\u00cf\3"+
		"\2\2\2\u00d0\u00d1\3\2\2\2\u00d1\u00d3\3\2\2\2\u00d2\u00cd\3\2\2\2\u00d3"+
		"\u00d6\3\2\2\2\u00d4\u00d2\3\2\2\2\u00d4\u00d5\3\2\2\2\u00d5\37\3\2\2"+
		"\2\u00d6\u00d4\3\2\2\2\u00d7\u00dc\5\36\20\2\u00d8\u00d9\7\32\2\2\u00d9"+
		"\u00db\5\36\20\2\u00da\u00d8\3\2\2\2\u00db\u00de\3\2\2\2\u00dc\u00da\3"+
		"\2\2\2\u00dc\u00dd\3\2\2\2\u00dd!\3\2\2\2\u00de\u00dc\3\2\2\2\u00df\u00e1"+
		"\7\36\2\2\u00e0\u00e2\5&\24\2\u00e1\u00e0\3\2\2\2\u00e1\u00e2\3\2\2\2"+
		"\u00e2\u00ea\3\2\2\2\u00e3\u00e4\7\32\2\2\u00e4\u00e6\7\36\2\2\u00e5\u00e7"+
		"\5&\24\2\u00e6\u00e5\3\2\2\2\u00e6\u00e7\3\2\2\2\u00e7\u00e9\3\2\2\2\u00e8"+
		"\u00e3\3\2\2\2\u00e9\u00ec\3\2\2\2\u00ea\u00e8\3\2\2\2\u00ea\u00eb\3\2"+
		"\2\2\u00eb\u00ed\3\2\2\2\u00ec\u00ea\3\2\2\2\u00ed\u00ee\7\33\2\2\u00ee"+
		"\u00ef\t\2\2\2\u00ef#\3\2\2\2\u00f0\u00f1\7\36\2\2\u00f1\u00f5\5&\24\2"+
		"\u00f2\u00f3\7\17\2\2\u00f3\u00f4\7\36\2\2\u00f4\u00f6\5&\24\2\u00f5\u00f2"+
		"\3\2\2\2\u00f5\u00f6\3\2\2\2\u00f6%\3\2\2\2\u00f7\u0102\7\26\2\2\u00f8"+
		"\u00ff\7 \2\2\u00f9\u00fb\7\32\2\2\u00fa\u00fc\7 \2\2\u00fb\u00fa\3\2"+
		"\2\2\u00fb\u00fc\3\2\2\2\u00fc\u00fe\3\2\2\2\u00fd\u00f9\3\2\2\2\u00fe"+
		"\u0101\3\2\2\2\u00ff\u00fd\3\2\2\2\u00ff\u0100\3\2\2\2\u0100\u0103\3\2"+
		"\2\2\u0101\u00ff\3\2\2\2\u0102\u00f8\3\2\2\2\u0102\u0103\3\2\2\2\u0103"+
		"\u0104\3\2\2\2\u0104\u0105\7\27\2\2\u0105\'\3\2\2\2\u0106\u010b\7\36\2"+
		"\2\u0107\u0108\7\32\2\2\u0108\u010a\7\36\2\2\u0109\u0107\3\2\2\2\u010a"+
		"\u010d\3\2\2\2\u010b\u0109\3\2\2\2\u010b\u010c\3\2\2\2\u010c)\3\2\2\2"+
		"\u010d\u010b\3\2\2\2\u010e\u0111\5\32\16\2\u010f\u0111\t\3\2\2\u0110\u010e"+
		"\3\2\2\2\u0110\u010f\3\2\2\2\u0111+\3\2\2\2+/\67<?BGORY\\dgkpty}\u0080"+
		"\u0084\u008a\u0094\u0099\u00a0\u00a7\u00b0\u00b6\u00bb\u00c5\u00cb\u00d0"+
		"\u00d4\u00dc\u00e1\u00e6\u00ea\u00f5\u00fb\u00ff\u0102\u010b\u0110";
	public static final ATN _ATN =
		ATNSimulator.deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}