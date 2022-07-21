using ParseModuleFile.ANTLR.SrcParser;
using ParseModuleFile.ANTLR.SrcParser.Expressions;
using ParseModuleFile.ANTLR.SrcParser.Movements;
using ParseModuleFile.ANTLR.SrcParser.Statements;
using ParseModuleFile.KUKA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR
{
    public class kukaSrcVisitor : kukaSrcBaseVisitor<ISrcItem>
    {
        #region fields
        //private Statements statements = new Statements();
        //private Routine routine;
        private ISrcItem curItem;
        //private Folds foldView = new Folds();
        #endregion fields

        #region methods

        public override ISrcItem VisitChildren(Antlr4.Runtime.Tree.IRuleNode node)
        {
            ISrcItem item = base.VisitChildren(node);
            return item;
        }

        public override ISrcItem VisitEnumElement(kukaSrcParser.EnumElementContext context)
        {
            EnumElement item = new EnumElement(context);
            item.Value = context.IDENTIFIER().GetText().ToUpperInvariant();
            return item;
        }

        public override ISrcItem VisitStructLiteral(kukaSrcParser.StructLiteralContext context)
        {
            StructLiteral item = new StructLiteral(context);
            if (context.typeName() != null)
                item.TypeName = context.typeName().IDENTIFIER().GetText().ToUpperInvariant();
            item.StructElementList = (StructElementList) VisitStructElementList(context.structElementList());
            return item;
        }
        public override ISrcItem VisitStructElementList(kukaSrcParser.StructElementListContext context)
        {
            StructElementList item = new StructElementList(context);
            foreach (var child in context.structElement())
            {
                item.Add((StructElement)VisitStructElement(child));
            }
            return item;
        }

        public override ISrcItem VisitStructElement(kukaSrcParser.StructElementContext context)
        {
            StructElement item = new StructElement(context);
            item.VariableName = (VariableName) VisitVariableName(context.variableName());
            ISrcItem val = VisitUnaryPlusMinusExpression(context.unaryPlusMinusExpression());
            if (val is UnaryOpetatorExpression)
                item.Expression = ((UnaryOpetatorExpression)val).Reduce();
            else item.Expression = (IExpression)val;
            //item.Expression = (UnaryPlusMinusExpression)VisitUnaryPlusMinusExpression(context.unaryPlusMinusExpression());
            return item;
        }

        public override ISrcItem VisitPrimary(kukaSrcParser.PrimaryContext context)
        {
            if (context.parExpression() != null) 
            {
                ParExpression item = new ParExpression(context);
                item.Value = (IAssignmentExpression)VisitAssignmentExpression(context.parExpression().assignmentExpression());
                return item;
            }
            else if (context.variableName().Count() > 0) 
            {
                VariableNameList item = new VariableNameList(context);
                foreach (var child in context.variableName())
                    item.Add((VariableName)this.VisitVariableName(child));
                if (context.arguments() != null)
                {
                    item.HasArguments = true;
                    if (context.arguments().expressionList() != null)
                        item.Arguments = (ExpressionList) VisitExpressionList(context.arguments().expressionList());
                }
                return item;
            }
            else if (context.literal() != null)
            {
                return VisitLiteral(context.literal());
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override ISrcItem VisitExpressionList(kukaSrcParser.ExpressionListContext context)
        {
            ExpressionList item = new ExpressionList(context);
            foreach (var child in context.assignmentExpression())
            {
                item.Add((IAssignmentExpression)VisitAssignmentExpression(child));
            }
            return item;
        }

        public override ISrcItem VisitLiteral(kukaSrcParser.LiteralContext context)
        {
            if (context.INTLITERAL() != null) return new Int(context, context.INTLITERAL().GetText());
            else if (context.FLOATLITERAL() != null) return new Float(context, context.FLOATLITERAL().GetText());
            else if (context.CHARLITERAL() != null) return new ParseModuleFile.ANTLR.SrcParser.Expressions.Char(context, context.CHARLITERAL().GetText());
            else if (context.STRINGLITERAL() != null) return new ParseModuleFile.ANTLR.SrcParser.Expressions.String(context, context.STRINGLITERAL().GetText());
            else if (context.structLiteral() != null) return VisitStructLiteral(context.structLiteral());
            else if (context.TRUE() != null) return new Bool(context, true);
            else if (context.FALSE() != null) return new Bool(context, false);
            else if (context.enumElement() != null) return VisitEnumElement(context.enumElement());
            else
            {
                throw new NotImplementedException();
            }
        }

        public override ISrcItem VisitVariableName(kukaSrcParser.VariableNameContext context)
        {
            VariableName item = new VariableName(context);
            item.Name = context.IDENTIFIER().GetText().ToUpperInvariant();
            if (context.arrayVariableSuffix() != null) item.ArrayVariableSuffix = (ArrayVariableSuffix)VisitArrayVariableSuffix(context.arrayVariableSuffix());
            return item;
        }

        public override ISrcItem VisitArrayVariableSuffix(kukaSrcParser.ArrayVariableSuffixContext context)
        {
            ArrayVariableSuffix item = new ArrayVariableSuffix(context);
            int level = 1;
            int count = context.expression().Count();
            foreach (var child in context.children)
                if (child is Antlr4.Runtime.Tree.TerminalNodeImpl && ((Antlr4.Runtime.Tree.TerminalNodeImpl)child).Payload.Text == ",") level += 1;
            item.Level = level;
            if (count > 0) item.Level1 = (IExpression)VisitExpression(context.expression()[0]);
            if (count > 1) item.Level2 = (IExpression)VisitExpression(context.expression()[1]);
            if (count > 2) item.Level3 = (IExpression)VisitExpression(context.expression()[2]);
            return item;
        }

        public override ISrcItem VisitAssignmentExpression(kukaSrcParser.AssignmentExpressionContext context)
        {
            AssignmentExpression item = new AssignmentExpression(context);
            int count = context.expression().Count();
            if (count > 1)
            {
                item.Declarations = new SrcList<IExpression>(context);
                for (int i = 0; i < count - 1; i++)
                {
                    item.Declarations.Add((IExpression)VisitExpression(context.expression()[i]));
                }
            }
            if (count > 0)
                item.Value = (IExpression)VisitExpression(context.expression().Last());
            return item;
        }

        public override ISrcItem VisitExpression(kukaSrcParser.ExpressionContext context)
        {
            OperatorExpression item = new OperatorExpression(context);
            item.Values = new SrcList<IExpression>(context);
            item.Operators = new SrcList<Operator>(context);
            foreach (var child in context.relationalOp())
            {
                string oper = child.GetText();
                if (oper == "==") item.Operators.Add(Operator.EQUALS);
                else if (oper == "<>") item.Operators.Add(Operator.NOT_EQUAL);
                else if (oper == "<=") item.Operators.Add(Operator.LESS_OR_EQUAL);
                else if (oper == ">=") item.Operators.Add(Operator.GREATER_OR_EQUAL);
                else if (oper == "<") item.Operators.Add(Operator.LESS);
                else if (oper == ">") item.Operators.Add(Operator.GREATER);
                else throw new NotImplementedException();
            }
            foreach (var child in context.conditionalOrExpression())
                item.Values.Add((IExpression)VisitConditionalOrExpression(child));
            return item.Reduce();
        }

        public override ISrcItem VisitConditionalOrExpression(kukaSrcParser.ConditionalOrExpressionContext context)
        {
            OperatorExpression item = new OperatorExpression(context);
            item.Values = new SrcList<IExpression>(context);
            item.Operators = new SrcList<Operator>(context);
            foreach (var child in context.children.OfType<Antlr4.Runtime.Tree.TerminalNodeImpl>())
            {
                int oper = child.Payload.Type;
                if (oper == kukaSrcLexer.OR) item.Operators.Add(Operator.OR);
                else if (oper == kukaSrcLexer.B_OR) item.Operators.Add(Operator.B_OR);
                else throw new NotImplementedException();
            }
            foreach (var child in context.exclusiveOrExpression())
                item.Values.Add((IExpression)VisitExclusiveOrExpression(child));
            return item.Reduce();
        }

        public override ISrcItem VisitExclusiveOrExpression(kukaSrcParser.ExclusiveOrExpressionContext context)
        {
            OperatorExpression item = new OperatorExpression(context);
            item.Values = new SrcList<IExpression>(context);
            item.Operators = new SrcList<Operator>(context);
            //if (context.ChildCount > 1) System.Diagnostics.Debugger.Break();
            foreach (var child in context.children.OfType<Antlr4.Runtime.Tree.TerminalNodeImpl>())
            {
                int oper = child.Payload.Type;
                if (oper == kukaSrcLexer.EXOR) item.Operators.Add(Operator.EXOR);
                else if (oper == kukaSrcLexer.B_EXOR) item.Operators.Add(Operator.B_EXOR);
                else throw new NotImplementedException();
            }
            foreach (var child in context.conditionalAndExpression())
                item.Values.Add((IExpression)VisitConditionalAndExpression(child));
            return item.Reduce();
        }

        public override ISrcItem VisitConditionalAndExpression(kukaSrcParser.ConditionalAndExpressionContext context)
        {
            OperatorExpression item = new OperatorExpression(context);
            item.Values = new SrcList<IExpression>(context);
            item.Operators = new SrcList<Operator>(context);
            foreach (var child in context.children.OfType<Antlr4.Runtime.Tree.TerminalNodeImpl>())
            {
                int oper = child.Payload.Type;
                if (oper == kukaSrcLexer.AND) item.Operators.Add(Operator.AND);
                else if (oper == kukaSrcLexer.B_AND) item.Operators.Add(Operator.B_AND);
                else throw new NotImplementedException();
            }
            foreach (var child in context.additiveExpression())
                item.Values.Add((IExpression)VisitAdditiveExpression(child));
            return item.Reduce();
        }

        public override ISrcItem VisitAdditiveExpression(kukaSrcParser.AdditiveExpressionContext context)
        {
            OperatorExpression item = new OperatorExpression(context);
            item.Values = new SrcList<IExpression>(context);
            item.Operators = new SrcList<Operator>(context);
            foreach (var child in context.children.OfType<Antlr4.Runtime.Tree.TerminalNodeImpl>())
            {
                string oper = child.Payload.Text;
                if (oper == "+") item.Operators.Add(Operator.ADDITION);
                else if (oper == "-") item.Operators.Add(Operator.SUBTRACTION);
                else throw new NotImplementedException();
            }
            foreach (var child in context.multiplicativeExpression())
                item.Values.Add((IExpression)VisitMultiplicativeExpression(child));
            return item.Reduce();
        }
        public override ISrcItem VisitMultiplicativeExpression(kukaSrcParser.MultiplicativeExpressionContext context)
        {
            OperatorExpression item = new OperatorExpression(context);
            item.Values = new SrcList<IExpression>(context);
            item.Operators = new SrcList<Operator>(context);
            foreach (var child in context.children.OfType<Antlr4.Runtime.Tree.TerminalNodeImpl>())
            {
                string oper = child.Payload.Text;
                if (oper == "*") item.Operators.Add(Operator.MULTIPLICATION);
                else if (oper == "/") item.Operators.Add(Operator.DIVISION);
                else throw new NotImplementedException();
            }
            foreach (var child in context.geometricExpression())
                item.Values.Add((IExpression)VisitGeometricExpression(child));
            return item.Reduce();
        }

        public override ISrcItem VisitGeometricExpression(kukaSrcParser.GeometricExpressionContext context)
        {
            OperatorExpression item = new OperatorExpression(context);
            item.Values = new SrcList<IExpression>(context);
            item.Operators = new SrcList<Operator>(context);
            foreach (var child in context.unaryNotExpression())
            {
                item.Operators.Add(Operator.VECTOR_ADD);
                item.Values.Add((IExpression)VisitUnaryNotExpression(child));
            }
            if (item.Operators.Count > 0) item.Operators.Remove(item.Operators.Last());
            return item.Reduce();
        }

        public override ISrcItem VisitUnaryNotExpression(kukaSrcParser.UnaryNotExpressionContext context)
        {
            if (context.NOT() != null)
            {
                UnaryOpetatorExpression item = new UnaryOpetatorExpression(context);
                item.Operator = Operator.NOT;
                ISrcItem val = VisitUnaryNotExpression(context.unaryNotExpression());
                if (val is UnaryOpetatorExpression)
                    item.Value = ((UnaryOpetatorExpression)val).Reduce();
                else item.Value = (IExpression)val;
                return item;
            }
            else if (context.B_NOT() != null)
            {
                UnaryOpetatorExpression item = new UnaryOpetatorExpression(context);
                item.Operator = Operator.B_NOT;
                ISrcItem val = VisitUnaryNotExpression(context.unaryNotExpression());
                if (val is UnaryOpetatorExpression)
                    item.Value = ((UnaryOpetatorExpression)val).Reduce();
                else item.Value = (IExpression)val;
                return item;
            }
            else return VisitUnaryPlusMinusExpression(context.unaryPlusMinusExpression());
        }

        public override ISrcItem VisitUnaryPlusMinusExpression(kukaSrcParser.UnaryPlusMinusExpressionContext context)
        {
            if (context.unaryPlusMinusExpression() != null && context.children.First() is Antlr4.Runtime.Tree.TerminalNodeImpl)
            {
                UnaryOpetatorExpression item = new UnaryOpetatorExpression(context);
                string sign = ((Antlr4.Runtime.Tree.TerminalNodeImpl)context.children.First()).Payload.Text;
                if (sign == "-") item.Operator = Operator.MINUS;
                else if (sign == "+") item.Operator = Operator.PLUS;
                else throw new NotImplementedException();
                ISrcItem val = VisitUnaryPlusMinusExpression(context.unaryPlusMinusExpression());
                if (val is UnaryOpetatorExpression)
                    item.Value = ((UnaryOpetatorExpression)val).Reduce();
                else item.Value = (IExpression)val;
                return item;
            }
            else return VisitPrimary(context.primary());
        }

        public override ISrcItem VisitCaseBlock(kukaSrcParser.CaseBlockContext context)
        {
            SwitchBlockStatementGroup xcase = new SwitchBlockStatementGroup(context, false);
            foreach (var acase in context.caseLabel().expression())
            {
                xcase.Label.Add((IExpression)VisitExpression(acase));
            }
            xcase.StatementList = (StatementList)VisitStatementList(context.statementList());
            return xcase;
        }

        public override ISrcItem VisitDefaultBlock(kukaSrcParser.DefaultBlockContext context)
        {
            SwitchBlockStatementGroup xcase = new SwitchBlockStatementGroup(context, true);
            xcase.StatementList = (StatementList)VisitStatementList(context.statementList());
            return xcase;
        }

        public override ISrcItem VisitSwitchBlockStatementGroups(kukaSrcParser.SwitchBlockStatementGroupsContext context)
        {
            SwitchBlockStatementGroups x = new SwitchBlockStatementGroups(context);
            foreach (var child in context.caseBlock())
            {
                x.Add((SwitchBlockStatementGroup)VisitCaseBlock(child));
            }
            if (context.defaultBlock() != null)
                x.Add((SwitchBlockStatementGroup)VisitDefaultBlock(context.defaultBlock()));
            //return base.VisitSwitchBlockStatementGroups(context);
            return x;
        }

        public override ISrcItem VisitAnalogInputStatement(kukaSrcParser.AnalogInputStatementContext context)
        {
            AnalogInput item = new AnalogInput(context);
            item.On = (context.IDENTIFIER()[0].GetText().ToUpperInvariant() == "ON");
            if (item.On) item.OnOn = (AssignmentExpression)VisitAssignmentExpression(context.assignmentExpression());
            else item.OnOff = context.IDENTIFIER()[1].GetText();
            return item;
        }

        public override ISrcItem VisitAnalogOutputStatement(kukaSrcParser.AnalogOutputStatementContext context)
        {
            AnalogOutput item = new AnalogOutput(context);
            item.On = (context.IDENTIFIER()[0].GetText().ToUpperInvariant() == "ON");
            if (item.On)
            {
                item.OnOn = (AssignmentExpression)VisitAssignmentExpression(context.assignmentExpression());
                int i = 0;
                foreach (var child in context.IDENTIFIER())
                {
                    if (!item.UsesDelay && child.GetText().ToUpperInvariant() == "DELAY" && context.literal().Count() > i)
                    {
                        item.UsesDelay = true;
                        item.Delay = (Literal)VisitLiteral(context.literal()[i]);
                        i++;
                    }
                    else if (!item.UsesMinimum && child.GetText().ToUpperInvariant() == "MINIMUM" && context.literal().Count() > i)
                    {
                        item.UsesMinimum = true;
                        item.Minimum = (Literal)VisitLiteral(context.literal()[i]);
                        i++;
                    }
                    else if (!item.UsesMaximum && child.GetText().ToUpperInvariant() == "MAXIMUM" && context.literal().Count() > i)
                    {
                        item.UsesMaximum = true;
                        item.Maximum = (Literal)VisitLiteral(context.literal()[i]);
                        i++;
                    }
                }
            }
            else item.OnOff = context.IDENTIFIER()[1].GetText();

            return item;
        }

        public override ISrcItem VisitStatementList(kukaSrcParser.StatementListContext context)
        {
            StatementList statements = new StatementList(context);
            foreach (var child in context.statement())
            {
                try
                {
                    statements.Add((SrcItem)VisitStatement(child));
                }
                catch (Exception ex)
                {
                    Exception d = ex;
                    System.Diagnostics.Debugger.Break();
                }
            }
            return statements;
        }

        public override ISrcItem VisitStatement(kukaSrcParser.StatementContext context)
        {
            if (context.CONTINUE() != null) return new Statement(context.Start.Line, StatementType.CONTINUE);
            else if (context.EXIT() != null) return new Statement(context.Start.Line, StatementType.EXIT);
            else if (context.FOR() != null && context.WAIT() == null)
            {
                For loop = new For(context.Start.Line, context.Stop.Line);
                loop.Identifier = context.IDENTIFIER().First().GetText();
                loop.From = (IExpression)VisitExpression(context.expression()[0]);
                loop.To = (IExpression)VisitExpression(context.expression()[1]);
                if (context.IDENTIFIER().Count() == 2 && context.IDENTIFIER()[1].GetText().ToUpperInvariant() == "STEP")
                    loop.Step = (IExpression)VisitExpression(context.expression()[2]);
                loop.StatementList = (StatementList)VisitStatementList(context.statementList().First());
                return loop;
            }
            else if (context.GOTO() != null) return new Goto(context, context.IDENTIFIER().First().GetText());
            else if (context.HALT() != null) return new Statement(context, StatementType.HALT);
            else if (context.IF() != null)
            {
                If ifStat = new If(context);
                try
                {
                    ifStat.Condition = (IExpression)VisitExpression(context.expression()[0]);
                }
                catch
                {
                    System.Diagnostics.Debugger.Break();
                }
                ifStat.IfTrue = (StatementList)VisitStatementList(context.statementList()[0]);
                if (context.ELSE() != null)
                    ifStat.IfFalse = (StatementList)VisitStatementList(context.statementList()[1]);
                return ifStat;
            }
            else if (context.LOOP() != null && context.ENDLOOP() != null)
            {
                Loop loop = new Loop(context);
                loop.StatementList = (StatementList)VisitStatementList(context.statementList().First());
                return loop;
            }
            else if (context.REPEAT() != null)
            {
                Repeat loop = new Repeat(context);
                loop.Condition = (IExpression)VisitExpression(context.expression()[0]);
                loop.StatementList = (StatementList)VisitStatementList(context.statementList().First());
                return loop;
            }
            else if (context.SWITCH() != null) {
                Switch item = new Switch(context);
                item.Condition = (IExpression)VisitExpression(context.expression()[0]);
                item.Cases = (SwitchBlockStatementGroups)VisitSwitchBlockStatementGroups(context.switchBlockStatementGroups());
                return item;
            }
            else if (context.WAIT() != null)
            {
                if (context.FOR() != null) return new WaitFor(context, (IExpression)VisitExpression(context.expression().First()));
                else if (context.SEC() != null) return new WaitSec(context, (IExpression)VisitExpression(context.expression().First()));
                else throw new NotImplementedException();
            }
            else if (context.WHILE() != null)
            {
                While loop = new While(context);
                loop.Condition = (IExpression)VisitExpression(context.expression()[0]);
                loop.StatementList = (StatementList)VisitStatementList(context.statementList().First());
                return loop;
            }
            else if (context.RETURN() != null)
            {
                Return item = new Return(context.Start.Line);
                if (context.assignmentExpression() != null)
                    item.Expression = (IAssignmentExpression)VisitAssignmentExpression(context.assignmentExpression());
                return item;
            }
            else if (context.BRAKE() != null) return new Brake(context, context.IDENTIFIER().Count() > 0);
            else if (context.ChildCount == 2 && context.assignmentExpression() != null && context.NEWLINE() != null) {
                return VisitAssignmentExpression(context.assignmentExpression());
            }
            else if (context.COLON() != null) return new Label(context, context.IDENTIFIER().First().GetText());
            else if (context.ChildCount == 1 && context.NEWLINE() != null) return new Statement(context, StatementType.EMPTY);
            else if (context.INTERRUPT() != null)
            {
                if (context.DECL() != null) 
                {
                    InterruptDecl item = new InterruptDecl(context);
                    item.Global = (context.GLOBAL() != null);
                    item.Action = (IAssignmentExpression)VisitAssignmentExpression(context.assignmentExpression());
                    item.Priority = (IPrimary)VisitPrimary(context.primary());
                    item.Condition = (IExpression)VisitExpression(context.expression().First());
                    return item;
                }
                else 
                {
                    Interrupt item = new Interrupt(context);
                    item.Action = context.IDENTIFIER().First().GetText().ToUpperInvariant();
                    if (context.primary() != null)
                        item.Priority = (IPrimary)VisitPrimary(context.primary());
                    return item;
                }
            }
            else if (context.PTP() != null)
            {
                PTP item = new PTP(context);
                if (context.C_PTP() != null) item.UsesCPTP = true;
                if (context.C_DIS().Count() > 0) item.Approximation = Approximation.C_DIS;
                else if (context.C_ORI() != null) item.Approximation = Approximation.C_ORI;
                else if (context.C_VEL() != null) item.Approximation = Approximation.C_VEL;
                item.Target = (IGeometricExpression)VisitGeometricExpression(context.geometricExpression().First());
                return item;
            }
            else if (context.PTP_REL() != null)
            {
                PTP_REL item = new PTP_REL(context.Start.Line);
                if (context.C_PTP() != null) item.UsesCPTP = true;
                if (context.C_DIS().Count() > 0) item.Approximation = Approximation.C_DIS;
                else if (context.C_ORI() != null) item.Approximation = Approximation.C_ORI;
                else if (context.C_VEL() != null) item.Approximation = Approximation.C_VEL;
                item.Target = (IGeometricExpression)VisitGeometricExpression(context.geometricExpression().First());
                return item;
            }
            else if (context.LIN() != null)
            {
                LIN item = new LIN(context.Start.Line);
                if (context.C_DIS().Count() > 1) item.UsesSecondCDIS = true;
                if (context.C_DIS().Count() > 0) item.Approximation = Approximation.C_DIS;
                else if (context.C_ORI() != null) item.Approximation = Approximation.C_ORI;
                else if (context.C_VEL() != null) item.Approximation = Approximation.C_VEL;
                item.Target = (IGeometricExpression)VisitGeometricExpression(context.geometricExpression().First());
                return item;
            }
            else if (context.LIN_REL() != null)
            {
                LIN_REL item = new LIN_REL(context.Start.Line);
                if (context.C_DIS().Count() > 0) item.Approximation = Approximation.C_DIS;
                else if (context.C_ORI() != null) item.Approximation = Approximation.C_ORI;
                else if (context.C_VEL() != null) item.Approximation = Approximation.C_VEL;
                item.Target = (IGeometricExpression)VisitGeometricExpression(context.geometricExpression().First());
                item.EnumElement = (EnumElement)VisitEnumElement(context.enumElement());
                return item;
            }
            else if (context.CIRC() != null) 
            {
                CIRC item = new CIRC(context.Start.Line);
                item.AuxiliaryPoint = (IGeometricExpression)VisitGeometricExpression(context.geometricExpression()[0]);
                item.Target = (IGeometricExpression)VisitGeometricExpression(context.geometricExpression()[1]);
                if (context.C_DIS().Count() > 0) item.Approximation = Approximation.C_DIS;
                else if (context.C_ORI() != null) item.Approximation = Approximation.C_ORI;
                else if (context.C_VEL() != null) item.Approximation = Approximation.C_VEL;
                if (context.IDENTIFIER().Length > 0 && context.IDENTIFIER().First().GetText().ToUpperInvariant() == "CA")
                {
                    item.UsesCircularAngle = true;
                    item.CAValue = (IPrimary)VisitPrimary(context.primary());
                }
                return item;
            }
            else if (context.CIRC_REL() != null)
            {
                CIRC_REL item = new CIRC_REL(context.Start.Line);
                item.AuxiliaryPoint = (IGeometricExpression)VisitGeometricExpression(context.geometricExpression()[0]);
                item.Target = (IGeometricExpression)VisitGeometricExpression(context.geometricExpression()[1]);
                if (context.C_DIS().Count() > 0) item.Approximation = Approximation.C_DIS;
                else if (context.C_ORI() != null) item.Approximation = Approximation.C_ORI;
                else if (context.C_VEL() != null) item.Approximation = Approximation.C_VEL;
                if (context.IDENTIFIER().First().GetText().ToUpperInvariant() == "CA")
                {
                    item.UsesCircularAngle = true;
                    item.CAValue = (IPrimary)VisitPrimary(context.primary());
                }
                return item;
            }
            else if (context.TRIGGER() != null) 
            {
                Trigger item = new Trigger(context.Start.Line);
                item.AtEnd = (IExpression)VisitExpression(context.expression()[0]);
                item.TimeDelay = (IExpression)VisitExpression(context.expression()[1]);
                if (context.expression().Count() > 2)
                {
                    item.WithPriority = true;
                    item.Priority = (IExpression)VisitExpression(context.expression()[2]);
                }
                return item;
            }
            else if (context.analogInputStatement() != null) return VisitAnalogInputStatement(context.analogInputStatement());
            else if (context.analogOutputStatement() != null) return VisitAnalogOutputStatement(context.analogOutputStatement());
            else
                throw new NotImplementedException();
        }

        public override ISrcItem VisitRoutineDataSection(kukaSrcParser.RoutineDataSectionContext context)
        {
            return base.VisitRoutineDataSection(context);
        }

        public override ISrcItem VisitRoutineImplementationSection(kukaSrcParser.RoutineImplementationSectionContext context)
        {
            return VisitStatementList(context.statementList());
        }

        public override ISrcItem VisitProcedureDefinition(kukaSrcParser.ProcedureDefinitionContext context)
        {
            ProcedureDefinition item = new ProcedureDefinition(context.Start.Line);
            item.IsGlobal = (context.GLOBAL() != null);
            item.Name = context.procedureName().GetText();
            item.FormalParameters = null;
            item.DataSection = null;
            //TODO: Formalparameters
            //TODO: DataSecion
            item.StatementList = (StatementList)VisitRoutineImplementationSection(context.routineBody().routineImplementationSection());
            VisitChildren(context.routineBody());
            curItem = item;
            return item;
        }
        #endregion methods
    }
}
