lexer grammar ExprLexer;

COMMA : ',' ;
LPAREN : '(' ;
RPAREN : ')' ;

INT : [0-9]+ ;
ID: [a-zA-Z_][a-zA-Z_0-9]* ;
WS: [ \t\n\r\f]+ -> skip ;
STRING : '"' ~('\r' | '\n' | '"')* '"' ;