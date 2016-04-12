tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/components/component.creator')
    .addDependency('tau/mashup.manager/models/model.mashup.manager.package')
    .addDependency('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.install')
    .addDependency('tau/mashup.manager/ui/templates/ui.template.mashup.manager.package')
    .addModule('tau/mashup.manager/components/component.mashup.manager.package', function (_, ComponentCreator, Model, ExtensionInstall, Template) {
        return {
            create: function (config) {

                var creatorConfig = {
                    extensions: [
                        Model,
                        ExtensionInstall
                    ],
                    template: Template
                };

                return ComponentCreator.create(creatorConfig, config);

            }
        };
    });