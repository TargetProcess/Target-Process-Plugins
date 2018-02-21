tau.mashups
    .addDependency('jQuery')
    .addDependency('tau/core/extension.base')
    .addModule('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.install', function($, ExtensionBase) {

        return ExtensionBase.extend({
            'bus afterInit + afterRender': function(evtData, initConfig, renderData) {
                var $element = renderData.element;
                var context = initConfig.config.context;
                this._bindInstall($element, context);
            },

            _bindInstall: function($element, context) {
                $element.on('click', '.i-role-installPackage', function(e) {
                    var $target = $(e.currentTarget);
                    var loggedUser = context.configurator.getLoggedUser();

                    var service = context.configurator.service('mashup.manager');
                    service.installPackage({
                        RepositoryName: $target.data('repositoryname'),
                        PackageName: $target.data('packagename'),
                        CreationDate: new Date().getTime(),
                        CreatedBy: {Id: loggedUser.id, Name: loggedUser.name}
                    });
                });
            }
        });
    });
