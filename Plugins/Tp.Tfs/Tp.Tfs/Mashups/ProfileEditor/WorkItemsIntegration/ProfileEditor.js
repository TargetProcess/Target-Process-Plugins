tau.mashups
	.addDependency("Tfs/UserMappingEditor")
    .addDependency("Tfs/EntityMappingEditor")
	.addDependency("tp/bus")
    .addDependency("tp/plugins/profileControlsBlock")
	.addDependency("tp/jquery.utils")
	.addDependency("libs/jquery/jquery.tmpl")
	.addModule("Tfs/ProfileEditor", function (UserMappingEditor, EntityMappingEditor, Bus, profileControlsBlock) {

	    function ProfileEditor(config) {
	        this._create(config);
	    }

	    ProfileEditor.prototype = {
	        template: null,
	        placeHolder: null,
	        profileValidToSave: false,
	        preloader: null,
	        editorTemplate:
				'<div>' +
				'		<h2 class="h2">' +
					'			TFS Integration</h2>' +
					'		<p class="note">' +
					'			Exports revisions from TFS and binds source code to TargetProcess user stories, bugs and' +
					'			tasks.</p>' +
					'		<div class="svn-settings">' +
                            '   <div class="pad-box">' +
                            '       <p class="label">Profile Name&nbsp;<span class="error" name="NameErrorLabel"></span></p>' +
                            '       <p class="note"><span class="small">Should be unique. Can\'t be changed later.</span></p>' +
                            '       <input type="text" id="name" name="Name" value="${Name}" class="input" style="width: 275px;" />' +
                            '   </div>' +
					'			<div class="separator"></div>' +
					'			<div class="pad-box">' +
					'				<h3 class="h3">' +
					'					Server credentials</h3>' +
					'				<p class="label">' +
					'					Enter a full path to the repository&nbsp;<a id="uriExamplesLink" class="small" href="javascript:void(0);">Examples</a>' +
                    '               <span class="error" name="UriErrorLabel"></span></p>' +
                    '               <div id="uriExamplesContent" style="display:none" class="small pt-10 pb-10"><p class="label pb-5">URL examples:<p class="rules-actions">http://ServerName:Port/tfs/DefaultCollection</p><p class="rules-actions">http://ServerName:Port/tfs/DefaultCollection/TeamName</p></div><p/> ' +
					'				<input type="text" class="input" name="Uri" id="uri" value="${Settings.Uri}" style="width: 100%;" /><br />' +
					'				<p class="label">' +
					'				<p class="label pt-10">' +
					'					Login&nbsp;<span class="error" name="LoginErrorLabel"></span></p>' +
					'				<input type="text" class="input" id="login" name="Login" value="${Settings.Login}" style="width: 275px;" />' +
					'				<p class="label pt-10">' +
					'					Password&nbsp;<span class="error" name="PasswordErrorLabel"></span></p>' +
					'				<input type="password" class="input" id="password" name="Password" value="${Settings.Password}" style="width: 275px;" />' +
					'			</div>' +
					'			<div class="check-block">' +
					'				<p class="message-error pb-10" style="display: none;">' +
					'					Login failed. You have entered incorrect or non-existent login.</p>' +
					'               <p class="error-message" id="failedConnection" style="display:none"><span></span></p>' +
    				'           	<p class="warning-message" id="warningConnection" style="display:none"><span></span></p>' +
					'				<a href="javascript:void(0);" id="checkConnection" class="check-connection-link">Check Connection</a><span class="preloader" style="display:none"></span>' +
					'			</div>' +
					'		</div>' +
                    '<div class="pad-box">' +
                        '<p class="error-message" id="integrationsDisabled" style="display:none"><span></span></p>' +
                        '<input type="button" class="source-control-toggle" id="source-control-button"/>' +
                    '</div>' +
					'<div class="svn-settings">' +
                        '<h3 class="collapsable">Source control integration</h3>' +
                        '<div class="source-control-settings">' +
                            '<div class="pad-box">' +
                                '<p class="h3">' +
                                '   Export all the revisions starting from&nbsp;&nbsp;<input id="startRevision" name="StartRevision" value="${Settings.StartRevision}" type="text" class="input"' +
                                '       style="width: 97px;" /><span class="error" name="StartRevisionErrorLabel"></span></p>' +
                                '<h3 class="label pt-20">User Mapping</h3>' +
                                '<div class="svn-map-users" style="width: auto">\n</div>' +
                            '</div>' +
                        '</div>' +
					'</div>' +
                    '<div class="pad-box">' +
                        '<input type="button" class="work-items-toggle" id="workitems-button"/>' +
                    '</div>' +
    				'<div class="workitems-settings" id="workitems-box">' +
	                    '<h3 class="collapsable">Work Items integration</h3>' +
	        //    	                '<table style="border-width: 1px">' +
	        //        	                '<tr>' +
	        //            	                '<td><input type="checkbox" style="padding: 0px" /></td>' +
	        //                	            '<td><h3>Work Items integration</h3></td>' + 
	        //                    	        '<td><a class="showLog">Work Items integration</a></td>' +    
	        //            	            '</tr>' +
	        //            	        '</table>' +
                        '<div class="pad-box">' +
                            '<p class="h3" style="margin-bottom: 15px">' +
                                'Export all the workitems starting from&nbsp;&nbsp;<input id="startWorkItem" name="StartWorkItem" value="${Settings.StartWorkItem}" type="text" class="input"' +
                                'style="width: 97px;" /><span class="error" name="StartWorkItemErrorLabel"></span>' +
                            '</p>' +
                            '<div class="separator"></div>' +
                            '<h3 class="label pt-20">Projects Mapping</h3>' +
                            '<div class="pad-box">' +
                                '<p class="error-message pb10" id="projectsMappingError" style="display:none"><span></span></p>' +
                                '<p class="note"><span class="small">Select pair of projects here (Example: TFS project -> TP project)</span></p>' +
                            '<ul class="projects-mapping" style="width: auto">' +
                                '<li><p class="label pt-10"> Team Projects </p></li>' +
                                '<li class="chain-no"></li>' +
                                '<li><p class="label pt-10"> TargetProcess Projects</p></li>' +
                            '</ul>' +
                            '<ul class="projects-mapping" style="width: auto">' +
                                '<li> <select id="tfs-projects" class="select workitems-tfs-projects"><option>- Select project -</option></select> </li>' +
                                '<li class="chain"></li>' +
                                '<li> <select id="tp-projects" class="select workitems-tp-projects"><option>- Select project -</select> </li>' +
                            '</ul>' +
                        '</div>' +
                        '<div class="separator"></div>' +
                        '<h3 class="label pt-20">Entity Mapping</h3>' +
                        '<h2 class="label pt-10">You can not modify entity mapping after the profile will be saved. To add a new one only.</h2>' +
                        '<div class="tfs-map-entities" style="width: auto"></div>' +
                      '</div>' +
                    '</div>' +
					'       <div class="controls-block">' +
					'        </div>' +
					'</div>',

	        _create: function (config) {
	            this.placeHolder = config.placeHolder;
	            this.model = config.model;
	            this.controller = config.controller;
	            this.tpProjects = config.tpProjects;
	            Bus.subscribe("TfsProfileEditor", {
	                onCheckConnectionForced: $.proxy(this._onCheckConnectionForced, this),
	                onCheckConnectionInitiated: $.proxy(this.onCheckConnectionInitiated, this),
	                onCheckConnectionSuccessful: $.proxy(this.onCheckConnectionSuccessful, this),
	                onCheckConnectionFailed: $.proxy(this.onCheckConnectionFailed, this),
	                onCheckConnectionError: $.proxy(this.onCheckConnectionError, this),
	                onSaveProfile: $.proxy(this._onSave, this),
	                onProfileSaveSucceed: $.proxy(this._onSaveSucceed, this)
	            }, true);
	        },

	        getTeamProjects: function () {
	            var projects;

	            $.ajax({
	                type: 'POST',
	                url: new Tp.WebServiceURL('/api/v1/Plugins/TFS/Commands/GetTeamProjects').url,
	                dataType: 'json',
	                processData: false,
	                contentType: 'application/json',
	                async: false,
	                data: JSON.stringify(this._getConnectionSettingsFromEditor())
	            })
                .done(function (data) { projects = data; });

	            var tfsProjectsListBox = this._find('#tfs-projects')[0];
	            tfsProjectsListBox.options.length = 0;

	            var emptyOption = document.createElement("Option");
	            emptyOption.text = "- Select project -";
	            tfsProjectsListBox.add(emptyOption);

	            for (var i = 0; i < projects.length; i++) {
	                var tfsProject = document.createElement("Option");
	                tfsProject.text = projects[i];

	                if (this.model.Settings.ProjectsMapping != null) {
	                    for (var j = 0; j < this.model.Settings.ProjectsMapping.length; j++)
	                        if (this.model.Settings.ProjectsMapping[j].Key == tfsProject.text) {
	                            tfsProject.selected = true;
	                            break;
	                        }
	                }

	                tfsProjectsListBox.add(tfsProject);
	            }
	        },

	        onCheckConnectionInitiated: function () {
	            this.clearErrors();
	            this.placeHolder.find('#failedConnection').hide().find('span').text('');
	            this.placeHolder.find('#warningConnection').hide().find('span').text('');
	            this.checkConnectionInProgress(true);
	        },

	        onCheckConnectionSuccessful: function () {
	            this.checkConnectionInProgress(false);
	            this.checkConnectionErrors([]);
	            this.getTeamProjects();
	        },

	        onCheckConnectionFailed: function (args) {
	            this.checkConnectionInProgress(false);

	            var errors = args;

	            var isWarning = this.checkConnectionWarnings(errors);

	            if (isWarning) {
	                var message = '';

	                for (var i = 0; i < errors.length; i++) {
	                    if (errors[i].Status == 2) {
	                        message = errors[i].Message;
	                        break;
	                    }
	                }

	                this.placeHolder.find('#warningConnection').show().find('span').text(message);
	            }
	            else {
	                var errorMessage = '';

	                for (i = 0; i < errors.length; i++) {
	                    if (errors[i].Status == 4) {
	                        errorMessage = errors[i].Message;
	                        break;
	                    }
	                }

	                if (errorMessage == '')
	                    errorMessage = "Unable to establish connection";

	                this.placeHolder.find('#failedConnection').show().find('span').text(errorMessage);
	            }
	        },

	        onCheckConnectionError: function (responseText) {
	            this.checkConnectionInProgress(false);

	            this.placeHolder.find('#failedConnection').show().find('span').text(responseText);
	        },

	        _toggleControlsStatus: function (parent, status) {
	            try {
	                parent.disabled = status;
	            }
	            catch (e) {
	            }

	            if (parent.childNodes && parent.childNodes.length > 0) {
	                for (var i = 0; i < parent.childNodes.length; i++)
	                    this._toggleControlsStatus(parent.childNodes[i], status);
	            }
	        },

	        _setControlsStatus: function (rendered) {
	            var that = this;
	            var sourceControlButton = rendered.find(".source-control-toggle")[0];
	            sourceControlButton.value =
                    this.model.Settings.SourceControlEnabled ? "Disable Source Control Integration" : "Enable Source Control Integration";

	            if (!this.model.Settings.SourceControlEnabled) {
	                var container = rendered.find(".source-control-settings")[0];
	                that._toggleControlsStatus(container, true);
	            }

	            if (sourceControlButton.onclick == undefined) {
	                sourceControlButton.onclick = function () {
	                    var container = rendered.find(".source-control-settings")[0];

	                    sourceControlButton.value =
                            (container.disabled ? "Disable Source Control Integration" : "Enable Source Control Integration");

	                    that._toggleControlsStatus(container, !container.disabled);
	                };
	            }

	            var workItemsButton = rendered.find(".work-items-toggle")[0];
	            workItemsButton.value =
                    this.model.Settings.WorkItemsEnabled ? "Disable Work Items Integration" : "Enable Work Items Integration";

	            if (!this.model.Settings.WorkItemsEnabled) {
	                var container = rendered.find(".workitems-settings")[0];
	                that._toggleControlsStatus(container, true);
	            }

	            if (workItemsButton.onclick == undefined) {
	                workItemsButton.onclick = function () {
	                    var container = rendered.find(".workitems-settings")[0];

	                    workItemsButton.value =
                            (container.disabled ? "Disable Work Items Integration" : "Enable Work Items Integration");

	                    that._toggleControlsStatus(container, !container.disabled);
	                };
	            }
	        },

	        render: function () {
	            this.placeHolder.html('');
	            var rendered = $.tmpl(this.editorTemplate, this.model, this);

	            function onProjectMappingChanged() {
	                if (rendered.find(".workitems-tfs-projects")[0].selectedIndex == 0 ||
                        rendered.find(".workitems-tp-projects")[0].selectedIndex == 0)
	                    return;

	                that.EntityMappingEditor.onProjectMappingChanged(that._getProfileFromEditor());
	            };

	            rendered.find(".workitems-tfs-projects")[0].onchange = onProjectMappingChanged;
	            rendered.find(".workitems-tp-projects")[0].onchange = onProjectMappingChanged;

	            this.checkConnectionBtn = rendered.find('#checkConnection');
	            this.checkConnectionBtn.click($.proxy(this._onCheckConnection, this));
	            this.preloader = rendered.find('span.preloader');

	            if (this.controller.isEditMode())
	                rendered.find('#tfs-projects').enabled(false);

	            rendered.appendTo(this.placeHolder);

	            rendered.find('#uriExamplesLink').click(function () {
	                $('#uriExamplesContent').animate({ opacity: 'toggle', height: 'toggle' }, 'slow');
	            });

	            this._disableNameIfNecessary();

	            this.UserMappingEditor = new UserMappingEditor({
	                placeHolder: this.placeHolder.find('.svn-map-users'),
	                model: this.model
	            });

	            this.UserMappingEditor.render();

	            this.placeHolder.find('.collapsable').live('click', this._toggle);

	            this._fillProjectsMapping();

	            var that = this;
	            this.EntityMappingEditor = new EntityMappingEditor({
	                placeHolder: this.placeHolder.find('.tfs-map-entities'),
	                model: this.model,
	                controller: this.controller
	            });

	            this.EntityMappingEditor.render();

	            this._setControlsStatus(rendered);

	            new profileControlsBlock({ placeholder: rendered }).render();
	        },

	        _disableNameIfNecessary: function () {
	            this.placeHolder.find('#name').enabled(!this.controller.isEditMode());
	        },

	        _fillProjectsMapping: function () {
	            if (this.controller.isEditMode()) {
	                var tfsProjects = this._find('#tfs-projects')[0];
	                if (tfsProjects.options.length == 1)
	                    this.getTeamProjects();
	            }

	            var tpProjectsSelect = this._find('#tp-projects')[0];

	            for (var i = 0; i < this.tpProjects.length; i++) {
	                var tpProject = document.createElement("Option");
	                tpProject.text = this.tpProjects[i].Name;
	                tpProject.value = this.tpProjects[i].Id;

	                if (this.model.Settings.ProjectsMapping != null) {
	                    for (var j = 0; j < this.model.Settings.ProjectsMapping.length; j++)
	                        if (this.model.Settings.ProjectsMapping[j].Value.Id == tpProject.value) {
	                            tpProject.selected = true;
	                            break;
	                        }
	                }

	                tpProjectsSelect.add(tpProject);
	            }
	        },

	        _toggle: function () {
	            $(this).toggleClass('collapsed').toggleClass('expanded');
	            $(this).next().slideToggle('fast');
	        },

	        _onSave: function () {
	            this.clearErrors();
	            this._clientValidate();

	            if (!this._isProfileValidToSave()) {
	                Bus.publish('ProfileSaveFailed');
	                return;
	            }

	            this.controller.save(this, this._getProfileFromEditor());
	        },

	        _onSaveSucceed: function () {
	            this.placeHolder.find('#name').enabled(false);
	            this.placeHolder.find('#tfs-projects').enabled(false);
	        },

	        _onCheckConnectionForced: function () {
	            this._onCheckConnection({ preventDefault: function () { } });
	        },

	        _onCheckConnection: function (e) {
	            e.preventDefault();

	            this.checkConnectionBtn.success('clear');
	            Bus.publish("CheckConnectionCommand", [this._getProfileFromEditor()]);
	        },

	        _clientValidate: function () {
	            this.UserMappingEditor.clientValidate();

	            profileValidToSave = this._validateIntegrationStatus() && this.EntityMappingEditor.isValidToSave();
	        },

	        _validateIntegrationStatus: function () {
	            if (this._IsWorkItemsEnabled() || this._IsSourceControlEnabled())
	                return true;

	            this.placeHolder.find('#integrationsDisabled').show().find('span').text('At least one integration section must be enabled');
	            return false;
	        },

	        _IsWorkItemsEnabled: function () {
	            return this.placeHolder.find("#workitems-button")[0].value == "Disable Work Items Integration";
	        },

	        _IsSourceControlEnabled: function () {
	            return this.placeHolder.find("#source-control-button")[0].value == "Disable Source Control Integration";
	        },

	        _validateProjectsMapping: function () {
	            if (this.placeHolder.find("#tfs-projects")[0].selectedIndex > 0 &&
    	            this.placeHolder.find("#tp-projects")[0].selectedIndex > 0)
	                return true;

	            this.placeHolder.find('#projectsMappingError').show().find('span').text('Project mapping mest be set before profile saving');
	            return false;
	        },

	        _validateEntitiesMapping: function () {
	            this.EntityMappingEditor.clientValidate();

	            return this.EntityMappingEditor.isValidToSave();
	        },

	        _isProfileValidToSave: function () {
	            var valid = this._validateIntegrationStatus();

	            if (valid && this._IsSourceControlEnabled())
	                valid = this._getValidationErrorsCount() == 0;

	            if (valid && this._IsWorkItemsEnabled())
	                return (this._validateProjectsMapping() && this._validateEntitiesMapping());

	            return valid;
	        },

	        _getValidationErrorsCount: function () {
	            return this.UserMappingEditor.getValidationErrorsCount();
	        },

	        _getConnectionSettingsFromEditor: function () {
	            return {
	                Name: this._find('#name').val(),
	                Settings: {
	                    Uri: this._find('#uri').val(),
	                    Login: this._find('#login').val(),
	                    Password: this._find('#password').val(),
	                    StartRevision: this._find('#startRevision').val(),
	                    UserMapping: [],
	                    EntityMapping: [],
	                    ProjectsMapping: []
	                }
	            };
	        },

	        _getButtonValue: function (button) {
	            return button.value.indexOf('Enable') == -1;
	        },

	        _getProfileFromEditor: function () {
	            return {
	                Name: this._find('#name').val(),
	                Settings: {
	                    Uri: this._find('#uri').val(),
	                    Login: this._find('#login').val(),
	                    Password: this._find('#password').val(),
	                    StartRevision: this._find('#startRevision').val(),
	                    UserMapping: this.UserMappingEditor.getUserMappings(),
	                    EntityMapping: this.EntityMappingEditor.getEntityMappings(),
	                    ProjectsMapping: this._getProjectsMapping(),
	                    SourceControlEnabled: this._getButtonValue(this._find('#source-control-button')[0]),
	                    WorkItemsEnabled: this._getButtonValue(this._find('#workitems-button')[0]),
	                    StartWorkItem: this._find('#startWorkItem').val()
	                }
	            };
	        },

	        _getProjectsMapping: function () {
	            var map = { Key: null, Value: null };

	            var tfsProjects = this.placeHolder.find(".workitems-tfs-projects")[0];
	            var tfsProject = tfsProjects.options[tfsProjects.selectedIndex];
	            if (tfsProject.text != "- Select project -")
	                map.Key = tfsProject.text;

	            var tpProjects = this.placeHolder.find(".workitems-tp-projects")[0];
	            var tpProject = tpProjects.options[tpProjects.selectedIndex];
	            if (tpProject.text != "- Select project -")
	                map.Value = { Id: tpProject.value, Name: tpProject.text };

	            if (map.Key == null || map.Value == null)
	                return null;

	            return [map];
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
	            this.placeHolder.find('#integrationsDisabled').hide().find('span').text('');
	            this.placeHolder.find('#projectsMappingError').hide().find('span').text('');

	            this.EntityMappingEditor.clearErrors();
	        },

	        checkConnectionInProgress: function (value) {
	            if (value) {
	                this.preloader.show();
	            } else {
	                this.preloader.hide();
	            }
	        },

	        checkConnectionWarnings: function (errors) {
	            this.showErrors(errors);

	            for (var i = 0; i < errors.length; i++) {
	                var error = errors[i];

	                if (error.Status != 2)
	                    return false;
	            }

	            this.checkConnectionBtn.success();
	            return true;
	        },

	        checkConnectionErrors: function (errors) {
	            this.showErrors(errors);

	            if ($(errors).length == 0)
	                this.checkConnectionBtn.success();
	        }
	    };
	    return ProfileEditor;
	});
