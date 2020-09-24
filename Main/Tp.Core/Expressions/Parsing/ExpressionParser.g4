// New lexer shoud be generated after update with target CSharp and namespace Tp.Core.Expressions.Parsing
// by JetBrains plugin(https://plugins.jetbrains.com/plugin/7358-antlr-v4-grammar-plugin) or antlr tool.
// Documentation about tool could be found here: https://github.com/antlr/antlr4/blob/master/doc/getting-started.md

parser grammar ExpressionParser;

options {
  tokenVocab = ExpressionLexer;
}

program : expression EOF ;

expression
  : PAREN_OPEN expression PAREN_CLOSE #parenthesis
  | objectExpr #object
  | target=expression DOT fieldName=fieldNameExpr #fieldAccess
  | fieldName=fieldNameExpr #fieldAccess
  | functionName=fieldNameExpr PAREN_OPEN arguments? PAREN_CLOSE #call
  | target=expression DOT functionName=fieldNameExpr PAREN_OPEN arguments? PAREN_CLOSE #call
  | target=expression SQUARE_OPEN index=literalExpr SQUARE_CLOSE #indexer
  | target=expression DOT AS LESS fieldNameExpr GREATER #cast
  | MINUS right=expression #unary
  | LOGICAL_NOT right=expression #unary
  | left=expression (MULT | DIV) right=expression #binary
  | left=expression (PLUS | MINUS) right=expression #binary
  | left=expression AMPERSAND right=expression #binary
  | left=expression IN SQUARE_OPEN arguments? SQUARE_CLOSE #relational
  | left=expression GREATER right=expression #relational
  | left=expression GREATER_OR_EQUAL right=expression #relational
  | left=expression LESS right=expression #relational
  | left=expression LESS_OR_EQUAL right=expression #relational
  | left=expression EQUAL right=expression #relational
  | left=expression NOT_EQUAL right=expression #relational
  | left=expression LOGICAL_AND right=expression #logical
  | left=expression LOGICAL_OR right=expression #logical
  | cond=expression QUESTION left=expression COLON right=expression #conditional
  | value=literalExpr #constant
  ;

objectExpr
  : CURLY_OPEN objectPropertyExpr (COMMA objectPropertyExpr)* CURLY_CLOSE
  | CURLY_OPEN CURLY_CLOSE
  | NEW PAREN_OPEN objectPropertyExpr (COMMA objectPropertyExpr)* PAREN_CLOSE
  ;

objectPropertyExpr
  : expression
  | alias=ALPHANUMERIC_IDENTIFIER ':' expression
  | expression AS alias=ALPHANUMERIC_IDENTIFIER
  ;

literalExpr
  : INTEGER_NUMBER
  | FLOAT_NUMBER
  | STRING
  | TRUE
  | FALSE
  | NULL
  ;

arguments
    : expression (COMMA expression)*
    ;

fieldNameExpr
  : name=ALPHANUMERIC_IDENTIFIER
  ;

// this rule used for vistior case generation, because expression case isn't generated for some reason
expressionContainer : expression;

orderingProgram
  : ordering (COMMA ordering)* EOF;

ordering
  : selector=expressionContainer (ASC|DESC)?
  ;
