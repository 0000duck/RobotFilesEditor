using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public enum Operator
    {
        // Arithmetic operators
        ADDITION,
        SUBTRACTION,
        DIVISION,
        MULTIPLICATION,
        // Logic operators
        NOT,
        AND,
        OR,
        EXOR,
        // Relational operators
        EQUALS,
        LESS,
        GREATER,
        LESS_OR_EQUAL,
        GREATER_OR_EQUAL,
        NOT_EQUAL,
        // Bit operators
        B_NOT,
        B_AND,
        B_OR,
        B_EXOR,
        // Geometric operators
        VECTOR_ADD,
        // Inversing additive operators
        PLUS,
        MINUS,
    }
}
