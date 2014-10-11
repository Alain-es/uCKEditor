CKEDITOR.dialog.add('umbracomediaDialog', function (editor) {
    return {
        title: 'Umbraco media picker',
        minWidth: 400,
        minHeight: 200,
        contents: [{
            id: 'media',
            label: 'Media',
            elements: [{
                type: 'text',
                id: 'src',
                label: 'Media',
            }]
        }],
        onShow: function () {
            // Hide this dialog since Umbraco media picker is going to be used to select the media file
            CKEDITOR.dialog.getCurrent().hide();
        }
    }
});