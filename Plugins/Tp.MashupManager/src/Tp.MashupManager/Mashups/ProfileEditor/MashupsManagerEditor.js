tau.mashups
    .addDependency("MashupManager/EditorTemplate")
    .addDependency("MashupManager/ProfileRepository")
    .addDependency("tp/plugins/ErrorMessageContainer")
    .addDependency("tp/status")
    .addDependency("tp/codemirror/codemirror")
    .addDependency("tp/codemirror/mode/javascript/javascript")
    .addDependency("libs/jquery/jquery")
    .addDependency("libs/jquery/jquery.tmpl")
    .addDependency("libs/jquery/jquery.ui.confirmation")
    .addCSS("../../tau/scripts/tp/codemirror/codemirror.css")
    .addCSS("../../tau/scripts/tp/codemirror/theme/default.css")
    .addModule("MashupManager/MashupManagerEditor", function (template, repository, errorMessageContainer, status) {
        function managerEditor(config) {
            this._create(config);
        }

        ;

        managerEditor.prototype = {
            _create: function(config) {
                this.placeholder = config.placeholder;
                this.template = template;
                this.status = status;
                this.repository = new repository({defaultErrorHandler: $.proxy(this._onError, this)});

                this.errorContainer = new errorMessageContainer({
                    placeholder: this.placeholder,
                    generalErrorContainer: '#failedOperation' });
            },

            initialize: function() {
                this.placeholder.html('');

                this.repository.getProfile($.proxy(this._onGetProfileSuccess, this), $.proxy(this._onGetProfileFail, this));
            },

            _save: function() {
                this.status.saving();
                this._disableSaveBtn();

                this.errorContainer.clearErrors();

                var mashup = this._getEditingMashup();

                if (this._isMashupEditing()) {
                    mashup.OldName = this.hiddenMashupName.val();
                    this.repository.updateMashup(mashup, $.proxy(this._onSaveMashupSuccess(mashup), this), $.proxy(this._onSaveMashupFail, this));
                }
                else {
                    this.repository.addMashup(mashup, $.proxy(this._onSaveMashupSuccess(mashup), this), $.proxy(this._onSaveMashupFail, this));
                }
            },

            _onError: function(responseText) {
                this.status.error("An exception occurred: " + responseText);
                this._enableSaveBtn();
            },

            _onSaveMashupSuccess: function(mashup) {
                return function(){
                    this.status.success('Mashup saved successfully');
                    this._enableSaveBtn();

                    this.repository.getProfile($.proxy(this._profileUpdated(mashup), this), $.proxy(this._onGetProfileFail, this));
                }
            },

            _selectMashup: function(mashup){
                var mashupName = $.grep(this.placeholder.find('a.mashupName'), function(item){
                    return $(item).text().trim() === mashup.Name;
                });

                $(mashupName).addClass('selected');
            },

            _onSaveMashupFail: function(responseText) {
                this.status.hide();
                this._enableSaveBtn();

                var data = JSON.parse(responseText);
                this.errorContainer.addRange(data);
                this.errorContainer.render();
            },

            _isMashupEditing: function() {
                return this.hiddenMashupName.val() && this.hiddenMashupName.val() != '';
            },

            _onGetProfileSuccess: function(profile) {
                var formZone = $(this.template.formTemplate);
                formZone.appendTo(this.placeholder);

                var profileZone = $.tmpl(this.template.profileTemplate, profile);
                formZone.find('#mashupList').append(profileZone);

                var mashupZone = $.tmpl(this.template.mashupTemplate, this._getDefaultMashup());
                formZone.find('#mashupEdit').append(mashupZone);

                this._initMashupAreaControls();

                this._initListAreaControls();
            },

            _initListAreaControls: function() {
                this.hiddenProfileName = this.placeholder.find('#hiddenProfileName');
                this.placeholder.find('a.mashupName').click($.proxy(this._onMashupEditClick, this));
                this.placeholder.find('button.delete-mashup').click($.proxy(this._onMashupDeleteClick, this));
                this.placeholder.find('button#addMashup').click($.proxy(this._onAddNewMashupClick, this));
            },

            _initMashupAreaControls: function() {
                this.scriptEditor = CodeMirror.fromTextArea(this.placeholder.find('#script')[0], {
                    lineNumbers: true,
                    matchBrackets: true
                });

                this.placeholder.find('div.CodeMirror-scroll').addClass('mashup-code-editor');

                this.hiddenMashupName = this.placeholder.find('#hiddenMashupName');
                this.btnSave = this.placeholder.find('#save');
                this.btnSave.click($.proxy(this._save, this));

                this.placeholder.find('#name').focus();

                this.placeholder.find('._moreLink').click($.proxy(this._showPlaceholdersHelp, this));
            },

            _showPlaceholdersHelp: function(){
                this.placeholder.find('._placeholdersHelp').animate({ opacity: 'toggle', height: 'toggle' }, 'slow');

            },

            _onGetProfileFail: function(error) {
                this.status.error("An exception occurred when loading the page: " + error);
            },

            _onAddNewMashupClick: function() {
                this._removeSelectionInMashupList();
                this._bindMashup(this._getDefaultMashup());
            },

            _onMashupEditClick: function(a) {
                if ($(a.target).is("button")) {
                    return;
                }

                this._removeSelectionInMashupList();

                var target = $(a.target);
                target.addClass('selected');

                this.repository.getMashupByName(target.text(), $.proxy(this._onGetMashupSuccess, this), $.proxy(this._onGetMashupFail, this));
            },

            _removeSelectionInMashupList: function() {
                this.placeholder.find('a.mashupName').removeClass('selected');
            },

            _onMashupDeleteClick: function(btn) {
                btn.preventDefault();
                var target = $(btn.target);
                if(target.is('span')){
                    target = target.parent();
                }
                var deleteBlock = target.parent();
                var that = this;

                deleteBlock.confirmation({
                    message: '',
                    okLabel: 'OK',
                    cancelLabel: 'Cancel',
                    ok: function() {
                        deleteBlock.confirmation('hideConfirmationMessage');

                        var mashupName = target.attr('mashup').toString();
                        that.repository.deleteMashup({Name:mashupName, Placeholders:'', Script:''}, $.proxy(that._onDeleteMashupSuccess, that), $.proxy(that._onDeleteMashupFail, that))
                    },

                    cancel: function() {
                        deleteBlock.confirmation('hide');
                    }
                });

                deleteBlock.confirmation('show');
            },

            _onDeleteMashupSuccess: function() {
                this.status.success('Mashup deleted successfully');
                this.initialize();
            },

            _onDeleteMashupFail: function(error) {
                this.status.error("An exception occurred when deleting the mashup: " + error);
            },

            _onGetMashupSuccess: function(mashup) {
                this._bindMashup(mashup);
            },

            _onGetMashupFail: function(error) {
                this.status.error("An exception occurred when loading the mashup: " + error);
            },

            _bindMashup: function(mashup) {
                var mashupZone = $.tmpl(this.template.mashupTemplate, mashup);
                this.placeholder.find('#mashupDetails').replaceWith(mashupZone);

                this._initMashupAreaControls();
            },

            _profileUpdated: function(mashup) {
                return function(profile){
                    var profileZone = $.tmpl(this.template.profileTemplate, profile);
                    this.placeholder.find('#profileArea').replaceWith(profileZone);

                    this._initListAreaControls();

                    this._selectMashup(mashup);
                    this.hiddenMashupName.val(mashup.Name);
                }
            },

            _getDefaultMashup: function() {
                return {
                    Name:'',
                    Placeholders:'footerplaceholder',
                    Script: "tau.mashups\n .addDependency('libs/jquery/jquery')\n .addMashup(function ($, config) {\n   alert('Hello World');\n  });"
                };
            },

            _getEditingMashup: function() {
                return {
                    Script: this.scriptEditor.getValue(),
                    Name: this.placeholder.find('#name').val(),
                    Placeholders: this.placeholder.find('#placeholders').val()
                };
            },

            _disableSaveBtn: function() {
                this.btnSave.attr("disabled", "disabled");
            },

            _enableSaveBtn: function() {
                this.btnSave.removeAttr("disabled");
            }
        };
        return managerEditor;
    });