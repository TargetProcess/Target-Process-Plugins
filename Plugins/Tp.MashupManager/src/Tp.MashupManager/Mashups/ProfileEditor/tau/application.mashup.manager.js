define([
    'jQuery'
    , 'tau/service.container'
    , 'tau/services/service.navigator'
    , 'tau/mashup.manager/services/service.mashup.manager'
    , 'tau/mashup.manager/services/service.mashup.manager.config'
    , 'tau/configurator'
    , 'tau/components/component.application.generic'
    , 'tau/ui/extensions/application.generic/ui.extension.application.generic.placeholder'
    , 'tau/mashup.manager/utils/utils.mashup.manager.actionType'
], function($, ServiceContainer, ServiceNavigator, ServiceMashupManager, ServiceMashupManagerConfig, configurator, ApplicationGeneric, ExtensionPlaceholder, actionType) {

    var routes = [
        {
            pattern: /library/,
            host: {
                name: 'master empty',
                type: 'master.empty'
            },
            type: {
                name: 'mashup manager',
                type: 'mashup.manager',
                namespace: 'tau/mashup.manager'
            },
            adapter: function() {
                this.configurator.service('mashup.manager.config').getConfig()
                    .done(_.bind(function(config) {
                        if (!config.libraryIsAllowed) {
                            this.configurator.service('navigator').to('add');
                            return;
                        }
                        this.resolve({
                            actionData: {
                                actionType: actionType.library,
                                libraryIsAllowed: config.libraryIsAllowed
                            }
                        });
                    }, this));
            }
        },
        {
            pattern: /mashups\/(.+)/,
            host: {
                name: 'master empty',
                type: 'master.empty'
            },
            type: {
                name: 'mashup manager',
                type: 'mashup.manager',
                namespace: 'tau/mashup.manager'
            },
            adapter: function(mashupName) {
                this.configurator.service('mashup.manager.config').getConfig()
                    .done(_.bind(function(config) {
                        this.resolve({
                            actionData: {
                                actionType: actionType.updateMashup,
                                mashupName: mashupName,
                                libraryIsAllowed: config.libraryIsAllowed
                            }
                        });
                    }, this));
            }
        },
        {
            pattern: /add/,
            host: {
                name: 'master empty',
                type: 'master.empty'
            },
            type: {
                name: 'mashup manager',
                type: 'mashup.manager',
                namespace: 'tau/mashup.manager'
            },
            adapter: function() {
                this.configurator.service('mashup.manager.config').getConfig()
                    .done(_.bind(function(config) {
                        this.resolve({
                            actionData: {
                                actionType: actionType.addMashup,
                                libraryIsAllowed: config.libraryIsAllowed
                            }
                        });
                    }, this));
            }
        },
        {
            pattern: /repositories\/(.+)\/packages\/(.+)/,
            host: {
                name: 'master empty',
                type: 'master.empty'
            },
            type: {
                name: 'mashup manager',
                type: 'mashup.manager',
                namespace: 'tau/mashup.manager'
            },
            adapter: function(repositoryName, packageName) {
                this.configurator.service('mashup.manager.config').getConfig()
                    .done(_.bind(function(config) {
                        if (!config.libraryIsAllowed) {
                            this.configurator.service('navigator').to('add');
                            return;
                        }
                        this.resolve({
                            actionData: {
                                actionType: actionType.package,
                                repositoryName: repositoryName,
                                packageName: packageName,
                                libraryIsAllowed: config.libraryIsAllowed
                            }
                        });
                    }, this));
            }
        }
    ];

    return function(placeholder) {
        var applicationId = 'mashup.manager';
        var configurator = new ServiceContainer();
        configurator._id = _.uniqueId('mashup.manager');
        configurator.registerService('navigator', new ServiceNavigator(configurator, { parameterName: applicationId }));
        configurator.registerService('mashup.manager', new ServiceMashupManager(configurator, { placeholder: placeholder }));
        configurator.registerService('mashup.manager.config', new ServiceMashupManagerConfig(configurator));
        var hashService = configurator.getHashService();
        if (!hashService.getHash()) {
            configurator.service('navigator').to('library');
        }
        var config = {
            name: 'mashup manager',
            options: {
                applicationId: applicationId,
                placeholder: placeholder
            },
            routes: routes,
            context: {
                configurator: configurator
            },
            extensions: [ExtensionPlaceholder]
        };

        var app = ApplicationGeneric.create(config);
        app.initialize(config);
    };
});
