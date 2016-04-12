//If you rename or remove this file, it will be re-created during package update.
tau.mashups
.addDependency('tp/bus')
.addDependency('app.path')
.addDependency('tp/status')
.addDependency('tp/plugins/errorMessageContainer')
.addDependency('tp/plugins/profileControlsBlock')
.addDependency('libs/jquery/jquery')
.addDependency('libs/jquery/jquery.tmpl')
.addDependency('tau/mashups/TpSearch/ProfileEditor/editorTmpl')
.addDependency('tp/plugins/profileNameSource')
.addDependency('tp/plugins/profileRepository')
.addDependency('tp/plugins/commandGateway')
.addModule('tau/mashups/TpSearch/ProfileEditor/searcherProfileEditor', function (bus, appPath, status, errorMessageContainer, profileControlsBlock, $,
	jqueryTmpl, editorTmpl, profileNameSource, profileRepository, commandGateway) {
	function searcherProfileEditor(config) {
		this._create(config);
	}

	searcherProfileEditor.prototype = {
		template: null,
		placeHolder: null,
		controlsBlock: null,
		_pluginName: null,
		_status: null,
		_bus: null,

		_create: function (config) {
		    this._commandGateway = new commandGateway();
			this.placeHolder = config.placeHolder;
			this._status = status;
			this._bus = bus;

			this._bus.subscribe('SearcherProfileEditor', {
				onSaveProfile: $.proxy(this._saveProfile, this),
				onProfileSaveSucceed: $.proxy(this._onProfileSaved, this)
			}, true);

			this.template = editorTmpl;
		},

		render: function () {
			this._pluginName = profileNameSource.getPluginName();

			$.ajax({
				url: appPath.get() + '/api/v1/Plugins.asmx/{PluginName}/Profiles/Now%20running'.replace(/{PluginName}/g, this._pluginName),
				dataType: 'json',
				success: $.proxy(function (res) {
					this._renderProfile(res);
				}, this),
				error: $.proxy(function (response) {
					this._renderPluginFailed(response);
				}, this)
			});
		},

        _renderPluginFailed: function(err) {
            var $page = $([
                '<p>Search plugin is failed with the following error:</p>',
                '<p style="background-color: #f7e0e0; padding: 5px">',
                err.responseText || JSON.stringify(err),
                '</p>'
            ].join(''));
            $page.appendTo(this.placeHolder);
        },

		_renderProfile: function (profile) {
			var self = this;

			this.placeHolder.html('');
			var rendered = $.tmpl(this.template, profile);
			this.controlsBlock = new profileControlsBlock({ placeholder: rendered });
			this.controlsBlock.render();
			rendered.find('a#save').text("Rebuild indexes").unbind('click').click(function () {
				if (!$(this).enabled()) {
					return;
				}

				self.controlsBlock.errorMessageContainer.clearErrors();
				$(this).enabled(false);
				var relativeLoaderUrl = appPath.get() + '/Javascript/tau/css/images/loader-yellow.gif';
				self._status.custom('saving', '<img src="' + relativeLoaderUrl + '" /><span>Starting to rebuild indexes...</span>', 30000, 'fast');
				self._bus.publish('SaveProfile');
			});
			rendered.find('#enabled-for-tp2').click(function () {
			    var relativeLoaderUrl = appPath.get() + '/Javascript/tau/css/images/loader-yellow.gif';
			    self._status.custom('saving', '<img src="' + relativeLoaderUrl + '" /><span>Profile is being saved...</span>', 30000, 'fast');
		        self._commandGateway.executeForProfile('SetEnableForTp2',
		            $(this).prop('checked'),
		            $.proxy(self._enableForTp2Succeed, self),
		            $.proxy(self._enableForTp2Failed, self),
		            $.proxy(self._enableForTp2Error, self)
		        );
		    });
			this._bus.unsubscribe('ProfileControlsBlock');
			rendered.appendTo(this.placeHolder);
		},
		_enableForTp2Succeed: function (data) {
		    this._status.success(data);
		},
		_enableForTp2Failed: function (err) {
		    this._status.error(err);
	    },
		_enableForTp2Error: function (err) {
		    this._status.error(err);
	    },
		_isEditMode: function () {
			return profileNameSource.getProfileName() != null;
		},

		_find: function (selector) {
			return this.placeHolder.find(selector);
		},

		_saveProfile: function () {
			var profile = this._getProfileFromEditor();

			if (this._isEditMode()) {
				profileRepository.update(profile);
			}
			else {
				profileRepository.create(profile);
			}
		},

		_onProfileSaved: function () {
			this._status.success('Started to rebuild indexes successfully');
			this._find('a#save').enabled(true);
			this.controlsBlock.activityLog.render();
		},

		_getProfileFromEditor: function () {
			var enabledForTp2CheckBox = this._find('input#enabled-for-tp2');
			return {
				Name: 'Now running',
				Settings: {
					SynchronizationInterval: 5,
					EnabledForTp2: enabledForTp2CheckBox.length > 0 ? enabledForTp2CheckBox.prop('checked') : false
				}
			};
		}
	};

	return searcherProfileEditor;
})
