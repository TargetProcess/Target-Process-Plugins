tau.mashups
    .addDependency("emailIntegration/editor")
    .addDependency("tp/plugins/profileRepository")
    .addDependency("tp/plugins/commandGateway")
    .addDependency("libs/jquery/jquery")
    .addMashup(function (editor, profileRepository, commandGateway, $, config) {

        new editor({
            config: config,
            placeHolder: $('#' + config.placeholderId),
            profileRepository: profileRepository,
            commandGateway: new commandGateway()
        }).renderProfile();
});

tau.mashups
    .addDependency("tp/bus")
    .addDependency("tp/plugins/profileControlsBlock")
    .addDependency("emailIntegration/editorTemplate")
    .addDependency("tp/plugins/errorMessageContainer")
    .addDependency("libs/jquery/jquery")
    .addDependency("emailIntegration/jquery/numeric")
    .addDependency("emailIntegration/jquery/iphone-switch")
    .addDependency("emailIntegration/jquery/defaultvalue.source")
    .addModule("emailIntegration/editor",

     function (bus, profileControlsBlock, editorTemplate, errorMessageContainer, $) {

         function emailProfileEditor(config) {
             this._create(config);
         }

         emailProfileEditor.prototype = {
             template: null,
             placeHolder: null,
             saveBtn: null,
             cancelBtn: null,
             portInput: null,
             checkConnectionErrorMessageContainer: null,
             showSampleLink: null,
             samplePopup: null,

             _create: function (config) {
                 this.placeHolder = config.placeHolder;
                 this.repository = config.profileRepository;
                 this.commandGateway = config.commandGateway;
                 this.template = editorTemplate;
                 this.bus = bus;
                 this.checkConnectionErrorMessageContainer = new errorMessageContainer({ placeholder: this.placeHolder, generalErrorMessage: 'Unable to establish connection', generalErrorContainer: '#failedConnection' });

                 this.bus.subscribe('EmailProfileEditor', {
                     onSaveProfile: $.proxy(this._saveProfile, this),
                     onProfileSaveSucceed: $.proxy(this._onSaveSuccess, this)
                 }, true);
             },

             renderProfile: function () {
                 this.repository.getCurrentProfile($.proxy(this._renderProfile, this));
             },

             _getDefaultProfile: function () {
                 return {
                     Name: '',
                     Settings: {
                         Login: '',
                         Password: '',
                         Protocol: '',
                         Port: 110,
                         UseSSL: false,
                         Rules: null,
                         MailServer: '',
                         Protocol: ''
                     }
                 };
             },

             _renderProfile: function (data) {
                 this.placeHolder.html('');

                 var rendered = $.tmpl(this.template, data || this._getDefaultProfile());
                 rendered.find('#name').enabled(!this._isEditMode());

                 var that = this;

                 this.preloader = rendered.find('span.preloader');
                 this.successfulConnection = rendered.find('#successfulConnection');
                 this.portInput = rendered.find('#Port');
                 this.portInput.numeric({ negative: false });
                 this.protocolSelect = rendered.find('#Protocol');
                 this.protocolSelect.change(function() { that._setPort(); });

                 rendered.appendTo(this.placeHolder);

                 $('#linkSample').click(function () {
                     $('#ruleDescription').animate({ opacity: 'toggle', height: 'toggle' }, 'slow');
                 });

                 this._initUseSsl(data, rendered);

                 this._initCheckConnectionButton();

                 $('#Rules').defaultValue();
                 this._setFocus();

                 new profileControlsBlock({ placeholder: rendered }).render();
             },

             _isImapSelected: function() {
                 return this.protocolSelect.val() === 'imap';
             },

             _initCheckConnectionButton: function () {
                 var btn = this.placeHolder.find("#checkConnection");
                 btn.click($.proxy(this._checkConnection, this));
             },

             _initUseSsl: function (data, rendered) {
                 this.useSsl = data == null || data.Settings == null ? false : !!data.Settings.UseSSL;

                 rendered.find('#switch').iphoneSwitch(
                    data != null && data.Settings.UseSSL ? 'on' : 'off',
                    $.proxy(this._onSwitchOn, this),
                    $.proxy(this._onSwitchOff, this),
                    {
                        switch_on_container_path: '../javascript/tau/css/images/plugins/switch_on.png',
                        switch_off_container_path: '../javascript/tau/css/images/plugins/switch_off.png',
                        switch_path: '../javascript/tau/css/images/plugins/switch.png'
                    }
                );
             },

             _setPort: function () {
                 var port = this._isImapSelected()
                     ? this.useSsl ? 993 : 143
                     : this.useSsl ? 995 : 110;

                 this.portInput.val(port);
             },

             _onSwitchOn: function (el) {
                 this.useSsl = true;
                 this._setPort();
             },

             _onSwitchOff: function (el) {
                 this.useSsl = false;
                 this._setPort();
             },

             _setFocus: function (profile) {
                 var nameInput = this.placeHolder.find('#Name');
                 if (this._isEditMode()) {
                     this.placeHolder.find('#Rules').focus();
                     nameInput.enabled(false);
                 }
                 else {
                     nameInput.focus();
                 }
             },

             _find: function (selector) {
                 return this.placeHolder.find(selector);
             },

             _isEditMode: function () {
                 return this.repository.getCurrentProfileName() != null;
             },

             _saveProfile: function () {
                 this._hideConnectionSuccess();

                 var profile = this._getProfileFromEditor();

                 if (this._isEditMode()) {
                     this.repository.update(profile);
                 }
                 else {
                     this.repository.create(profile);
                 }
             },

             _onSaveSuccess: function () {
                 this.placeHolder.find('#Name').enabled(false);
             },

             _showError: function (index, error) {
                 this._find('*[name="' + error.FieldName + '"]').addClass('error');
                 this.placeHolder.find('*[name="' + error.FieldName + 'ErrorLabel"]').html(error.Message);
             },

             _checkConnection: function (e) {
                 e.preventDefault();
                 this._hideConnectionSuccess();
                 this.checkConnectionErrorMessageContainer.clearErrors();

                 this.preloader.show();

                 var profile = this._getProfileFromEditor();
                 this.commandGateway.execute("CheckConnection", profile, $.proxy(this._onCheckConnectionSuccess, this), $.proxy(this._onCheckConnectionError, this), $.proxy(this._onCheckConnectionError, this));
             },

             _onCheckConnectionSuccess: function (data) {
                 if (data != null && data.length > 0) {
                     this.checkConnectionErrorMessageContainer.addRange(data);
                     this.checkConnectionErrorMessageContainer.render();
                 }
                 else {
                     this._showConnectionSuccess();
                 }

                 this.preloader.hide();
             },

             _onCheckConnectionError: function (responseText) {
                 this.checkConnectionErrorMessageContainer.add({FieldName: null, Message: responseText});
                 this.checkConnectionErrorMessageContainer.render();
                 this.preloader.hide();
             },

             _showConnectionSuccess: function () {
                 this.successfulConnection.show();
             },

             _hideConnectionSuccess: function () {
                 this.successfulConnection.hide();
             },

             _getProfileFromEditor: function () {
                 return {
                     Name: this._find('#Name').val(),
                     Settings: {
                         Login: this._find('#Login').val(),
                         Password: this._find('#Password').val(),
                         Protocol: this._find('#Protocol').val(),
                         Port: this._getSetPort(),
                         UseSSL: this._getSetUseSsl(),
                         Rules: escape(this._find('#Rules').val()),
                         MailServer: this._find('#MailServer').val()
                     }
                 };
             },

             _getSetPort: function () {
                 var port = this._find('#Port').val();
                 if (typeof (port) == 'undefined' || port == '')
                     port = 0;
                 return port;
             },

             _getSetUseSsl: function () {
                 var state = 'off';
                 this._find('#switch').iphoneSwitch(function () {
                     state = this.getState();
                 });

                 return state == 'on';
             }
         };
         return emailProfileEditor;
     }
    );

