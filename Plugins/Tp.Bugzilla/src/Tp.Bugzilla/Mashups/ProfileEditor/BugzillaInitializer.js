tau.mashups
    .addDependency("tp/plugins/profileRepository")
    .addDependency("Bugzilla/RestService")
    .addDependency("Bugzilla/BugzillaProfileController")
    .addDependency("libs/jquery/jquery")
    .addModule("Bugzilla/BugzillaInitializer", function (profileRepository, restService, profileController, $) {
        function bugzillaInitializer(config) {
            this._ctor(config);
        }

        bugzillaInitializer.prototype = {
            placeholder: null,
            isLoaded: false,

            _ctor: function (config) {
                this.placeholder = config.placeholder;
                this.profileRepository = profileRepository;
                this.restService = new restService();
                this.profileController = new profileController(config);
                this.requestsInProgress = 0;
                this.projects = [];
                this.roles = [];
            },

            initialize: function () {
                this.requestsInProgress = 2;
                this.restService.getContext($.proxy(this._onProjectsReceived, this));
            },

            _onProjectsReceived: function (context) {
                this.projects = context.projects.sort(function (a, b) {
                    return a.Name.toLocaleLowerCase().localeCompare(b.Name.toLocaleLowerCase());
                });

                this.restService.getRoles(context.processes, $.proxy(this._onRolesReceived, this));
            },

            _onRolesReceived: function (roles) {
                this.roles = roles.sort(function (a, b) {
                    return a.Name.toLocaleLowerCase().localeCompare(b.Name.toLocaleLowerCase());
                });

                this.profileRepository.getCurrentProfile($.proxy(this._onGetCurrentProfile, this));
            },

            _onGetCurrentProfile: function (profile) {
                profile = profile || this._getDefaultProfile();

                this._initializeProfile(profile);
                this._render(profile);
            },

            _getDefaultProfile: function () {
                return {
                    Name: '',
                    Settings: {
                        Login: '',
                        Password: '',
                        Url: '',
                        Project: 0,
                        SavedSearches: '',
                        StatesMapping: [],
                        SeveritiesMapping: [],
                        PrioritiesMapping: [],
                        RolesMapping: [],
                        UserMapping: []
                    }
                };
            },

            _initializeProfile: function (profile) {
                profile.Projects = this.projects;
                profile.Roles = this.roles;

                this._addRoleToMapping(profile, 'Assignee', ['Developer']);
                this._addRoleToMapping(profile, 'Reporter', ['QA Engineer', 'Tester', 'Verifier']);
            },

            _addRoleToMapping: function (profile, thirdPartyName, tpNames) {
                if ($.grep(profile.Settings.RolesMapping, function (element) {
                    var elementExistsInTp = $.grep(profile.Roles, function(roleElement) {
                        return roleElement.Id == element.Value.Id;
                    }).length > 0;
                    return element.Key == thirdPartyName && elementExistsInTp;
                }).length > 0) {
                    return;
                }

                for (var i = 0; i < tpNames.length; i++) {
                    var tpName = tpNames[i];
                    var roles = $.grep(this.roles, function (element) {
                        return element.Name == tpName;
                    });
                    if (roles.length > 0) {
                        var role = roles[0];
                        profile.Settings.RolesMapping.push({
                            Key: thirdPartyName,
                            Value: {
                                Id: role.Id,
                                Name: role.Name
                            }
                        });
                        return;
                    }
                }
                profile.Settings.RolesMapping.push({
                    Key: thirdPartyName,
                    Value: {}
                });
            },

            _render: function (profile) {
                this.profileController.render(profile);
            }
        };

        return bugzillaInitializer;
    });
