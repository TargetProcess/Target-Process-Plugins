tau.mashups
.addDependency("tp/bus")
.addDependency("tp/plugins/profileControlsBlock")
.addDependency("tp/plugins/profileRepository")
.addDependency("libs/jquery/jquery")
.addMashup(function (bus, profileControlsBlock, profileRepository, jquery, config) {
	function taskCreatorProfileEditor(config) {
		this._create(config);
	}

	taskCreatorProfileEditor.prototype = {
		template: null,
		placeHolder: null,
		saveBtn: null,
		_projects: null,
		_returnUrl: null,
		showSampleLink: null,
		samplePopup: null,

		_create: function (config) {
			this.placeHolder = config.placeHolder;
			this.repository = profileRepository;
			this._returnUrl = new Tp.WebServiceURL("/Admin/Plugins.aspx").toString();

			this.template = '<div>' +
                    '<form method="POST">' +
                    '<h2 class="h2">Task Creator</h2>' +
                    '<p class="note">This plugin automatically creates a set of tasks for a user story.</p>' +
                    '<div class="task-creator-settings">' +
                    '	<div class="pad-box">' +
                    '		<p class="label">Profile Name&nbsp;<span class="error" name="NameErrorLabel"></span><br />' +
					'		<span class="small">Once this name is saved, you can not change it.</span></p>' +
                    '		<input id="profileNameTextBox" type="text" name="Name" class="input" style="width: 275px;" value="${Name}" />' +
                    '	</div>' +
                    '	<div class="separator"></div>' +
                    '	<div class="pad-box">' +
                    '		<h3 class="h3">Task Creator Settings</h3>' +
                    '		<p class="label pt-5">Select Project&nbsp;<span class="error" name="ProjectErrorLabel"></span><br /><span class="small">The plugin will work for user stories from this project.</span></p>' +
                    '		<select class="select" id="projectsDropDown" name="Project">' +
                    '           <option value="0">- Select project -</option>' +
                    '			{{each projects}}<option value="${Id}">${Name}</option>{{/each}}' +
                    '		</select>' +
                    '		<p class="label pt-10">Command Name&nbsp;<span class="error" name="CommandNameErrorLabel"></span><br />' +
                    '			<span class="small">' +
                    '				As you add/edit a user story, you should put a special command prefix before user story name. Let\'s use' +
					'				<strong>{CT}</strong> as a prefix, and <strong>User Guide</strong> as the user story name. So, if we put <strong>{CT}User Guide</strong> to user story name field,' +
					'				a set of tasks entered to the Task List field will be created and the {CT} prefix will be then removed from user' +
					'				story name.' +
                    '			</span>' +
                    '		</p>' +
                    '		<input id="commandNameTextBox" name="CommandName" value="{{if Settings.CommandName != null}}${unescape(Settings.CommandName)}{{/if}}" type="text" class="input" style="width: 275px;" value="{Type your command name}" />' +
                    '		<p class="label pt-10">Task List&nbsp;<span class="error" name="TasksListErrorLabel"></span><br /><span class="small">Enter tasks here. One task per line.</span>' +
                    '		<a id="linkSample" class="note" style="font-size: 11px;" href="javascript:void(0);">Example</a></p>' +
                    '       <div id="tasksSample" class="tasksSamplePopup" style="display: none;">' +
                    '<div class="context-popup-uxo-t" style="right: -25px;"></div>' +
                    '<div class="p-10"><img src="../javascript/tau/css/images/plugins/task-creator-example.png" width="242px" height="138px" /></div>' +
                    '</div>' +
                    '<textarea id="tasksListTextArea" name="TasksList" class="textarea" style="width: 100%;" rows="10">{{if Settings.TasksList != null}}${unescape(Settings.TasksList)}{{/if}}</textarea>' +
                    '	</div>' +
                    '</div>' +
					'<div class="controls-block">' +
                    '</div>' +
                    '</form>' +
                    '</div>';

			this.bus = bus;
			this.bus.subscribe('TaskCreatorProfileEditor', {
				onSaveProfile: $.proxy(this._saveProfile, this),
				onProfileSaveSucceed: $.proxy(this._onProfileSaved, this)
			});
		},

		render: function () {
			$.getJSON(new Tp.WebServiceURL('/api/v1/Projects.asmx/?include=[Id,Name]&take=1000').url, $.proxy(this._onProjectsReceived, this));
		},

		_onProjectsReceived: function (projects) {
			this._projects = projects.Items.sort(function (a, b) {
				return a.Name.toLocaleLowerCase().localeCompare(b.Name.toLocaleLowerCase());
			});

			this.repository.getCurrentProfile($.proxy(this._renderProfile, this));
		},

		_getEditingProfileName: function () {
			return this.repository.getCurrentProfileName();
		},

		_renderProfile: function (profile) {
			profile = profile || { Name: null, Settings: { CommndName: null, TasksList: null} };
			profile.projects = this._projects;
			this.placeHolder.html('');

			var rendered = $.tmpl(this.template, profile);
			rendered.appendTo(this.placeHolder);

			this.showSampleLink = this.placeHolder.find('#linkSample');
			this.samplePopup = this.placeHolder.find('#tasksSample');

			$('body').click($.proxy(this._toggleSamplePopup, this));

			this._setSelectedProject(profile);
			this._setFocus(profile.Name);

			new profileControlsBlock({ placeholder: this.placeHolder }).render();
		},

		_setFocus: function (name) {
			var nameInput = this.placeHolder.find('#profileNameTextBox');

			if (name != null) {
				this.placeHolder.find('#tasksListTextArea').focus();
				nameInput.attr('disabled', true);
			}
			else {
				nameInput.focus();
			}
		},

		_toggleSamplePopup: function (e) {
			if ($(e.target).get(0) == this.showSampleLink.get(0)) {
				this._setPopupPosition();
				this.samplePopup.toggle();
			}
			else {
				this._hideSamplePopup();
			}
		},

		_setPopupPosition: function () {
			var position = this.showSampleLink.position();
			this.samplePopup.css('left', position.left);
			this.samplePopup.css('top', position.top + 25);
		},

		_hideSamplePopup: function () {
			if (this.samplePopup.is(':visible')) {
				this.samplePopup.hide();
			}
		},

		_setSelectedProject: function (profile) {
			if (profile.Name) {
				$("#projectsDropDown").val(profile.Settings.Project);
			}
		},

		_saveProfile: function () {
			var project = this.placeHolder.find('#projectsDropDown').val();
			if (project == null)
				project = 0;

			var profile =
            {
            	Name: this.placeHolder.find('#profileNameTextBox').val(),
            	Settings:
                {
                	Project: project,
                	CommandName: escape(this.placeHolder.find('#commandNameTextBox').val()),
                	TasksList: escape(this.placeHolder.find('#tasksListTextArea').val())
                }
            };

			if (this._getEditingProfileName()) {
				this.repository.update(profile);
			}
			else {
				this.repository.create(profile);
			}
		},

		_onProfileSaved: function (profile) {
			this._setFocus(profile.Name);
		}
	};

	new taskCreatorProfileEditor({
		placeHolder: $('#' + config.placeholderId)
	}).render();
})
