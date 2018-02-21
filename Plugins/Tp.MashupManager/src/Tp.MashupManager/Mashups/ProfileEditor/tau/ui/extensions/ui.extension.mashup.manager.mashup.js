tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('libs/scriptErrorChecker')
    .addDependency('tau/core/extension.base')
    .addDependency('tau/mashup.manager/utils/utils.mashup.manager.actionType')
    .addDependency('tau/mashup.manager/utils/utils.mashup.manager.placeholderParser')
    .addDependency('tau/mashup.manager/utils/utils.mashup.manager.mashup.data.converter')
    .addDependency('tp/plugins/errorMessageContainer')
    .addDependency('libs/aceEditor/aceEditor')
    .addDependency('tau/mashup.manager/ui/templates/ui.template.mashup.manager.mashup.metainfo')
    .addModule('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.mashup',
        function($, _, scriptErrorChecker, ExtensionBase, mashupManagerActionType, PlaceholderParser,
            mashupDataConverter, ErrorMessageContainer, aceEditor, mashupMetaInfoTemplate) {

            return ExtensionBase.extend({

                'bus afterInit:last + dataBind:last + afterRender': function(evt, initConfig, mashupInfo, renderData) {
                    var $element = renderData.element;
                    var context = initConfig.config.context;
                    this._service = context.configurator.service('mashup.manager');

                    this.codeEditor = aceEditor.setEditor($element.find('.i-role-script'), this.bus,
                        {
                            maxLines: Infinity,
                            mode: 'ace/mode/javascript',
                            useSoftTabs: true,
                            tabSize: 4
                        });

                    this._bindSave($element, mashupInfo, context);
                    this._bindMetaInfoRefresh($element);

                    this.errorContainer = new ErrorMessageContainer({
                        placeholder: $element,
                        generalErrorContainer: '.i-role-failedOperation'
                    });
                    this._showLibraryLink($element);
                    this._bindPlaceholderParser($element);
                    this._bindPlaceholdersHelp($element);
                },

                _showLibraryLink: function($element) {
                    $element.find('.i-role-libraryLink').show();
                },

                _bindPlaceholderParser: function($element) {
                    (new PlaceholderParser()).attach($element.find('.i-role-placeholders'));
                },

                _bindPlaceholdersHelp: function($element) {
                    $element.find('.i-role-placeholdersHelpLink').click(function() {
                        $element.find('.i-role-placeholdersHelp')
                            .animate({opacity: 'toggle', height: 'toggle'}, 'slow');
                    });
                },

                _bindMetaInfoRefresh: function($element) {
                    var refreshMetaInfo = function(event, mashup) {
                        var mashupInfo = mashupDataConverter.convertServerMashupDataToClientFormat(mashup);
                        var markup = mashupMetaInfoTemplate.renderToString(mashupInfo);
                        $element.find('.i-role-mashupMetaInfo').html(markup);
                    };
                    this._service.on(['mashupAdded', 'mashupUpdated'], refreshMetaInfo, this);
                },

                _bindSave: function($element, mashupInfo, context) {
                    this._eventNamespace = _.uniqueId('.mashupManager');
                    $(document).on('keydown' + this._eventNamespace, function(e) {
                        if ((e.metaKey || e.ctrlKey) && String.fromCharCode(e.keyCode).toLowerCase() === 's') {
                            e.preventDefault();
                            this._saveMashup($element, mashupInfo, context);
                        }
                    }.bind(this));

                    $element.find('.i-role-save').click(function() {
                        this._saveMashup($element, mashupInfo, context);
                    }.bind(this));
                },

                _saveMashup: function($element, mashupInfo, context) {
                    this.errorContainer.clearErrors();
                    var service = this._service;

                    this
                        ._getEditingMashup($element, mashupInfo, context)
                        .then(function(mashup, error) {
                            if (error && error.name === 'SyntaxError') {
                                service.status.error(error.name + ': ' + error.message);
                            } else {
                                var actionType = context.actionData.actionType;
                                var action = actionType === mashupManagerActionType.addMashup ?
                                    'addMashup' : 'updateMashup';
                                service[action](mashup, this._saveFailHandler.bind(this));
                            }
                        }.bind(this));
                },

                _getEditingMashup: function($element, mashupInfo, context) {
                    return this.codeEditor.then(function(editor) {
                        var mashupName = _.trim($element.find('.i-role-mashupName').val());
                        var script = editor.getValue();

                        var updatedInfo = {
                            name: mashupName,
                            placeholders: $element.find('.i-role-placeholders').val(),
                            fileName: mashupInfo.fileName || mashupName + '.js',
                            script: script,
                            mashupMetaInfo: {
                                isEnabled: $element.find('.i-role-mashupIsEnabled:checked').val() === 'true',
                                lastModificationDate: new Date(),
                                lastModifiedBy: context.configurator.getLoggedUser()
                            }
                        };

                        var mashup = mashupDataConverter.convertClientMashupDataToServerFormat(mashupInfo, updatedInfo);
                        var error = scriptErrorChecker.checkForScriptErrors(script);
                        return $.when(mashup, error);
                    });
                },

                _saveFailHandler: function(responseText) {
                    var error = JSON.parse(responseText);
                    this.errorContainer.addRange(error);
                    this.errorContainer.render();
                },

                destroy: function() {
                    if (this._eventNamespace) {
                        $(document).off(this._eventNamespace);
                    }
                    if (this._service) {
                        this._service.removeAllListeners(this);
                    }
                    this._super();
                }
            });
        }
    );
