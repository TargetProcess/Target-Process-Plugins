tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/components/component.creator')
    .addDependency('tau/mashup.manager/models/model.mashup.manager.list')
    .addDependency('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.list')
    .addDependency('tau/mashup.manager/ui/templates/ui.template.mashup.manager.list')
    .addModule('tau/mashup.manager/components/component.mashup.manager.list', function (_, ComponentCreator, Model, ExtensionMain, Template) {
        return {
            create: function (config) {

                var creatorConfig = {
                    extensions: [
                        Model,
                        ExtensionMain
                    ],
                    template: Template
                };

                return ComponentCreator.create(creatorConfig, config);

            }
        };
    });
