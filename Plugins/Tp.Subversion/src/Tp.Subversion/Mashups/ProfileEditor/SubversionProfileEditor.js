tau.mashups
	.addDependency("Subversion/UserMappingEditor")
	.addDependency("tp/bus")
	.addDependency("tp/plugins/profileControlsBlock")
	.addDependency("libs/jquery/jquery")
	.addModule("Subversion/SubversionProfileEditor", function (UserMappingEditor, Bus, profileControlsBlock) {

	    function SubversionProfileEditor(config) {
	        this._create(config);
	    }

	    SubversionProfileEditor.prototype = {
	        template: null,
	        placeHolder: null,
	        saveBtn: null,
	        preloader: null,
	        editorTemplate:
				'<div>' +
				'		<h2 class="h2">' +
					'			Subversion Plugin</h2>' +
					'		<p class="note">' +
					'			Exports revisions from Subversion and binds source code to TargetProcess user stories, bugs and tasks.</p>' +
					'		<div class="svn-settings">' +
					'   <div class="pad-box">' +
					'       <p class="label">Profile Name&nbsp;<span class="error" name="NameErrorLabel"></span></p>' +
                    '       <p class="note"><span class="small">Should be unique. Can\'t be changed later.</span></p>' +
					'       <input type="text" id="name" name="Name" value="${Name}" class="input" style="width: 275px;" />' +
					'   </div>' +
					'			<div class="separator">' +
					'			</div>' +
					'			<div class="pad-box">' +
					'				<h3 class="h3">' +
					'					Repository Settings</h3>' +
					'				<p class="label">' +
					'					Enter a full path to the repository&nbsp;<span class="small">(example: https://svn.targetprocess.com/svn/public)</span>&nbsp;<span class="error" name="UriErrorLabel"></span></p>' +
					'				<input type="text" class="input" name="Uri" id="uri" value="${Settings.Uri}" style="width: 100%;" /><br />' +
					'				<p class="label pt-10">' +
					'					Login&nbsp;<span class="error" name="LoginErrorLabel"></span></p>' +
					'				<input type="text" class="input" id="login" name="Login" value="${Settings.Login}" style="width: 275px;" />' +
					'				<p class="label pt-10">' +
					'					Password&nbsp;<span class="error" name="PasswordErrorLabel"></span></p>' +
					'				<input type="password" class="input" id="password" name="Password" value="${passwordValue}" style="width: 275px;" />' +
					'				<p class="label pt-20">' +
					'					Export all the revisions starting from&nbsp;&nbsp;<input id="startRevision" name="StartRevision" value="${Settings.StartRevision}" type="text" class="input"' +
					'						style="width: 97px;" />&nbsp;&nbsp;revision&nbsp;<span class="error" name="StartRevisionErrorLabel"></span></p>' +
					'			</div>' +
					'			<div class="check-block">' +
					'				<p class="message-error pb-10" style="display: none;">' +
					'					Login failed. You have entered incorrect or non-existent login.</p>' +
					'               <p class="error-message" id="failedConnection" style="display:none"><span></span></p>' +
					'				<a href="javascript:void(0);" id="checkConnection" class="check-connection-link">Check Connection</a><span class="preloader" style="display:none"></span>' +
					'			</div>' +
					'		</div>' +
					'<div class="svn-settings">' +
						'<h3 class="collapsable">User Mapping</h3>' +
						'<div class="svn-map-users">\n</div>' +
					'</div>' +

					'       <div class="controls-block">' +
					'        </div>' +
					'</div>',

	        _create: function (config) {
	            this.placeHolder = config.placeHolder;
	            this.model = config.model;
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
	            this.placeHolder.find('#failedConnection').show().find('span').text('Unable to establish connection');
	        },

	        onCheckConnectionError: function (responseText) {
	            this.checkConnectionInProgress(false);

	            this.placeHolder.find('#failedConnection').show().find('span').text(responseText);
	        },

	        render: function () {
                this.placeHolder.html('');

                this.model.passwordValue = this.model.Settings.HasPassword ? '0000000000000000' : '';
                this._passwordChanged = false;

                var rendered = $.tmpl(this.editorTemplate, this.model);

                rendered.find("#password").change(function () {
                    this._passwordChanged = true;
                }.bind(this));

	            this.checkConnectionBtn = rendered.find('#checkConnection');
	            this.checkConnectionBtn.click($.proxy(this._onCheckConnection, this));
	            this.preloader = rendered.find('span.preloader');

	            rendered.appendTo(this.placeHolder);

	            this._disableNameIfNecessary();

	            this.UserMappingEditor = new UserMappingEditor({
	                placeHolder: this.placeHolder.find('.svn-map-users'),
	                model: this.model
	            });

	            this.UserMappingEditor.render();

	            this.placeHolder.find('.collapsable').on('click', this._toggle);

	            new profileControlsBlock({ placeholder: rendered }).render();
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
	        },

	        _onCheckConnectionForced: function () {
	            this._onCheckConnection({ preventDefault: function () { } });
	        },

	        _onCheckConnection: function (e) {
	            e.preventDefault();

	            var btn = this.checkConnectionBtn;
	            btn.data('ui-success') && btn.success('clear');
	            Bus.publish("CheckConnectionCommand", [this._getProfileFromEditor()]);
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
	                    UserMapping: this.UserMappingEditor.getUserMappings()
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
	        }
	    };
	    return SubversionProfileEditor;
	});
