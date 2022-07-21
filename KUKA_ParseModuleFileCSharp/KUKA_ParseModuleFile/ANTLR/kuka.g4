/*
 * Reads a KUKA dat file.
 * Author: Lukasz Holetzke (lukasz.holetzke@gmail.com)
 * Licence: BSD
 */

grammar kuka;


/*
 *  Parser:
 */
prog: 
        FILEATTRIBUTES* 
        defdat NEWLINE
        line*
        ENDDAT COMMENT? NEWLINE?
        
        (COMMENT? NEWLINE)*
        EOF ;

defdat : DEFDAT ID PUBLIC?;
line :   (
          COMMENT?
          | data
          | myenum
          | mystruc
          | signal
          | mychan
          | ext) 
        COMMENT? NEWLINE;

data    : noDecl | assign | decl | xconst;
noDecl  : GLOBAL? id array? assign (COMMA assign?)*;
assign  : id array? (ASSIGMENT value)?;
decl    : GLOBAL? DECL GLOBAL? id assign;
xconst  : GLOBAL? XCONST id assign;

ext     : EXT id RBracketOpen parameterList? RBracketClose ;
signal  : GLOBAL? SIGNAL id signalRange;
myenum  : GLOBAL? ENUMD id idList;
mystruc : GLOBAL? STRUCD id sparamList;
mychan  : CHANNEL COLON id COLON id id;

structure     : BraceOpen id array? value (COMMA id array? value)* BraceClose ;
parameterList : parameters (COMMA parameters)* ;
params        : id id array? (COMMA id array?)*;
sparamList    : params (COMMA params)* ;
parameters    : id array? (COMMA id array?)* COLON (IN|OUT);
signalRange   : id array (TO id array)?;
array         : BracketOpen (INT (COMMA INT?)*)? BracketClose;
idList        : id (COMMA id)* ;
id			  : ID | OUT;

value : structure
      | ( BOOL
          | ENUM
          | INT
          | FLOAT
          | BITArr
          | STRING)
      ;

/*
 *  Lexer:
 */
COMMENT         : ';' ~('\n'|'\r')* -> channel(HIDDEN);
FILEATTRIBUTES  : '&' ~('\n'|'\r')* NEWLINE ;
NEWLINE         : ('\r'? '\n')+;

// Keywords:
DEFDAT  : 'DEFDAT';
ENDDAT  : 'ENDDAT';
DECL    : 'DECL';
GLOBAL  : 'GLOBAL';
PUBLIC  : 'PUBLIC'|'Public';
EXT     : 'EXT';
SIGNAL  : 'SIGNAL';
TO      : 'TO';
ENUMD   : 'ENUM';
STRUCD  : 'STRUC';
XCONST  : 'CONST';
CHANNEL : 'CHANNEL';

ASSIGMENT     : '=';
RBracketOpen  : '(';
RBracketClose : ')';
BracketOpen   : '[';
BracketClose  : ']';
BraceOpen     : '{';
BraceClose    : '}';
COMMA         : ',';
COLON         : ':';
IN           : 'IN';
OUT           : 'OUT';

WS    : [ \t]+ -> skip ;
BOOL  : True | False;
ID    : [a-zA-Z_$] [a-zA-Z0-9_$]*;

// Datatypes:
ENUM   : '#' ID;
INT    : '-'?[0-9]+ ;
FLOAT  :   '-'? (
             [0-9]+ '.' [0-9]* EXPONENT?
             | '.' [0-9]+ EXPONENT?
             | [0-9]+ EXPONENT
                ) ;
BITArr  :  '\'B' [01]+ '\'';
STRING :  '"' ( ~('\\'|'"') )* '"' ;

fragment EXPONENT : ('e'|'E') ('+'|'-')? ('0'..'9')+ ;
fragment True     : [Tt][Rr][Uu][Ee];
fragment False    : [Ff][Aa][Ll][Ss][Ee];
