grammar BSPEnt;


file: entity* EOF;

entity: OPEN_BRACKETS keyvalue*? CLOSE_BRACKETS;

keyvalue: key value;
key: STRING;
value: STRING;

STRING: '"' ('\\"'|.)*? '"';
OPEN_BRACKETS: '{';
CLOSE_BRACKETS: '}';
TEXT: [a-zA-Z0-9]+;
COMMENT: '//' ~[\r\n]* -> skip;
WHITESPACE: [ \t\r\n]+ -> skip;

