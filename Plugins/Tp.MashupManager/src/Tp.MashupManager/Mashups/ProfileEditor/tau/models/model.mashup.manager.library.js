tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/core/extension.base')
    .addModule('tau/mashup.manager/models/model.mashup.manager.library', function(_, ExtensionBase) {
        return ExtensionBase.extend({
            DESCRIPTION_MAX_SIZE: 160,
            'bus afterInit': function(evtData, initConfig){
                var configurator = initConfig.config.context.configurator;
                var service = configurator.service('mashup.manager');
                var navigator = initConfig.config.context.configurator.service('navigator');
                service.getLibraryRepositories()
                    .done(_.bind(function(repositories){
                        this.fire('dataBind', {
                            mashupPackages: this._transform(repositories)
                        })
                    }, this))
                    .fail(_.bind(function(navigatorService){
                        navigatorService.to("add");
                    }, this, navigator));
            },

            _transform: function(repositories){
                var mashupPackages = [];
                for(var i = 0, len = repositories.length; i < len; i++){
                    var repository = repositories[i];
                    for(var j = 0, jlen = repository.Packages.length; j < jlen; j++){
                        var description,
                            extraDescription,
                            mashupPackage = repository.Packages[j],
                            tooLongDescription = mashupPackage.BaseInfo.ShortDescription.length > this.DESCRIPTION_MAX_SIZE;

                        if (tooLongDescription){
                            // We need to break by DESCRIPTION_MAX_SIZE when breaking position equals to zero (it means string doesn't contain whitespaces)
                            var breakingDescriptionPosition = (mashupPackage.BaseInfo.ShortDescription.substr(0, this.DESCRIPTION_MAX_SIZE).search(/ [^ ]*$/) + 1)
                                || this.DESCRIPTION_MAX_SIZE;
                            description = mashupPackage.BaseInfo.ShortDescription.substr(0, breakingDescriptionPosition);
                            extraDescription = mashupPackage.BaseInfo.ShortDescription.substr(breakingDescriptionPosition);
                        } else {
                            description = mashupPackage.BaseInfo.ShortDescription;
                            extraDescription = null;
                        }

                        mashupPackages.push({
                            name: mashupPackage.Name,
                            description: description,
                            extraDescription: extraDescription,
                            compatibleTpVersionMinimum: mashupPackage.BaseInfo.CompatibleTpVersion.Minimum,
                            repositoryName: repository.Name
                        });
                    }
                }

                return mashupPackages;
            }
        });
    });