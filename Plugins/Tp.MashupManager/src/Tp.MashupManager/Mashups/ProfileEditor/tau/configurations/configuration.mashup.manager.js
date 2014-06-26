define([
    'Underscore'
    , 'tau/core/class'
    , 'tau/mashup.manager/ui/templates/ui.template.mashup.manager'
    , 'tau/mashup.manager/utils/utils.mashup.manager.actionType'
    , 'tau/mashup.manager/components/all.components'
], function(_, Class, Template, mashupManagerActionType) {

    var ConfigurationMashupManager = Class.extend({

        getConfig: function(appConfig) {
            var config = {
                layout: 'selectable',
                template: Template,
                children: [
                    {
                        type: 'container',
                        selector: '.i-role-mashupList',
                        spinnerConfigForLazy: {
                            isShow: true,
                            selectorForHeight: '.i-role-mashupList'
                        },
                        name: 'lazyPlaceholder container',
                        children: [
                            {
                                lazy: true,
                                name: 'mashup manager list',
                                type: 'mashup.manager.list',
                                namespace: 'tau/mashup.manager'
                            }
                        ]
                    },
                    {
                        type: 'container',
                        selector: '.i-role-mashupEdit',
                        spinnerConfigForLazy: {
                            isShow: true,
                            selectorForHeight: '.i-role-mashupEdit'
                        },
                        name: 'lazyPlaceholder container',
                        children: this._getActionControlConfig(appConfig.actionData.actionType)
                    }
                ]
            };

            return config;
        },

        _getActionControlConfig: function(actionType) {
            switch (actionType) {
                case mashupManagerActionType.library:
                    return [
                        {
                            lazy: true,
                            name: 'mashup manager library',
                            type: 'mashup.manager.library',
                            namespace: 'tau/mashup.manager'
                        }
                    ];
                case mashupManagerActionType.addMashup:
                case mashupManagerActionType.updateMashup:
                    return [
                        {
                            lazy: true,
                            name: 'mashup manager mashup',
                            type: 'mashup.manager.mashup',
                            namespace: 'tau/mashup.manager'
                        }
                    ];
                case mashupManagerActionType.package:
                    return [
                        {
                            lazy: true,
                            name: 'mashup manager package',
                            type: 'mashup.manager.package',
                            namespace: 'tau/mashup.manager'
                        }
                    ];
            }
        }
    });

    return ConfigurationMashupManager;
});
