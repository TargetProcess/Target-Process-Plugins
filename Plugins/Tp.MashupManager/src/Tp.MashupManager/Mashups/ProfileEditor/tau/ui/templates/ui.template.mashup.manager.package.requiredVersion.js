tau.mashups
    .addDependency('tau/core/templates-factory')
    .addModule('tau/mashup.manager/ui/templates/ui.template.mashup.manager.package.requiredVersion', function(templates) {
        var config = {
            name:   'mashup.manager.package.requiredVersion',
            engine: 'jqote2',
            markup: [
                '<p class="mashup-info__compatible-tp-version">Requires Targetprocess <%! this %>+</p>'
            ]
        };

        return templates.register(config);
    });