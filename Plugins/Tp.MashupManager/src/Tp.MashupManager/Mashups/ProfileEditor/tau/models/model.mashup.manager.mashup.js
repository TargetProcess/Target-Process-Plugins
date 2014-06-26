define([
    'Underscore'
    , 'tau/core/extension.base'
    , 'tau/mashup.manager/utils/utils.mashup.manager.actionType'
], function(_, ExtensionBase, mashupManagerActionType) {
    return ExtensionBase.extend({
        'bus afterInit': function(evtData, initConfig) {
            var configurator = initConfig.config.context.configurator;
            var service = configurator.service('mashup.manager');
            switch (initConfig.config.context.actionData.actionType) {
                case mashupManagerActionType.addMashup:
                    this.fire('dataBind', this._getDefaultMashup());
                    break;
                case mashupManagerActionType.updateMashup:
                    var navigator = initConfig.config.context.configurator.service('navigator');
                    service.getMashupByName(initConfig.config.context.actionData.mashupName)
                        .done(_.bind(function(mashup) {
                            this.fire('dataBind', this._transform(mashup));
                        }, this))
                        .fail(_.bind(function(navigatorService) {
                            navigatorService.to('library');
                        }, this, navigator));
                    break;
            }
        },
        _transform: function(mashup) {
            var fileForDisplay = this._findFileForDisplay(mashup) || {Content: ''};
            return {
                name: mashup.Name,
                placeholders: mashup.Placeholders,
                script: fileForDisplay.Content,
                fileName: fileForDisplay.FileName
            };
        },
        DEFAULT_MASHUP: [
            'tau.mashups',
            '  .addDependency(\'libs/jquery/jquery\')',
            '  .addMashup(function($, config) {',
            '    alert(\'Hello World\');',
            '  });'
        ],
        _getDefaultMashup: function() {
            return {
                placeholders: 'footerplaceholder',
                script: this.DEFAULT_MASHUP.join('\n')
            };
        },
        _findFileForDisplay: function(mashup) {
            var jsFiles = _.filter(mashup.Files, function(file) {
                return /.+\.js$/i.test(file.FileName);
            });
            if (jsFiles.length === 1) {
                return jsFiles[0];
            }
            var configFile = _.find(jsFiles, function(file) {
                return /^.+\.config\.js\s*$/i.test(file.FileName);
            });
            if (configFile) {
                return configFile;
            }
            var mashupNameLowerCased = mashup.Name.toLowerCase();
            var mashupNameFile = _.find(jsFiles, function(file) {
                return file.FileName.toLowerCase().indexOf(mashupNameLowerCased) >= 0;
            });
            if (mashupNameFile) {
                return mashupNameFile;
            }
            if (jsFiles.length === 0) {
                return null;
            }
            return jsFiles[0];
        }
    });
});
