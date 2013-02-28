tau.mashups

	.addDependency("tp/plugins/vcs/tpUsersPopoverWidget")
	.addDependency("Tfs/ProfileEditor")
	.addDependency("tp/plugins/commandGateway")
	.addDependency("tp/plugins/vcs/SubversionProfileEditorDefaultController")
	.addDependency("tp/plugins/profileRepository")
	.addDependency("tp/plugins/vcs/ui.widgets")
	.addModule("Tfs/registerMashup",

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

	        _getTPProjects: function () {
                $.getJSON(new Tp.WebServiceURL('/api/v1/Projects/?include=[Id,Name]').url, $.proxy(this._onProjectsReceived, this));
	        },

	        _onProjectsReceived: function (projects) {
	            var _projects = projects.Items.sort(function (a, b) {
	                return a.Name.toLocaleLowerCase().localeCompare(b.Name.toLocaleLowerCase());
	            });

	            var _profileRepository = profileRepository;

	            function profileLoaded(data) {
	                // TODO: repository should return default profile?
	                
	                var defaultProfile = {
	                    Name: '',
	                    Settings: {
	                        Uri: '',
	                        Login: '',
	                        Password: '',
	                        StartRevision: 1,
                            StartWorkItem: 1,
                            SourceControlEnabled: true,
                            WorkItemsEnabled: false
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
	                    controller: controller,
	                    tpProjects: _projects
	                });

	                editor.render();
	            }

	            _profileRepository.getCurrentProfile($.proxy(profileLoaded, this));
	        },

	        renderEditor: function () {
	            this._getTPProjects();
	        },

	        renderAll: function () {
	            this.renderEditor();
	            globalAnimation.prototype.turnedOn = true;
	        }
	    };
	    return SubversionRenderer;
	});

tau.mashups
	.addDependency("Tfs/registerMashup")
	.addMashup(function(subversionRenderer, config) {
		var placeholder = $('#' + config.placeholderId);

		new subversionRenderer({ placeholder: placeholder }).renderAll();
	});