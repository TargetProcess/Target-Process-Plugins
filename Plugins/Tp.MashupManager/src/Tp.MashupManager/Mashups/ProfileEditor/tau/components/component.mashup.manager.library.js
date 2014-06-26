define([
    'Underscore'
    , 'tau/components/component.creator'
    , 'tau/mashup.manager/models/model.mashup.manager.library'
    , 'tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.library'
    , 'tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.install'
    , 'tau/mashup.manager/ui/templates/ui.template.mashup.manager.library'
], function (_, ComponentCreator, Model, ExtensionMain, ExtensionInstall, Template) {
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