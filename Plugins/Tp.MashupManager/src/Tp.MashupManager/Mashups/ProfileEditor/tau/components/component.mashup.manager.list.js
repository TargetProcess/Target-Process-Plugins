define([
    'Underscore'
    , 'tau/components/component.creator'
    , 'tau/mashup.manager/models/model.mashup.manager.list'
    , 'tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.list'
    , 'tau/mashup.manager/ui/templates/ui.template.mashup.manager.list'
], function (_, ComponentCreator, Model, ExtensionMain, Template) {
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
