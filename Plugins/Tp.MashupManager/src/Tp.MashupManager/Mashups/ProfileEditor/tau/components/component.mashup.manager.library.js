tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/components/component.creator')
    .addDependency('tau/mashup.manager/models/model.mashup.manager.library')
    .addDependency('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.library')
    .addDependency('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.install')
    .addDependency('tau/mashup.manager/ui/templates/ui.template.mashup.manager.library')
    .addModule('tau/mashup.manager/components/component.mashup.manager.library', function(_, ComponentCreator, Model, ExtensionMain, ExtensionInstall, Template) {
        return {
            create: function (config) {

                var creatorConfig = {
                    extensions: [
                        Model,
                        ExtensionMain,
                        ExtensionInstall
                    ],
                    template: Template
                };

                return ComponentCreator.create(creatorConfig, config);

            }
        };
    });