tau.mashups
	.addDependency("tp/bus")
	.addDependency("tp/plugins/errorMessageContainer")
	.addDependency("tp/plugins/profileControlsBlock")
	.addDependency("TestRunImport/editorTemplate")
	.addDependency("TestRunImport/seleniumButtons")
	.addDependency("TestRunImport/seleniumUrlEditor")
	.addDependency("TestRunImport/testPlansPopoverWidget")
	.addDependency("TestRunImport/jquery/iphone-switch")
	.addDependency("TestRunImport/jquery/numeric")
	.addDependency("libs/jquery/jquery")
	.addDependency("libs/jquery/jquery.tmpl")
	.addDependency("TestRunImport/jquery.utils")
	.addModule("TestRunImport/testRunImportProfileEditor", function (bus, errorMessageContainer, profileControlsBlock, editorTemplate, seleniumButtons, seleniumUrlEditor, testPlansPopoverWidget) {
		function testRunImportProfileEditor(config) {
			this._create(config);
		}

		testRunImportProfileEditor.prototype = {
			template: null,
			placeHolder: null,
			seleniumButtons: null,
			seleniumUrlEditor: null,
			saveBtn: null,
			cancelBtn: null,
			preloader: null,
			returnUrl: null,
			errorMessageContainer: null,

			_create: function (config) {
				this.placeHolder = config.placeHolder;
				this.repository = config.profileRepository;
				this.commandGateway = config.commandGateway;
				this.profileNameSource = config.profileNameSource;
				this.template = '<div>' + editorTemplate + '</div>';
				this.returnUrl = 'javascript:window.history.back()';
				this.errorMessageContainer = new errorMessageContainer({ placeholder: this.placeHolder });
				this.bus = bus;

				this.bus.subscribe('TestRunImportProfileEditor', {
					onSaveProfile: $.proxy(this._saveProfile, this),
					onProfileSaveSucceed: $.proxy(this._onProfileSaved, this)
				});
			},

			render: function () {
				this.repository.getCurrentProfile($.proxy(this._onProfileReceived, this));
			},

			_onProfileReceived: function (profile) {
				var callback = function (data) {
					profile = profile || this._getDefaultProfile();
					this.projects = profile.projects = data.items ? data.items : [];
					profile.Settings.SynchronizationInterval = profile.Settings.SynchronizationInterval / 60;
					if (!this._isEditMode() || !this._isProjectAvailable(profile.Settings.Project)) {
						profile.projects.splice(0, 0, { id: 0, name: ' - Select Project - ' });
						profile.testplan = {};
					}
					else {
						return $.getJSON(this._getTestPlanUrl(profile.Settings.Project, profile.Settings.TestPlan), $.proxy(function (data) {
							profile.testplan = data.items && data.items.length ? data.items[0] : {};
							this._renderProfile(profile);
							var testPlansSelect = this._find('input.tptestplan');
							this.popoverWidget = new testPlansPopoverWidget({ elements: testPlansSelect, projectId: profile.Settings.Project });
							testPlansSelect.enabled(this._isEditMode());
						}, this));
					}
					this._renderProfile(profile);
				};

				$.getJSON(new Tp.WebServiceURL('/api/v2/Project?select={id,name}&where=TestPlans.Count%20>%200&orderBy=name%20asc&take=1000').url, $.proxy(callback, this));
			},

			_getDefaultProfile: function () {
				return {
					Name: '',
					Settings: {
						ResultsFilePath: '',
						SynchronizationInterval: 0,
						AuthTokenUserId: 0,
						RemoteResultsUrl: '',
						PassiveMode: false,
						RegExp: '',
						Project: 0,
						TestPlan: 0,
						FrameworkType: 0,
						PostResultsToRemoteUrl: false
					}
				};
			},

			_renderProfile: function (profile) {
				this.placeHolder.html('');
				this.projects = profile.projects;

				var rendered = $.tmpl(this.template, profile), name = rendered.find('#name');
				name.enabled(!this._isEditMode());
				rendered.find('#syncInterval').numeric({ negative: false });
				rendered.find('#switch').iphoneSwitch(
					profile.Settings.PassiveMode ? 'on' : 'off', $.proxy(this._onSwitch, this), $.proxy(this._onSwitch, this),
					{ switch_on_container_path: '../img/plugins/switch_on.png', switch_off_container_path: '../img/plugins/switch_off.png', switch_path: '../img/plugins/switch.png' }
				);

				var projectsSelect = rendered.find('#projectsDropDown'), testPlansSelect = rendered.find('#testPlansDropDown'), frameworkSelect = rendered.find('#frameworkDropDown');
				if (this._isEditMode() && this._isProjectAvailable(profile.Settings.Project)) {
					projectsSelect.val(profile.Settings.Project);
					testPlansSelect.val(profile.Settings.TestPlan);
					frameworkSelect.val(profile.Settings.FrameworkType);
					if (profile.Settings.FrameworkType == 3) {
						this._processSeleniumEditors(rendered,
							profile.Settings.PostResultsToRemoteUrl ? 'url' : 'path',
							profile.Settings.PostResultsToRemoteUrl ? profile.Settings.RemoteResultsUrl : '',
							profile.Settings.PostResultsToRemoteUrl ? profile.Settings.AuthTokenUserId : -1);
					}
					frameworkSelect.enabled(false);
				} else {
					testPlansSelect.enabled(false);
				}

				rendered.find('#linkSample').click(function () {
					rendered.find('#mappingDescription').animate({ opacity: 'toggle', height: 'toggle' }, 'slow');
				});

				rendered.find('#checkMapping').click($.proxy(this._checkMapping, this));
				projectsSelect.change($.proxy(this._projectChange, this));
				testPlansSelect.change(function (e) { $(e.target).find('option[value="0"]').remove(); });
				frameworkSelect.change($.proxy(this._frameworkChange, this));

				this.preloader = rendered.find('span.preloader');

				rendered.appendTo(this.placeHolder);
				name.focus();

				new profileControlsBlock({ placeholder: rendered }).render();
			},

			_find: function (selector) {
				return this.placeHolder.find(selector);
			},

			_isEditMode: function () {
				return this.profileNameSource.getProfileName() != null;
			},

			_isObjectWithIdInArray: function (arr, id) {
				var r = false;
				$.each(arr, function (i, v) { if (v.id === id) r = true; });
				return r;
			},

			_onSwitch: function (el) {
			},

			_editorChange: function (e, el) {
				if (el.isUrlActive()) {
					this._createUrlEditorIfNeeded(this.placeHolder, '', -1);
					this._find('div#pathHolder').hide();
					this.seleniumUrlEditor.rendered.animate({ opacity: 'show' }, 'slow');
				} else {
					this.seleniumUrlEditor.rendered.hide();
					this._find('div#pathHolder').animate({ opacity: 'show' }, 'slow');
				}
			},

			_projectChange: function (e) {
				var select = $(e.target), testPlansSelect = this._find('input.tptestplan');
				if (this.popoverWidget) {
					this.popoverWidget.destroy();
					this.popoverWidget = null;
				}
				var projectId = parseInt(select.val());
				testPlansSelect.removeAttr('testplanid').val('').enabled(projectId);
				if (projectId) {
					this.popoverWidget = new testPlansPopoverWidget({ elements: testPlansSelect, projectId: projectId });
				};
			},

			_createUrlEditorIfNeeded: function (container, url, authUserId) {
				if (!this.seleniumUrlEditor) {
					this.seleniumUrlEditor = new seleniumUrlEditor({
						pluginName: this.profileNameSource.getPluginName(),
						profileNameInput: container.find('#name'),
						profileGetter: $.proxy(this._getProfileFromEditor, this),
						commandGateway: this.commandGateway
					});
					this.seleniumUrlEditor.render({ RemoteResultsUrl: url, AuthUserId: authUserId });
					this.seleniumUrlEditor.rendered.insertAfter(container.find('div#pathHolder'));
					this.seleniumUrlEditor.rendered.bind('showerrors', $.proxy(function (e, d) {
						this.errorMessageContainer.clearErrors();
						this.errorMessageContainer.addRange(d);
						this.errorMessageContainer.render();
					}, this));
				}
			},

			_createSeleniumButtonsIfNeeded: function (active, afterEl) {
				if (!this.seleniumButtons) {
					this.seleniumButtons = new seleniumButtons({ active: active });
					this.seleniumButtons.render();
					this.seleniumButtons.rendered.insertAfter(afterEl);
					this.seleniumButtons.rendered.bind('editorchange', $.proxy(this._editorChange, this));
				}
			},

			_processSeleniumEditors: function (container, active, url, authUserId) {
				this._createSeleniumButtonsIfNeeded(active, container.find('h3:contains(Result File)'));
				if (this.seleniumButtons.isUrlActive()) {
					this._createUrlEditorIfNeeded(container, url, authUserId);
					container.find('div#pathHolder').hide();
					this.seleniumUrlEditor.rendered.show();
				}
				container.find('h3:contains(Result File)').hide();
				this.seleniumButtons.rendered.animate({ opacity: 'show' }, 'slow');
			},

			_frameworkChange: function (e) {
				var select = $(e.target);
				select.find('option[value="0"]').remove();
				if (select.val() == 3) {
					this._processSeleniumEditors(this.placeHolder, 'path', '', -1);
				} else {
					if (this.seleniumUrlEditor) {
						this.seleniumUrlEditor.rendered.hide();
					}
					if (this.seleniumButtons) {
						this.seleniumButtons.rendered.hide();
						if (this.seleniumButtons.isUrlActive()) {
							this.seleniumUrlEditor.rendered.hide();
						}
					}
					this._find('h3:contains(Result File)').show();
					this._find('div#pathHolder').show();
				}
			},

			_isProjectAvailable: function (projectId) {
				return this._isObjectWithIdInArray(this.projects, projectId);
			},

			_getTestPlanUrl: function (projectId, testPlanId) {
				return new Tp.WebServiceURL('/api/v2/TestPlan?select={id,name}&where=Project.Id%20=%20{ProjectId}%20and%20Id%20=%20{TestPlanId}'.replace(/{ProjectId}/g, projectId).replace(/{TestPlanId}/g, testPlanId)).url;
			},

			_saveProfile: function () {
				var profile = this._getProfileFromEditor();

				if (this._isEditMode()) {
					this.repository.update(profile);
				}
				else {
					this.repository.create(profile);
				}
			},

			_onProfileSaved: function (profile) {
				this.placeHolder.find('#name').enabled(false);
				if (this._isProjectAvailable(profile.Settings.Project)) {
					this.placeHolder.find('#frameworkDropDown').enabled(false);
				} else {
					this.placeHolder.find('#testPlansDropDown').enabled(false);
				}
			},

			_checkMapping: function (e) {
				e.preventDefault();

				var btn = $(e.target);
				if (!btn.enabled()) {
					return;
				}
				btn.enabled(false);
				this._find('#mappingHolder').hide();
				this.preloader.show();

				var profile = this._getProfileFromEditor();
				this.commandGateway.execute('ValidateProfileForMapping', profile, $.proxy(function (data) {
					this._onValidateProfileForMappingSuccess(data, profile, btn);
				}, this), $.proxy(this._onValidateProfileForMappingError, this), $.proxy(this._onValidateProfileForMappingError, this));
			},

			_onValidateProfileForMappingError: function (responseText) {
				this.placeHolder.find('#failedMappingCheck').show().find('span').text(responseText);
				this.preloader.hide();
			},

			_onValidateProfileForMappingSuccess: function (data, profile, btn) {
				if (data.length > 0) {
					this.errorMessageContainer.clearErrors();
					this.errorMessageContainer.addRange(data);
					this.errorMessageContainer.render();
					this.preloader.hide();
					btn.enabled(true);
				} else {
					var testCasesUrl = new Tp.WebServiceURL('/api/v1/TestPlans.asmx/{TestPlanId}/TestCases?include=[id,name]&take=1000'.replace(/{TestPlanId}/g, profile.Settings.TestPlan)).url;
					$.getJSON(testCasesUrl, $.proxy(function (data) {
						this.placeHolder.find('#failedMappingCheck').hide().find('span').text('');
						var onError = $.proxy(function (d) {
							if (d.Errors.length > 0) {
								this.errorMessageContainer.clearErrors();
								this.errorMessageContainer.addRange(d.Errors);
								this.errorMessageContainer.render();
							}
							else {
								this.errorMessageContainer.clearErrors();
								if (d.NamesMappers.length > 0) {
									var mappingsBlock = this._find('#mappingHolder');
									var table = mappingsBlock.find('#checkMappingTable');
									table.find("tr:gt(0)").remove();
									mappingsBlock.find('div.check-mapping-block').next('p').html('');
									$.each(d.NamesMappers, function (i, o) {
										table.append(((!o.Case || o.Case.length == 0 || !o.Test || o.Test.length == 0) ? $('<tr></tr>').css('color', '#999') : $('<tr></tr>'))
												.append($('<td></td>').text(i + 1))
												.append($('<td></td>').text(o.Case))
												.append($('<td></td>').text(o.Test))
												.append($('<td></td>')));
									});
									mappingsBlock.find('div.check-mapping-block').next('p')
										.html('<span class="mapped">{Mapped}</span>&nbsp;mapped,&nbsp;<span class="not-mapped">{NotMappedTestCases} test cases</span>&nbsp;<span class="not-mapped">{NotMappedUnitTests}&nbsp;unit tests</span>&nbsp;unmatched&nbsp;<span style="display:{ShowWarning}" class="warning-message">{OverMappedUnitTests}&nbsp;test case(s)</span>&nbsp;<span style="display:{ShowWarning}">mapped more than once</span>'
												.replace(/{Mapped}/g, d.Mapped)
												.replace(/{NotMappedTestCases}/g, d.NotMappedTestCases)
												.replace(/{NotMappedUnitTests}/g, d.NotMappedUnitTests)
												.replace(/{OverMappedUnitTests}/g, d.OverMappedUnitTests)
												.replace(/{ShowWarning}/g, d.OverMappedUnitTests == 0 ? 'none' : ''));
									mappingsBlock.animate({ height: 'show', opacity: 'show' }, 'slow');
								}
							}
							this.preloader.hide();
							btn.enabled(true);
						}, this);
						this.commandGateway.execute('CheckMapping', { Profile: profile, TestCases: data.Items || [] }, onError, onError);
					}, this));
				}
			},

			_getSynchronizationInterval: function () {
				var res = parseInt(this._find('#syncInterval').val());
				return isNaN(res) ? 0 : res;
			},

			_getProfileFromEditor: function () {
				var state = 'off';
				var postToRemote = this._find('#frameworkDropDown').val() == 3 && this.seleniumButtons != null && this.seleniumButtons.isUrlActive();
				this._find('#switch').iphoneSwitch(function () { state = this.getState(); });
				return {
					Name: this._find('#name').val(),
					Settings: {
						ResultsFilePath: postToRemote ? '' : this._find('#resultsFilePath').val(),
						SynchronizationInterval: postToRemote ? 24 : this._getSynchronizationInterval(),
						AuthTokenUserId: postToRemote ? this._find('input.tpuser').attr('userId') : 0,
						RemoteResultsUrl: postToRemote ? this.seleniumUrlEditor.getSeleniumUrl() : '',
						PassiveMode: state === 'on',
						RegExp: this._find('#regExp').val(),
						Project: this._find('#projectsDropDown').val() || 0,
						TestPlan: this._find('input.tptestplan').attr('testplanid') || 0,
						FrameworkType: this._find('#frameworkDropDown').val() || 0,
						PostResultsToRemoteUrl: postToRemote
					}
				};
			}
		};
		return testRunImportProfileEditor;
	});
