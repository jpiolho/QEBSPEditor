

export function monacoSetCursor(editor, row, col) {
    editor.setPosition({
        column: col,
        lineNumber: row
    });
    
    editor.revealLine(row);
    editor.focus();
}

export function monacoInitialize() {
    return new Promise((resolve, reject) => {
        require(["vs/editor/editor.main"], async () => {
            monaco.languages.register({
                id: 'bspent'
            });
            monaco.languages.setMonarchTokensProvider('bspent', {
                // Set defaultToken to invalid to see what you do not tokenize yet
                //defaultToken: 'invalid',
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
                        [/[ \t\r\n]+/, ''],
                        [/\/\/.*$/, 'comment']
                    ],

                    key: [
                        [/[^\\"]+/, 'type'],
                        [/\\"/, 'type.escape'],
                        [/"/, 'type', '@prevalue']
                    ],

                    prevalue: [
                        { include: "whitespace" },
                        [/"/, 'string', '@value'],

                        // Return if there's missing a value
                        [/[{}]/, '@brackets', '@pop']
                    ],

                    value: [
                        [/[^\\"]+/, 'string'],
                        [/\\"/, 'string.escape'],
                        [/"/, 'string', '@popall']
                    ]
                }
            });
            resolve();
        });
    });
}

export function monacoSetModelCode(model, code) {
    model.surpressOnChange = true;
    model.setValue(code);
    model.surpressOnChange = false;
}

export function monacoHookupModel(csharp, model) {
    model.onDidChangeContent(async (e) => {
        if (model.surpressOnChange)
            return;

        await csharp.invokeMethodAsync("JSSetCode", model.getValue());
    });
}