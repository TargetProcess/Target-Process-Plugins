tau.mashups
    .addDependency("Subversion/UserMappingEditor")
    .addDependency("tp/bus")
    .addDependency("tp/plugins/profileControlsBlock")
    .addDependency("libs/jquery/jquery")
    .addModule("Git/ProfileEditor", function (UserMappingEditor, Bus, profileControlsBlock, $) {

        function ProfileEditor(config) {
            this._create(config);
        }

        ProfileEditor.prototype = {
            template: null,
            placeHolder: null,
            saveBtn: null,
            preloader: null,
            editorTemplate:
                '<div>' +
                '	<h2 class="h2">Git Integration</h2>' +
                '	<p class="note">' +
                '		Exports revisions from Git and binds source code to TargetProcess user stories, bugs and' +
                '		tasks.</p>' +
                '	<div class="svn-settings">' +
                '   <div class="pad-box">' +
                '       <p class="label">Profile Name&nbsp;<span class="error" name="NameErrorLabel"></span></p>' +
                '       <p class="note"><span class="small">Should be unique. Can\'t be changed later.</span></p>' +
                '       <input type="text" id="name" name="Name" value="${Name}" class="input" style="width: 275px;" />' +
                '   </div>' +
                '			<div class="separator"></div>' +
                '			<div class="pad-box">' +
                '				<h3 class="h3">' +
                '					Repository Settings</h3>' +
                '				<p class="label">' +
                '					Enter a full path to the repository&nbsp;<a id="uriExamplesLink" class="small" href="javascript:void(0);">Examples</a>' +
                '               <span class="error" name="UriErrorLabel"></span></p>' +
                '               <div id="uriExamplesContent" style="display:none" class="small pt-10 pb-10"><p class="label pb-5">URL examples:</p><p class="rules-actions">git://github.com/Company/Project.git</p><p class="rules-actions">https://github.com/Company/Project.git</p><p class="rules-actions">git@github.com:Company/Project.git</p></div><p/> ' +
                '				<input type="text" class="input" name="Uri" id="uri" value="${Settings.Uri}" style="width: 100%;" /><br />' +
                '               <p class="label"></p>' +
                '               <div class="controls-inline-group" style="margin-top: 18px;">' +
                '                   <button class="tau-btn i-role-http">Use Login and Password</button><button class="tau-btn i-role-ssh">Use SSH Keys</button>' +                    
                '               </div>' +                
                '               <div class="i-role-loginBlock">' +
                '				    <p class="label">' +
                '   				<p class="label pt-10">' +
                '	    				Login&nbsp;<span class="error" name="LoginErrorLabel"></span></p>' +
                '		    		<input type="text" class="input" id="login" name="Login" value="${Settings.Login}" style="width: 275px;" />' +
                '			    	<p class="label pt-10">' +
                '				    	Password&nbsp;<span class="error" name="PasswordErrorLabel"></span></p>' +
                '   				<input type="password" class="input" id="password" name="Password" value="${passwordValue}" style="width: 275px;" />' +
                '               </div>' +
                '               <div class="i-role-sshKeysBlock">' +
                '                  <div style="margin-top: 10px; margin-bottom: 15px;">' +
                '                      <a target="_blank" href="https://www.targetprocess.com/guide/integrations/integrations-source-control/issue-ssh-key-pair-from-git-bash-use-in-targetprocess/">How to issue a new SSH key pair from Git Bush and use it in Targetprocess?</a>' +
                '                  </div>' +
                '                  <div>' +
                '                       <button class="tau-btn i-role-sshGenerateKeys" style="margin-top: 12px; margin-bottom: 5px; display: inline-block">Generate keys for me</button>' +
                '                       <span class="preloader i-role-sshGenerateKeysProgress" style="display:none"></span>' +
                '                       &nbsp;&nbsp;<span class="label"><span class="small">or upload keys in OpenSSH format</span></span>' +
                '                  </div>' +
                '                   <p class="label pt-10">' +
                '                       Public Key <a href="javascript:void(0);" class="small i-role-sshUploadPublicKey">Upload file</a>' +
                '                       <span style="margin-left: 10px;" class="yes-message i-role-publicKeyAddedFlag">Added</span>' +
                '                       <span style="margin-left: 10px;" class="no-message i-role-publicNoKeyFlag">No key</span>' +
                '                       &nbsp;<span class="error" name="SshPublicKeyErrorLabel"></span>' +
                '                   </p>' +
                '                  <textarea rows="10" readonly disabled="disabled" class="i-role-sshPublicKey input" style="width: 100%" name="SshPublicKey"></textarea>' +
                '                   <p class="label pt-10">' +
                '                       Private Key <a href="javascript:void(0);" class="small i-role-sshUploadPrivateKey">Upload file</a>' +
                '                       <span style="margin-left: 10px;" class="yes-message i-role-privateKeyAddedFlag">Added</span>' +
                '                       <span style="margin-left: 10px;" class="no-message i-role-privateNoKeyFlag">No key</span>' +
                '                       &nbsp;<span class="error" name="SshPrivateKeyErrorLabel"></span>' +
                '                   </p>' +                
                '               </div>' +
                '				<p class="label pt-20">' +
                '					Export all the revisions starting from&nbsp;&nbsp;<input id="startRevision" name="StartRevision" value="${Settings.StartRevision}" type="text" class="input"' +
                '						style="width: 97px;" /><span class="error" name="StartRevisionErrorLabel"></span></p>' +
                '				<p class="label pt-20">Sync every <input id="syncInterval" name="SyncInterval" value="${Settings.SyncInterval}" type="text" class="input" style="width: 97px;" /> minutes<span class="error" name="SyncIntervalErrorLabel"></span></p>' +
                '			</div>' +
                '			<div class="check-block">' +
                '				<p class="message-error pb-10" style="display: none;">' +
                '					Login failed. You have entered incorrect or non-existent login.</p>' +
                '               <p class="error-message" id="failedConnection" style="display:none"><span></span></p>' +
                '				<a href="javascript:void(0);" id="checkConnection" class="check-connection-link">Check Connection</a><span class="preloader i-role-checkConnectionProgress" style="display:none"></span>' +
                '               <a href="javascript:void(0);" id="rescan" class="rescan-link" title="Sometimes Git might skip some revisions. This action will rescan the Git repository and import any missing commits. This may take some time.">Rescan</a>' +
                '			</div>' +
                '		</div>' +
                '	<div class="svn-settings">' +
                '		<h3 class="collapsable">User Mapping</h3>' +
                '		<div class="svn-map-users">\n</div>' +
                '	</div>' +
                '	<div class="controls-block"></div>' +
                '</div>',

            _create: function (config) {
                this.placeHolder = config.placeHolder;
                this.model = config.model;

                this.model.passwordValue = this.model.Settings.HasPassword ? '0000000000000000' : '';
                this._passwordChanged = false;
                this._sshPrivateKeyChanged = false;
                this._sshPrivateKey = null;                

                this.controller = config.controller;
                Bus.subscribe("SubversionProfileEditor", {
                    onCheckConnectionForced: $.proxy(this._onCheckConnectionForced, this),
                    onCheckConnectionInitiated: $.proxy(this.onCheckConnectionInitiated, this),
                    onCheckConnectionSuccessful: $.proxy(this.onCheckConnectionSuccessful, this),
                    onCheckConnectionFailed: $.proxy(this.onCheckConnectionFailed, this),
                    onCheckConnectionError: $.proxy(this.onCheckConnectionError, this),
                    onSaveProfile: $.proxy(this._onSave, this),
                    onProfileSaveSucceed: $.proxy(this._onSaveSucceed, this)
                }, true);
            },

            onCheckConnectionInitiated: function () {
                this.clearErrors();
                this.placeHolder.find('#failedConnection').hide().find('span').text('');
                this.checkConnectionInProgress(true);
            },

            onCheckConnectionSuccessful: function () {
                this.checkConnectionInProgress(false);
                this.checkConnectionErrors([]);
            },

            onCheckConnectionFailed: function (args) {
                this.checkConnectionInProgress(false);

                var errors = args;
                this.checkConnectionErrors(errors);
                var message = (args[0] && args[0].Message) || 'Unable to establish connection';
                this.placeHolder.find('#failedConnection').show().find('span').text(message);
            },

            onCheckConnectionError: function (responseText) {
                this.checkConnectionInProgress(false);

                this.placeHolder.find('#failedConnection').show().find('span').text(responseText);
            },

            render: function () {
                this.placeHolder.html('');
                var rendered = $.tmpl(this.editorTemplate, this.model);
                
                rendered.find("#password").change(function() {
                    this._passwordChanged = true;
                }.bind(this));

                this.httpButton = rendered.find('.i-role-http');
                this.sshButton = rendered.find('.i-role-ssh');

                this.httpButton.click(function (e) {
                    e.preventDefault();
                    this._toggleSsh(false);
                }.bind(this));

                this.sshButton.click(function(e) {
                    e.preventDefault();
                    this._toggleSsh(true);
                }.bind(this));

                this.sshPublicKey = rendered.find('.i-role-sshPublicKey');
                this.sshPublicKey.text(this.model.Settings.SshPublicKey);                

                this.loginBlock = rendered.find('.i-role-loginBlock');
                this.sshKeysBlock = rendered.find('.i-role-sshKeysBlock');

                this._toggleSsh(this.model.Settings.UseSsh);

                rendered.find('.i-role-sshGenerateKeys').click(this._sshGenerateKeys.bind(this));
                rendered.find('.i-role-sshUploadPublicKey').click(this._sshUploadKey.bind(this, false));
                rendered.find('.i-role-sshUploadPrivateKey').click(this._sshUploadKey.bind(this, true));                
                
                this.checkConnectionBtn = rendered.find('#checkConnection');
                this.checkConnectionBtn.click($.proxy(this._onCheckConnection, this));
                this.preloader = rendered.find('span.preloader.i-role-checkConnectionProgress');

                this.rescanButton = rendered.find('#rescan');
                this.rescanButton.click($.proxy(this._onRescan, this));
                if (!this.model.Name) {
                    this.rescanButton.hide();
                }

                rendered.appendTo(this.placeHolder);

                rendered.find('#uriExamplesLink').click(function () {
                    $('#uriExamplesContent').animate({ opacity: 'toggle', height: 'toggle' }, 'slow');
                });
                var $syncIntervalField = rendered.find('#syncInterval');
                $syncIntervalField.blur(this._limitSyncInterval.bind(this, $syncIntervalField));
                this._limitSyncInterval($syncIntervalField);

                this._disableNameIfNecessary();

                this.UserMappingEditor = new UserMappingEditor({
                    placeHolder: this.placeHolder.find('.svn-map-users'),
                    model: this.model
                });

                this.UserMappingEditor.render();

                this.placeHolder.on('click', '.collapsable', this._toggle);
                this.placeHolder.find('#startRevision').datepicker({
                    dateFormat: 'MM/dd/yy',
                    minDate: '01/01/1970',
                    maxDate: '01/19/2038'
                });

                new profileControlsBlock({ placeholder: rendered }).render();

                this._updateSshFlags();
            },

            _limitSyncInterval: function ($syncIntervalField) {
            	var value = parseInt($syncIntervalField.val(), 10) || 0;
            	var minimumSyncInterval = 5;
            	if (value < minimumSyncInterval) {
            		$syncIntervalField.val(minimumSyncInterval);
            	}
            },

            _disableNameIfNecessary: function () {
                this.placeHolder.find('#name').enabled(!this.controller.isEditMode());
            },

            _toggle: function () {
                $(this).toggleClass('collapsed').toggleClass('expanded');
                $(this).next().slideToggle('fast');
            },

            _onSave: function () {
                this.clearErrors();
                this._clientValidate();

                if (this._getValidationErrorsCount() > 0) {
                    Bus.publish('ProfileSaveFailed');
                    return;
                }

                this.controller.save(this, this._getProfileFromEditor());
            },

            _onSaveSucceed: function () {
                this.placeHolder.find('#name').enabled(false);
                this.rescanButton.show();
            },

            _onCheckConnectionForced: function () {
                this._onCheckConnection({ preventDefault: function () {
                } });
            },

            _onCheckConnection: function (e) {
                e.preventDefault();

                var btn = this.checkConnectionBtn;
                btn.data('ui-success') && btn.success('clear');
                Bus.publish("CheckConnectionCommand", [this._getProfileFromEditor()]);
            },

            _onRescan: function(e) {
                e.preventDefault();

                Bus.publish("RescanRepositoryCommand", []);
            },

            _clientValidate: function () {
                this.UserMappingEditor.clientValidate();
            },

            _getValidationErrorsCount: function () {
                return this.UserMappingEditor.getValidationErrorsCount();
            },

            _getProfileFromEditor: function () {
                return {
                    Name: this._find('#name').val(),
                    Settings: {
                        Uri: this._find('#uri').val(),
                        Login: this._find('#login').val(),
                        Password: this._passwordChanged ? this._find('#password').val() : null,
                        StartRevision: this._find('#startRevision').val(),
                        SyncInterval: this._find("#syncInterval").val(),
                        UserMapping: this.UserMappingEditor.getUserMappings(),
                        UseSsh: this.sshButton.hasClass('tau-checked'),
                        SshPublicKey: this.sshPublicKey.text(),
                        SshPrivateKey: this._sshPrivateKeyChanged ? this._sshPrivateKey : null
                    }
                };
            },

            _find: function (selector) {
                return this.placeHolder.find(selector);
            },

            showErrors: function (data) {
                this.clearErrors();

                var placeHolder = this.placeHolder;

                $.each(data, function (index, error) {
                    placeHolder.find('*[name="' + error.FieldName + '"]').error({ message: error.Message });
                });
            },

            clearErrors: function () {
                this.placeHolder.find('.ui-error').error('clear');
            },

            checkConnectionInProgress: function (value) {
                if (value) {
                    this.preloader.show();
                } else {
                    this.preloader.hide();
                }
            },

            checkConnectionErrors: function (errors) {
                this.showErrors(errors);

                if ($(errors).length == 0) {
                    this.checkConnectionBtn.success();
                }
            },               

            _sshGenerateKeys: function(e) {
                e.preventDefault();
                var progress = this._find('.i-role-sshGenerateKeysProgress');
                progress.show();
                this.controller.executeCommand("GenerateSshKeys", [], function(response) {
                    progress.hide();
                    this.sshPublicKey.text(response.publicKey);
                    this._sshPrivateKey = response.privateKey;
                    this._sshPrivateKeyChanged = true;
                    this._updateSshFlags();
                }.bind(this));
            },

            _sshUploadKey: function(isPrivate, e) {
                e.preventDefault();

                var input = document.createElement("input");
                input.onchange = function() {
                    var file = input.files[0];
                    var fileReader = new FileReader();
                    fileReader.onloadend = function(evt) {
                        if (evt.target.readyState === FileReader.DONE) {                                                        
                            if (isPrivate) {
                                this._sshPrivateKeyChanged = true;
                                this._sshPrivateKey = evt.target.result;
                            } else {
                                this.sshPublicKey.text(evt.target.result);
                            }
                            this._updateSshFlags();
                        }
                    }.bind(this);
                    fileReader.readAsBinaryString(file);
                }.bind(this);

                var $input = $(input);
                $input.attr("type", "file");
                $input.trigger("click");
            },

            _toggleSsh: function(useSsh) {
                this.httpButton.removeClass('tau-checked');
                this.sshButton.removeClass('tau-checked');
                if (useSsh) {
                    this.sshButton.addClass('tau-checked');
                    this.loginBlock.hide();
                    this.sshKeysBlock.show();
                } else {
                    this.httpButton.addClass('tau-checked');
                    this.sshKeysBlock.hide();
                    this.loginBlock.show();
                }
            },

            _updateSshFlags: function () {
                this._updatePublicSshKeyFlags();
                this._updatePrivateSshKeyFlags();
            },

            _updatePublicSshKeyFlags: function() {
                var keyAddedFlag = this._find('.i-role-publicKeyAddedFlag');
                var noKeyFlag = this._find('.i-role-publicNoKeyFlag');

                keyAddedFlag.hide();
                noKeyFlag.hide();
                if (this.sshPublicKey.text().length > 0) {
                    keyAddedFlag.show();
                } else {
                    noKeyFlag.show();
                }
            },

            _updatePrivateSshKeyFlags: function () {
                var keyAddedFlag = this._find('.i-role-privateKeyAddedFlag');
                var noKeyFlag = this._find('.i-role-privateNoKeyFlag');

                keyAddedFlag.hide();
                noKeyFlag.hide();
                if (this.model.Settings.HasSshPrivateKey || (this._sshPrivateKey && this._sshPrivateKey.length > 0)) {
                    keyAddedFlag.show();
                } else {
                    noKeyFlag.show();
                }
            }
        };
        return ProfileEditor;
    });
