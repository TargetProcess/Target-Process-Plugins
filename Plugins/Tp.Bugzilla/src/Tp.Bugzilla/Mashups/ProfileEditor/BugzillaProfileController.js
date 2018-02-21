tau.mashups
    .addDependency("tp/bus")
    .addDependency("Bugzilla/UserMappingEditor")
    .addDependency("tp/plugins/profileNameSource")
    .addDependency("tp/plugins/profileRepository")
    .addDependency("Bugzilla/ConnectionChecker")
    .addDependency("Bugzilla/MappingController")
    .addDependency("Bugzilla/BugzillaProfileView")
    .addDependency("libs/jquery/jquery")
    .addModule("Bugzilla/BugzillaProfileController", function (bus, userMappingEditor, profileNameSource, profileRepository, connectionChecker, mappingController, view, $) {
        function bugzillaProfileController(config) {
            this._ctor(config);
        }

        bugzillaProfileController.prototype = {
            placeholder: null,
            isLoaded: false,

            _ctor: function (config) {
                this.placeholder = config.placeholder;
                this.profileRepository = profileRepository;
                this.bus = bus;
                var profileRetriever = $.proxy(this.getProfile, this);
                this.view = new view({
                    placeholder: this.placeholder,
                    mashupPath: config.mashupPath,
                    onProjectChanged: $.proxy(this._onProjectChanged, this),
                    profileRetriever: profileRetriever
                });

                var connChecker = new connectionChecker({
                    placeholder: this.placeholder,
                    loaderSelector: 'span#automapPreloader',
                    profileRetriever: profileRetriever,
                    quiet: true
                });

                this.mappingController = new mappingController({
                    placeholder: this.placeholder,
                    connectionChecker: connChecker,
                    profileRetriever: profileRetriever
                });

                this.bus.subscribe('BugzillaProfileController', {
                    onSaveProfile: $.proxy(this._save, this)
                }, true);
            },

            render: function (profile) {
                this.view.render(profile, this._isEditMode());

                this.mappingController.render(profile);
                this._renderUserMappingControl(profile);

                this.placeholder.find('h3.collapsable').click(this._toggle);
            },

            getProfile: function () {
                var profile = this.view.getProfile();

                profile.Settings.UserMapping = this.userMappingEditor.getUserMappings();
                profile.Settings.StatesMapping = this.mappingController.getStatesMapping();
                profile.Settings.SeveritiesMapping = this.mappingController.getSeveritiesMapping();
                profile.Settings.PrioritiesMapping = this.mappingController.getPrioritiesMapping();
                profile.Settings.CustomMapping = this.mappingController.getCustomMapping();
                profile.Settings.RolesMapping = this.mappingController.getRolesMapping();

                return profile;
            },

            _onProjectChanged: function (profile) {
                this._renderUserMappingControl(profile);
            },

            _renderUserMappingControl: function (profile) {
                this.userMappingEditor = new userMappingEditor({
                    placeholder: this.placeholder.find('.bugzilla-map-users'),
                    model: profile
                });
                this.userMappingEditor.render();

                var projectId = profile.Settings.Project;
                this.userMappingEditor.renderUserAutocomplete(projectId);
            },

            _toggle: function () {
                $(this).toggleClass('collapsed').toggleClass('expanded');
                $(this).next().slideToggle('fast');
            },

            _isEditMode: function () {
                return this.profileRepository.getCurrentProfileName() != null;
            },

            _save: function () {
                var profile = this.getProfile();
                var save = $.proxy(this._isEditMode() ? this.profileRepository.update : this.profileRepository.create, this.profileRepository);

                save(profile);
            }
        };

        return bugzillaProfileController;
    });
