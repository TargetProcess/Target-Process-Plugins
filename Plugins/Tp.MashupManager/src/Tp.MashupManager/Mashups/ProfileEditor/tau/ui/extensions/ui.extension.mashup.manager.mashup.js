define([
    'jQuery'
    , 'tau/core/extension.base'
    , 'tau/mashup.manager/utils/utils.mashup.manager.codeEditor'
    , 'tau/mashup.manager/utils/utils.mashup.manager.actionType'
    , 'tau/mashup.manager/utils/utils.mashup.manager.placeholderParser'
    , 'tp/plugins/errorMessageContainer'
], function ($, ExtensionBase, CodeEditor, mashupManagerActionType, PlaceholderParser, ErrorMessageContainer) {

    return ExtensionBase.extend({
        'bus afterInit:last + dataBind:last + afterRender': function (evtData, initConfig, bindData, renderData) {
            var $element = renderData.element;
            var service = initConfig.config.context.configurator.service('mashup.manager');
            var actionType = initConfig.config.context.actionData.actionType;
            this.codeEditor = this._createCodeEditor($element);
            this.errorContainer = this._createErrorContainer($element);
            if (initConfig.config.context.actionData.libraryIsAllowed) {
                this._showLibraryLink($element);
            }
            this._bindPlaceholderParser($element);
            this._bindPlaceholdersHelp($element);
            this._bindSave($element, service, bindData, actionType);
        },
        _createCodeEditor: function (placeholder) {
            return new CodeEditor({
                placeholder: placeholder,
                textAreaSelector: '.i-role-script'
            });
        },
        _createErrorContainer: function ($element) {
            return new ErrorMessageContainer({
                placeholder: $element,
                generalErrorContainer: '.i-role-failedOperation' });
        },
        _showLibraryLink: function ($element) {
            $element.find('.i-role-libraryLink').show();
        },
        _bindPlaceholderParser: function ($element) {
            (new PlaceholderParser).attach($element.find('.i-role-placeholders'));
        },
        _bindPlaceholdersHelp: function ($element) {
            $element.find('.i-role-placeholdersHelpLink').click(_.bind(function () {
                $element.find('.i-role-placeholdersHelp').animate({ opacity: 'toggle', height: 'toggle' }, 'slow');
            }, this));
        },
        _bindSave: function ($element, service, bindData, actionType) {
            var mashupOldName = $element.find('.i-role-mashupName').val();
            $element.find('.i-role-save').click(_.bind(function (oldName) {
                this.errorContainer.clearErrors();
                var action = actionType === mashupManagerActionType.addMashup ? 'addMashup' : 'updateMashup';
                service[action](this._getEditingMashup($element, oldName, bindData), _.bind(this._saveFailHandler, this));
            }, this, mashupOldName));
        },
        _getEditingMashup: function ($element, oldName, bindData) {
            var mashupName = _.trim($element.find('.i-role-mashupName').val());
            var fileName = bindData.fileName || mashupName + '.js';
            var mashup = {
                Files: [
                    {FileName: fileName, Content: this.codeEditor.getValue()}
                ],
                Name: mashupName,
                Placeholders: $element.find('.i-role-placeholders').val()
            };

            if (oldName) {
                mashup.OldName = oldName;
            }

            return mashup;
        },
        _saveFailHandler: function (responseText) {
            var error = JSON.parse(responseText);
            this.errorContainer.addRange(error);
            this.errorContainer.render();
        }
    });
});
