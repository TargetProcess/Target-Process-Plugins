/*global tau*/
tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/service.container')
    .addDependency('tau/services/service.navigator')
    .addDependency('tau/mashup.manager/services/service.mashup.manager')
    .addDependency('tau/components/component.application.generic')
    .addDependency('tau/ui/extensions/application.generic/ui.extension.application.generic.placeholder')
    .addDependency('tau/mashup.manager/utils/utils.mashup.manager.actionType')
    .addModule('tau/mashup.manager/application.mashup.manager', function(_, ServiceContainer, ServiceNavigator,
        ServiceMashupManager, ApplicationGeneric, ExtensionPlaceholder, actionType) {

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
                    this.resolve({
                        actionData: {
                            actionType: actionType.library
                        }
                    });
;
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
                    this.resolve({
                        actionData: {
                            actionType: actionType.updateMashup,
                            mashupName: decodeURIComponent(mashupName),
                        }
                    });
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
                    this.resolve({
                        actionData: {
                            actionType: actionType.addMashup,
                        }
                    });
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
                    this.resolve({
                        actionData: {
                            actionType: actionType.packageType,
                            repositoryName: repositoryName,
                            packageName: decodeURIComponent(packageName),
                        }
                    });
                }
            }
        ];

        return function(placeholder) {
            var applicationId = 'mashup.manager';
            var configurator = new ServiceContainer();
            configurator._id = _.uniqueId('mashup.manager');
            configurator.registerService('navigator',
                new ServiceNavigator(configurator, {parameterName: applicationId}));
            configurator.registerService('mashup.manager',
                new ServiceMashupManager(configurator, {placeholder: placeholder}));

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
