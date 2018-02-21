tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/core/extension.base')
    .addDependency('tau/mashup.manager/utils/utils.mashup.manager.actionType')
    .addDependency('tau/mashup.manager/utils/utils.mashup.manager.mashup.data.converter')
    .addModule('tau/mashup.manager/models/model.mashup.manager.mashup',
        function(_, ExtensionBase, mashupManagerActionType, mashupDataConverter) {
            return ExtensionBase.extend({
                'bus afterInit': function(evtData, initConfig) {
                    var context = initConfig.config.context;
                    var configurator = context.configurator;

                    switch (context.actionData.actionType) {
                        case mashupManagerActionType.addMashup:
                            this._showAddedMashup(configurator);
                            break;

                        case mashupManagerActionType.updateMashup:
                            var mashupName = context.actionData.mashupName;
                            this._showUpdatedMashup(configurator, mashupName);
                            break;
                    }
                },

                _showMashup: function(mashupData) {
                    this.fire('dataBind', mashupData);
                },

                _showAddedMashup: function(configurator) {
                    var loggedUser = configurator.getLoggedUser();
                    var script = this.DEFAULT_MASHUP.join('\n');
                    var mashupData = mashupDataConverter.createNewMashupData(script, loggedUser);
                    mashupData.mashupMetaInfo.isDraft = true;
                    this._showMashup(mashupData);
                },

                _showUpdatedMashup: function(configurator, mashupName) {
                    var service = configurator.service('mashup.manager');
                    service.getMashupByName(mashupName)
                        .done(function(mashup) {
                            var mashupData = mashupDataConverter.convertServerMashupDataToClientFormat(mashup);
                            this._showMashup(mashupData);
                        }.bind(this))
                        .fail(function() {
                            var navigator = configurator.service('navigator');
                            navigator.to('library');
                        });
                },

                DEFAULT_MASHUP: [
                    'tau.mashups',
                    '    .addDependency(\'jQuery\')',
                    '    .addMashup(function($, config) {',
                    '        console.log(\'Hello World\');',
                    '    });'
                ]
            });
        });
