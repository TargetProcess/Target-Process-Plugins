define([
    'tau/core/templates-factory'
], function(templates) {

    var config = {
        name:   'mashup.manager',
        engine: 'jqote2',
        markup: [
            '<div class="plugins mashups-manager i-role-mashupManager">',
                '<h2 class="h2 icon">Mashup Manager</h2>',
                '<table class="mashups-block">',
                    '<tr>',
                        '<td class="list i-role-mashupList"></td>',
                        '<td class="editor i-role-mashupEdit"></td>',
                    '</tr>',
                '</table>',
            '</div>'
        ]
    };

    return templates.register(config);
});
