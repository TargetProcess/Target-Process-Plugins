tau.mashups
	.addDependency("tp/plugins/userRepository")
	.addDependency("tp/plugins/restService")
	.addDependency("tp/plugins/vcs/tpUsersPopoverWidget")
	.addDependency("Subversion/SubversionProfileEditor")
	.addDependency("tp/plugins/commandGateway")
	.addDependency("tp/plugins/vcs/SubversionProfileEditorDefaultController")
	.addDependency("tp/plugins/profileRepository")
	.addDependency("tp/plugins/vcs/ui.widgets")
	.addModule("Subversion/registerMashup",

	function (userRepository,
		restService,
		tpUsersPopoverWidget,
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
					var defaultProfile = {
						Name: '',
						Settings: {
							Uri: '',
							Login: '',
							Password: '',
							StartRevision: '0'
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

			renderUserAutocomplete: function () {
				var that = this;
				function bindToUserMapping(handler, handlerId) {
					var selector = 'input.tpuser';

					handler(that.placeholder.find(selector));

					$(selector).live('focus', function () {

						var $el = $(this);

						if ($el.data(handlerId)) {
							return;
						}

						$el.data(handlerId, true);
						handler($el);
					});

				}

				function usersLoaded(data) {
					bindToUserMapping(function (elements) {
						new tpUsersPopoverWidget({ elements: elements, source: data });
					}, 'tpUsersPopoverWidget');

					bindToUserMapping(function (elements) {
						elements.synchronizeUser({ source: data });
					}, 'synchronizeUserWidget');
				};
				usersLoaded(this.tpUsers);
			},

			renderAll: function () {
				new userRepository({ restService: new restService() }).getUsers($.proxy(function (usersLoaded) {
					this.tpUsers = usersLoaded;
					this.renderEditor();
					this.renderUserAutocomplete();

					globalAnimation.prototype.turnedOn = true;

				}, this));
			}
		};
		return SubversionRenderer;
	});

tau.mashups
	.addDependency("Subversion/registerMashup")
	.addMashup(function(subversionRenderer, config) {
		var placeholder = $('#' + config.placeholderId);

		new subversionRenderer({ placeholder: placeholder }).renderAll();
	});