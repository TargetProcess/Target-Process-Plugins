tau.mashups
    .addDependency('jQuery')
    .addDependency('tau/core/extension.base')
    .addDependency('tau/ui/behaviour/common/ui.behaviour.progressIndicator')
    .addModule('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.install',
        function($, ExtensionBase, ProgressIndicator) {

            return ExtensionBase.extend({
                'bus afterInit + afterRender' : function(evtData, initConfig, renderData){
                    var $element = renderData.element;
                    var service = initConfig.config.context.configurator.service('mashup.manager');
                    this._bindInstall($element, service);
                },
                _bindInstall: function($element, service){
                    $element.on('click', '.i-role-installPackage', _.bind(function(mashupManagerService, e){
                        var $target = $(e.currentTarget);
                        var packageToInstall = {
                            RepositoryName: $target.data('repositoryname'),
                            PackageName: $target.data('packagename')
                        };
                        mashupManagerService.installPackage(packageToInstall);
                    }, this, service));
                }
            });
        }
    );
