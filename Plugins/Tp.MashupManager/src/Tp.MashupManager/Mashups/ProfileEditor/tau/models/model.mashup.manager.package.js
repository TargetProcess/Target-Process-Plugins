tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/core/extension.base')
    .addDependency('tau/mashup.manager/libs/marked/marked')
    .addModule('tau/mashup.manager/models/model.mashup.manager.package', function(_, ExtensionBase, marked) {
        return ExtensionBase.extend({
            'bus afterInit': function(evtData, initConfig){
                var configurator = initConfig.config.context.configurator;
                var service = configurator.service('mashup.manager');
                var navigator = initConfig.config.context.configurator.service('navigator');
                var packageObj = {
                    RepositoryName: initConfig.config.context.actionData.repositoryName,
                    PackageName: initConfig.config.context.actionData.packageName
                };
                service.getPackageDetailed(packageObj)
                    .done(_.bind(function(repositoryName, packageDetailed){
                        this.fire('dataBind', this._transform(repositoryName, packageDetailed));
                    }, this, initConfig.config.context.actionData.repositoryName))
                    .fail(_.bind(function(navigatorService){
                        navigatorService.to("library");
                    }, this, navigator));
            },
            _transform: function(repositoryName, packageDetailed){
                return {
                    name: packageDetailed.Name,
                    repositoryName: repositoryName,
                    readmeHtml: marked(packageDetailed.ReadmeMarkdown),
                    compatibleTpVersionMinimum: packageDetailed.CompatibleTpVersionMinimum
                };
            }
        });
    });