CKEDITOR.plugins.add('umbracosave', {
    icons: 'umbracosave',
    hidpi: true,
    init: function (editor) {
        editor.addCommand('umbracosave',
            new CKEDITOR.dialogCommand('umbracosaveDialog', {
                allowedContent: 'img[*]',
                requiredContent: 'img'
            })
        );
        editor.ui.addButton('umbracosave', {
            //label: editor.lang.umbracosave.tooltip,
            label: "Save content in Umbraco",
            command: 'umbracosave',
            toolbar: 'insert,0'
        });
        CKEDITOR.dialog.add('umbracosaveDialog', this.path + 'dialogs/umbracosave.js')
    }
});