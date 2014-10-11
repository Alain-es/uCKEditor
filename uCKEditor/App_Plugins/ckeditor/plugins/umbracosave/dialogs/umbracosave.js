CKEDITOR.dialog.add('umbracosaveDialog', function (editor) {
    return {
        title: 'Save content in Umbraco',
        minWidth: 400,
        minHeight: 200,
        contents: [{
            id: 'save',
            label: 'Save',
            elements: [{
                type: 'text',
                id: 'src',
                label: 'Save',
            }]
        }],
        onShow: function () {
            // Hide this dialog since no dialog is needed
            CKEDITOR.dialog.getCurrent().hide();
        }
    }
});