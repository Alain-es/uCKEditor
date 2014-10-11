CKEDITOR.plugins.add('umbracomedia', {
    icons: 'umbracomedia',
    hidpi: true,
    init: function (editor) {
        editor.addCommand('umbracomedia',
            new CKEDITOR.dialogCommand('umbracomediaDialog', {
                allowedContent: 'img[*]',
                requiredContent: 'img'
            })
        );
        editor.ui.addButton('umbracomedia', {
            //label: editor.lang.umbracomedia.tooltip,
            label: "Umbraco media picker",
            command: 'umbracomedia',
            toolbar: 'insert,0'
        });
        CKEDITOR.dialog.add('umbracomediaDialog', this.path + 'dialogs/umbracomedia.js')
    }
});