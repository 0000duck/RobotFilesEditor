using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public enum StatementType
    {
        CONTINUE,
        EXIT,
        FOR,
        GOTO,
        HALT,
        IF,
        LOOP,
        REPEAT,
        SWITCH,
        SWITCH_CASE,
        WAIT_FOR,
        WAIT_SEC,
        WHILE,
        RETURN,
        BRAKE,
        assignmentExpression,
        LABEL,
        EMPTY,
        INTERRUPT_DECL,
        INTERRUPT,
        PTP,
        PTP_REL,
        LIN,
        LIN_REL,
        CIRC,
        CIRC_REL,
        TRIGGER,
        analogInputStatement,
        analogOutputStatement
    }
}
