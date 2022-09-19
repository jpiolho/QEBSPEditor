export function monacoSetCode(editor, code) {
    editor.csharp.changing = true;
    editor.setValue(code);
    editor.csharp.changing = false;
}

export function monacoSetCursor(editor, row, col) {
    editor.setPosition({
        column: col,
        lineNumber: row
    });
    
    editor.revealLine(row);

    /*
    editor.setSelection({
        startLineNumber: row,
        endLineNumber: row,
        startColumn: col,
        endColumn: col+1
    });
    */

    editor.focus();
}

export function monacoCreate(csharp, container) {
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
                        [/[ \t\r\n]+/, '']
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

            let editor = monaco.editor.create(container, {
                value: await csharp.invokeMethodAsync("JSGetCode"),
                language: 'bspent',
                theme: 'vs',
                automaticLayout: true
            });

            editor.onDidChangeModelContent(async function (e) {
                if (editor.csharp.changing)
                    return;

                await csharp.invokeMethodAsync("JSSetCode", editor.getValue());
            });



            editor.csharp = {};
            editor.csharp.reference = csharp;

            resolve(editor);
        });
    });
}