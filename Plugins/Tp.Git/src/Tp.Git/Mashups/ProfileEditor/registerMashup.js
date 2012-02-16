tau.mashups

	.addDependency("tp/plugins/vcs/tpUsersPopoverWidget")
	.addDependency("Git/ProfileEditor")
	.addDependency("tp/plugins/commandGateway")
	.addDependency("tp/plugins/vcs/SubversionProfileEditorDefaultController")
	.addDependency("tp/plugins/profileRepository")
	.addDependency("tp/plugins/vcs/ui.widgets")
	.addModule("Git/registerMashup",

	function (tpUsersPopoverWidget,
		SubversionProfileEditor,
		commandGateway,
		SubversionProfileEditorDefaultController,
		profileRepository,
		globalAnimation) {

		function SubversionRenderer(config) {
			this._ctor(config);
		}

		SubversionRenderer.prototype = {
			_ctor: function (config) {
				this.placeholder = config.placeholder;
			},

			renderEditor: function () {

				var _profileRepository = profileRepository;

				function profileLoaded(data) {
					// TODO: repository should return default profile?
					var currentDate = new Date();
					var defaultProfile = {
						Name: '',
						Settings: {
							Uri: '',
							Login: '',
							Password: '',
							StartRevision: currentDate.getMonth() + 1 + '/' + currentDate.getDate() + '/' + currentDate.getFullYear()
						}
					};

					data = data || defaultProfile;

					var controller = new SubversionProfileEditorDefaultController({
						profileRepository: _profileRepository,
						commandGateway: new commandGateway(),
						tpUsers: this.tpUsers
					});

					var editor = new SubversionProfileEditor({
						placeHolder: this.placeholder,
						model: data,
						controller: controller
					});

					editor.render();
				}

				_profileRepository.getCurrentProfile($.proxy(profileLoaded, this));
			},

			renderAll: function () {
				this.renderEditor();
				globalAnimation.prototype.turnedOn = true;
			}
		};
		return SubversionRenderer;
	});

tau.mashups
	.addDependency("Git/registerMashup")
	.addMashup(function(subversionRenderer, config) {
		var placeholder = $('#' + config.placeholderId);

		new subversionRenderer({ placeholder: placeholder }).renderAll();
	});