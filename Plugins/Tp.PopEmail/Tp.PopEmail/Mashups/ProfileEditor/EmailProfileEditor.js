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
    .addDependency("emailIntegration/openidconnect")
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
                     passwordValue : '',
                     Settings: {
                         SecureAccessMethod: 0,
                         Login: '',
                         Password: '',
                         Protocol: '',
                         Port: 110,
                         UseSSL: false,
                         Rules: null,
                         MailServer: '',
                         UsersMigrated: true,
                         OAuthDiscoverUri: null,
                         OAuthState: null
                     }
                 };
             },

             _renderProfile: function (data) {
                 this.placeHolder.html('');

                 if (data) {
                     data.passwordValue = data.Settings.HasPassword ? '0000000000000000' : '';
                     this._oauthState = data.Settings.OAuthState;
                 }
                 this._passwordChanged = false;

                 var rendered = $.tmpl(this.template, data || this._getDefaultProfile());
                 rendered.find('#name').enabled(!this._isEditMode());

                 var that = this;

                 this.preloader = rendered.find('span.preloader');
                 this.successfulConnection = rendered.find('#successfulConnection');
                 this.portInput = rendered.find('#Port');
                 this.portInput.numeric({ negative: false });
                 this.protocolSelect = rendered.find('#Protocol');
                 this.protocolSelect.change(function() { that._setPort(); });
                 this.password = rendered.find('#Password');
                 this.password.change(function() { that._passwordChanged = true; });
                 this.authSelect = rendered.find('#authDropDown');
                 this.authSelect.change($.proxy(this._authChange, this));
                 this.sign = rendered.find('#SignIn');
                 this.sign.click($.proxy(this._sign, this));

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
                        switch_on_container_path: '../img/plugins/switch_on.png',
                        switch_off_container_path: '../img/plugins/switch_off.png',
                        switch_path: '../img/plugins/switch.png'
                    }
                );
             },

             _initClient: function (code, accessToken, idToken) {
                 if (code) {
                     var tokenEndpoint = OIDC['token_endpoint'];

                     var formData =
                         'grant_type=' + encodeURIComponent('authorization_code')
                         + '&client_id=' + encodeURIComponent(this._find('#Login').val())
                         + '&client_secret=' + encodeURIComponent(this.password.val())
                         + '&code=' + code
                         + '&redirect_uri=' + encodeURIComponent(OIDC['redirect_uri']);

                     $.ajax({
                         type: 'POST',
                         url: tokenEndpoint,
                         contentType: 'application/x-www-form-urlencoded;charset=UTF-8',
                         data: formData,
                         success: $.proxy(function (resp) {
                             if (OIDC.isValidIdToken(resp.id_token) === true) {
                                 var idtParts = OIDC.getIdTokenParts(resp.id_token);
                                 var payload = OIDC.getJsonObject(idtParts[1]);
                                 $('#Login').next('span').text(payload.email);
                                 if (resp.refresh_token) {
                                     this.sign.addClass('tau-success').val('Sign out');
                                     this._oauthState = {
                                         Email: payload.email,
                                         TokenEndpoint: tokenEndpoint,
                                         RefreshToken: resp.refresh_token,
                                         AccessToken: resp.access_token,
                                         AccessTokenExpirationUtc: '\/Date(' + new Date(payload.exp * 1000).valueOf() + ')\/',
                                         AccessTokenIssueDateUtc: '\/Date(' + new Date(payload.iat * 1000).valueOf() + ')\/',
                                         Scope: resp.scope
                                     };
                                 } else {
                                     this._onCheckConnectionError(
                                         'The refresh_token is only provided on the first authorization from ' + (payload.email ? payload.email : 'the user') + '. Remove access for your app from the Third-party apps and try again.');
                                 }
                             }
                         }, this),
                         error: $.proxy(function(jqXhr) {
                             if (jqXhr) {
                                 this._showError(0, { FieldName: 'Password', Message: jqXhr.responseJSON.error_description });
                             }
                         }, this)
                     });
                 }
             },

             _sign: function () {
                 if (this._oauthState) {
                     this.sign.removeClass('tau-success').val('Sign in');
                     $('#Login').next('span').text('');
                     this._oauthState = null;
                     return;
                 }
                 if (!this.authSelect.val())
                     return;
                 var login = this._find('#Login');
                 var clientId = login.val();
                 if (!clientId || clientId.length === 0) {
                     this._showError(0, { FieldName: 'Login', Message: 'Client id should not be empty' });
                 }
                 var clientSecret = this.password.val();
                 if (!clientSecret || clientSecret.length === 0) {
                     this._showError(0, { FieldName: 'Password', Message: 'Client secret should not be empty' });
                 } else if (clientSecret === '0000000000000000' && !this._passwordChanged) {
                     this._showError(0, { FieldName: 'Password', Message: 'Client secret should be reentered' });
                 }
                 if (clientId && clientId.length > 0 && clientSecret && clientSecret.length && (clientSecret !== '0000000000000000' || this._passwordChanged)) {
                     this.checkConnectionErrorMessageContainer.clearErrors();
                     var url = new Tp.URL(location.href);
                     var redirectUri = new Tp.WebServiceURL('/Admin/Plugins.aspx');
                     redirectUri.host = url.host;
                     redirectUri.port = url.port;
                     redirectUri.portSep = url.portSep;
                     redirectUri.protocol = url.protocol;
                     redirectUri.protocolSep = url.protocolSep;
                     var clientInfo = {
                         client_id: clientId,
                         redirect_uri: redirectUri.toString()
                     };

                     OIDC.setClientInfo(clientInfo);

                     var selected = this.authSelect.find('option:selected');
                     var providerInfo = OIDC.discover(selected.attr('url'));
                     OIDC.setProviderInfo(providerInfo);
                     OIDC.storeInfo(providerInfo, clientInfo);
                     var scope = selected.attr('scope') +
                         ' ' + (this._isImapSelected() ? selected.attr('imap') : selected.attr('pop3'));
                     OIDC.login({
                         scope: scope,
                         response_type: 'code',
                         access_type: 'offline',
                         prompt: 'consent',
                         max_age: 3600
                     }, $.proxy(this._initClient, this));
                 } else {
                     if (!clientId || clientId.length === 0) {
                         login.focus();
                         return;
                     }
                     if (!clientSecret || clientSecret.length === 0 || clientSecret === '0000000000000000' && !this._passwordChanged) {
                         this.password.focus();
                         return;
                     }
                 }
             },

             _authChange: function (e) {
                 this.checkConnectionErrorMessageContainer.clearErrors();
                 var select = $(e.target);
                 var login = this._find('#Login');
                 login.next('span').text('');
                 this._oauthState = null;
                 if (!select.val()) {
                     this.sign.hide();
                     login.prevAll('p.label:first').contents()[0].nodeValue = 'Login\u00a0';
                     this.password.prevAll('p.label:first').contents()[0].nodeValue = 'Password\u00a0';
                 } else {
                     this.sign.show().removeClass('tau-success').val('Sign in');
                     login.prevAll('p.label:first').contents()[0].nodeValue = 'Client id\u00a0';
                     this.password.prevAll('p.label:first').contents()[0].nodeValue = 'Client secret\u00a0';
                 }
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
                         SecureAccessMethod: this._find('#authDropDown').val() || 0,
                         Login: this._find('#Login').val(),
                         Password: this._passwordChanged ? this._find('#Password').val() : null,
                         Protocol: this._find('#Protocol').val(),
                         Port: this._getSetPort(),
                         UseSSL: this._getSetUseSsl(),
                         Rules: escape(this._find('#Rules').val()),
                         MailServer: this._find('#MailServer').val(),
                         UsersMigrated: this._find('#UsersMigrated').val(),
                         OAuthDiscoverUri: this._find('#authDropDown').find('option:selected').attr('url'),
                         OAuthState: this._oauthState
                     }
                 };
             },

             _getSetPort: function () {
                 var port = this._find('#Port').val();
                 if (typeof (port) == 'undefined' || port === '')
                     port = 0;
                 return port;
             },

             _getSetUseSsl: function () {
                 var state = 'off';
                 this._find('#switch').iphoneSwitch(function () {
                     state = this.getState();
                 });

                 return state === 'on';
             }
         };
         return emailProfileEditor;
     });
