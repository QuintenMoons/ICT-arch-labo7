parser grammar ExprParser;
options { tokenVocab=ExprLexer; }

expr: plainint // odd name to avoid collisions
    | func
    | reference
    | plainstring
    ;

func : ID LPAREN arglist RPAREN ;
reference : ID ;
plainint : INT;
plainstring : STRING;
arglist :  ( expr (',' expr)* )?;