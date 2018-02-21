/*global tau*/
tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/core/extension.base')
    .addModule('tau/mashup.manager/models/model.mashup.manager.library', function(_, ExtensionBase) {
        return ExtensionBase.extend({
            DESCRIPTION_MAX_SIZE: 160,

            'bus afterInit': function(evtData, initConfig) {
                var configurator = initConfig.config.context.configurator;
                var service = configurator.service('mashup.manager');
                service.getLibraryRepositories()
                    .done(function(repositories) {
                        this.fire('dataBind', {
                            mashupPackages: this._transform(repositories)
                        });
                    }.bind(this))
                    .fail(function() {
                        var navigator = configurator.service('navigator');
                        navigator.to('add');
                    });
            },

            _transform: function(repositories) {
                var mashupPackages = [];

                for (var i = 0, len = repositories.length; i < len; i++) {
                    var repository = repositories[i];
                    for (var j = 0, jlen = repository.Packages.length; j < jlen; j++) {
                        var mashupPackage = repository.Packages[j];
                        var description = mashupPackage.BaseInfo.ShortDescription;

                        var result = {
                            name: mashupPackage.Name,
                            description: description,
                            extraDescription: null,
                            compatibleTpVersionMinimum: mashupPackage.BaseInfo.CompatibleTpVersion.Minimum,
                            repositoryName: repository.Name
                        };

                        if (description.length > this.DESCRIPTION_MAX_SIZE) {
                            // We need to break by DESCRIPTION_MAX_SIZE when breaking position equals to zero
                            // (it means string doesn't contain whitespaces)
                            var breakingPosition = (description.substr(0,
                                    this.DESCRIPTION_MAX_SIZE).search(/ [^ ]*$/) + 1) ||
                                this.DESCRIPTION_MAX_SIZE;
                            result.description = description.substr(0, breakingPosition);
                            result.extraDescription = description.substr(breakingPosition);
                        }

                        mashupPackages.push(result);
                    }
                }

                return _.sortBy(mashupPackages, 'name');
            }
        });
    });
