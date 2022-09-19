// Create your own language definition here
// You can safely look at other samples without losing modifications.
// Modifications are not saved on browser refresh/close though -- copy often!
return {
  // Set defaultToken to invalid to see what you do not tokenize yet
  defaultToken: 'invalid',
  tokenPostfix: '.ent',

  // The main tokenizer for our languages
  tokenizer: {
    root: [
      { include: '@whitespace' },

      [/[{}]/, '@brackets'],
      
      [/"([^"\\]|\\.)*$/, 'string.invalid'],
      [/"/, 'type', '@key']
    ],


    whitespace: [
      [/[ \t\r\n]+/, '']
    ],

    key: [
      [ /[^\\"]+/, 'type' ],
      [/\\"/, 'type.escape'],
      [ /"/, 'type', '@prevalue']
    ],

    prevalue: [
      { include: "whitespace" },
      [ /"/, 'string', '@value' ],

      // Return if there's missing a value
      [/[{}]/, '@brackets', '@pop']
    ],

    value: [
      [ /[^\\"]+/, 'string'],
      [/\\"/, 'string.escape'],
      [ /"/, 'string', '@popall']
    ]
  }
};