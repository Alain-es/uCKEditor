CKEDITOR.plugins.add('umbracomediatagging', {
    icons: 'umbracomediatagging',
    hidpi: true,
    init: function (editor) {
        editor.addCommand('umbracomediatagging', {
            allowedContent: 'img[*]',
            requiredContent: 'img',
            modes: { wysiwyg: 1 },
            canUndo: true,
            exec: function (editor) {
            }
        });
        editor.ui.addButton('umbracomediatagging', {
            //label: editor.lang.umbracomediatagging.tooltip,
            label: "Media picker",
            command: 'umbracomediatagging',
            toolbar: 'umbraco,2'
        });
    }
});