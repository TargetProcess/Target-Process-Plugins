// New lexer shoud be generated after update with target CSharp and namespace Tp.Core.Expressions.Parsing
// by JetBrains plugin(https://plugins.jetbrains.com/plugin/7358-antlr-v4-grammar-plugin) or antlr tool.
// Documentation about tool could be found here: https://github.com/antlr/antlr4/blob/master/doc/getting-started.md

lexer grammar ExpressionLexer;

COMMA: ',';
QUESTION: '?';
COLON: ':';
AMPERSAND: '&';
LOGICAL_AND: [aA][nN][dD] | '&&';
LOGICAL_OR: '||' | [oO][rR];
LOGICAL_NOT: [nN][oO][tT] | '!';
IN: [iI][nN];
GREATER: '>';
GREATER_OR_EQUAL: '>=';
LESS: '<';
LESS_OR_EQUAL: '<=';
NOT_EQUAL: '!=' | '<>' ;
EQUAL: '==' | '=' ;
PAREN_OPEN : '(' ;
PAREN_CLOSE : ')' ;
CURLY_OPEN: '{';
CURLY_CLOSE: '}';
SQUARE_OPEN : '[' ;
SQUARE_CLOSE : ']' ;
DOT : '.' ;
MULT: '*' ;
DIV: '/' ;
PLUS: '+' ;
MINUS: '-' ;
TRUE: [tT][rR][uU][eE];
FALSE: [fF][aA][lL][sS][eE];
NULL: [nN][uU][lL][lL];
AS: [aA][sS];
NEW : [nN] [eE] [wW] ;
ASC: [aA][sS][cC]([eE][nN][dD][iI][nN][gG])?;
DESC: [dD][eE][sS][cC]([eE][nN][dD][iI][nN][gG])?;
INTEGER_NUMBER : DIGIT+ ;
FLOAT_NUMBER : DIGIT+ (DOT DIGIT+)? EXPONENT? [fF]? ;
STRING : '"' ('\\"' | ~["\r\n])* '"' | '\'' ('\\\'' | ~['\r\n])* '\'';
ALPHANUMERIC_IDENTIFIER : (ALPHA|'_') (ALPHA | DIGIT|'_')* ;
fragment ALPHA
  : 'A'..'Z'
  | 'a'..'z'
  | '\u00C0'..'\u00D6'
  | '\u00D8'..'\u00F6'
  | '\u00F8'..'\u02FF'
  | '\u0370'..'\u037D'
  | '\u037F'..'\u1FFF'
  | '\u200C'..'\u200D'
  | '\u2070'..'\u218F'
  | '\u2C00'..'\u2FEF'
  | '\u3001'..'\uD7FF'
  | '\uF900'..'\uFDCF'
  | '\uFDF0'..'\uFFFD';

fragment SIGN: '-' ;

fragment
EXPONENT: ('e'|'E') SIGN? DIGIT+;

fragment DIGIT : [0-9] ;
WS : [ \t\r\n]+ -> skip;
