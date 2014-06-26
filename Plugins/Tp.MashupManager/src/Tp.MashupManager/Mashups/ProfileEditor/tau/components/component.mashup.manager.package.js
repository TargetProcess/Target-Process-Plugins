define([
    'Underscore'
    , 'tau/components/component.creator'
    , 'tau/mashup.manager/models/model.mashup.manager.package'
    , 'tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.install'
    , 'tau/mashup.manager/ui/templates/ui.template.mashup.manager.package'
], function (_, ComponentCreator, Model, ExtensionInstall, Template) {
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