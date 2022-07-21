// Generated from C:\Users\lholetzke\Documents\Visual Studio 2012\Projects\KUKA_ParseModuleFileCSharp\KUKA_ParseModuleFile\ANTLR\kuka.g4 by ANTLR 4.1
import org.antlr.v4.runtime.misc.NotNull;
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link kukaParser}.
 */
public interface kukaListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link kukaParser#structure}.
	 * @param ctx the parse tree
	 */
	void enterStructure(@NotNull kukaParser.StructureContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#structure}.
	 * @param ctx the parse tree
	 */
	void exitStructure(@NotNull kukaParser.StructureContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#assign}.
	 * @param ctx the parse tree
	 */
	void enterAssign(@NotNull kukaParser.AssignContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#assign}.
	 * @param ctx the parse tree
	 */
	void exitAssign(@NotNull kukaParser.AssignContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#myenum}.
	 * @param ctx the parse tree
	 */
	void enterMyenum(@NotNull kukaParser.MyenumContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#myenum}.
	 * @param ctx the parse tree
	 */
	void exitMyenum(@NotNull kukaParser.MyenumContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#data}.
	 * @param ctx the parse tree
	 */
	void enterData(@NotNull kukaParser.DataContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#data}.
	 * @param ctx the parse tree
	 */
	void exitData(@NotNull kukaParser.DataContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#line}.
	 * @param ctx the parse tree
	 */
	void enterLine(@NotNull kukaParser.LineContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#line}.
	 * @param ctx the parse tree
	 */
	void exitLine(@NotNull kukaParser.LineContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#signal}.
	 * @param ctx the parse tree
	 */
	void enterSignal(@NotNull kukaParser.SignalContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#signal}.
	 * @param ctx the parse tree
	 */
	void exitSignal(@NotNull kukaParser.SignalContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#signalRange}.
	 * @param ctx the parse tree
	 */
	void enterSignalRange(@NotNull kukaParser.SignalRangeContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#signalRange}.
	 * @param ctx the parse tree
	 */
	void exitSignalRange(@NotNull kukaParser.SignalRangeContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#params}.
	 * @param ctx the parse tree
	 */
	void enterParams(@NotNull kukaParser.ParamsContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#params}.
	 * @param ctx the parse tree
	 */
	void exitParams(@NotNull kukaParser.ParamsContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#idList}.
	 * @param ctx the parse tree
	 */
	void enterIdList(@NotNull kukaParser.IdListContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#idList}.
	 * @param ctx the parse tree
	 */
	void exitIdList(@NotNull kukaParser.IdListContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#sparamList}.
	 * @param ctx the parse tree
	 */
	void enterSparamList(@NotNull kukaParser.SparamListContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#sparamList}.
	 * @param ctx the parse tree
	 */
	void exitSparamList(@NotNull kukaParser.SparamListContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#ext}.
	 * @param ctx the parse tree
	 */
	void enterExt(@NotNull kukaParser.ExtContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#ext}.
	 * @param ctx the parse tree
	 */
	void exitExt(@NotNull kukaParser.ExtContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#prog}.
	 * @param ctx the parse tree
	 */
	void enterProg(@NotNull kukaParser.ProgContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#prog}.
	 * @param ctx the parse tree
	 */
	void exitProg(@NotNull kukaParser.ProgContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#noDecl}.
	 * @param ctx the parse tree
	 */
	void enterNoDecl(@NotNull kukaParser.NoDeclContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#noDecl}.
	 * @param ctx the parse tree
	 */
	void exitNoDecl(@NotNull kukaParser.NoDeclContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#xconst}.
	 * @param ctx the parse tree
	 */
	void enterXconst(@NotNull kukaParser.XconstContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#xconst}.
	 * @param ctx the parse tree
	 */
	void exitXconst(@NotNull kukaParser.XconstContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#mystruc}.
	 * @param ctx the parse tree
	 */
	void enterMystruc(@NotNull kukaParser.MystrucContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#mystruc}.
	 * @param ctx the parse tree
	 */
	void exitMystruc(@NotNull kukaParser.MystrucContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#parameterList}.
	 * @param ctx the parse tree
	 */
	void enterParameterList(@NotNull kukaParser.ParameterListContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#parameterList}.
	 * @param ctx the parse tree
	 */
	void exitParameterList(@NotNull kukaParser.ParameterListContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#value}.
	 * @param ctx the parse tree
	 */
	void enterValue(@NotNull kukaParser.ValueContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#value}.
	 * @param ctx the parse tree
	 */
	void exitValue(@NotNull kukaParser.ValueContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#parameters}.
	 * @param ctx the parse tree
	 */
	void enterParameters(@NotNull kukaParser.ParametersContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#parameters}.
	 * @param ctx the parse tree
	 */
	void exitParameters(@NotNull kukaParser.ParametersContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#defdat}.
	 * @param ctx the parse tree
	 */
	void enterDefdat(@NotNull kukaParser.DefdatContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#defdat}.
	 * @param ctx the parse tree
	 */
	void exitDefdat(@NotNull kukaParser.DefdatContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#array}.
	 * @param ctx the parse tree
	 */
	void enterArray(@NotNull kukaParser.ArrayContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#array}.
	 * @param ctx the parse tree
	 */
	void exitArray(@NotNull kukaParser.ArrayContext ctx);

	/**
	 * Enter a parse tree produced by {@link kukaParser#decl}.
	 * @param ctx the parse tree
	 */
	void enterDecl(@NotNull kukaParser.DeclContext ctx);
	/**
	 * Exit a parse tree produced by {@link kukaParser#decl}.
	 * @param ctx the parse tree
	 */
	void exitDecl(@NotNull kukaParser.DeclContext ctx);
}