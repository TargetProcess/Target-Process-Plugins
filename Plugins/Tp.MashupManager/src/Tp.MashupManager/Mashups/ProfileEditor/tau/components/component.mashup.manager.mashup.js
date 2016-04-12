tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/components/component.creator')
    .addDependency('tau/mashup.manager/models/model.mashup.manager.mashup')
    .addDependency('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.mashup')
    .addDependency('tau/mashup.manager/ui/templates/ui.template.mashup.manager.mashup')
    .addModule('tau/mashup.manager/components/component.mashup.manager.mashup', function (_, ComponentCreator, Model, ExtensionMain, Template) {
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