require(["Bugzilla/BugzillaProfileView"],
    function (profileView) {
        module('test bugzilla profile view', {
            setup: function () {
                this.placeholder = $('<div />');

                this.view = new profileView({
                    placeholder: this.placeholder,
                    isEditMode: true
                });

                var that = this;

                this.errorMessageContainer = new ErrorMessageContainerMock();
                this.view.errorMessageContainer = this.errorMessageContainer;

                this.profile = {
                    "Name": "bz",
                    "Settings": {
                        "Login": "bugzilla@targetprocess.com",
                        "Password": "bugzillaadmin",
                        "PrioritiesMapping": null,
                        "Project": 1,
                        "SavedSearches": "alla plugin test",
                        "Url": "http:\/\/new-bugzilla\/bugzilla363"
                    },
                    "Projects": [
                        {
                            "Id": 280,
                            "Name": "#2"
                        },
                        {
                            "Id": 1,
                            "Name": "Private Universe #1"
                        }]
                };

                this.checkInput = function (id, val) {
                    var input = that.placeholder.find('#' + id);
                    ok(input.length == 1, 'input for ' + id + ' rendered');
                    ok(input.val() == val, 'input for ' + id + ' has value ' + val);
                };
            },

            teardown: function () {
            }
        });

        test('should render view for new profile', function () {
            this.view.isEditMode = false;
            this.view.render({
                Name: '',
                Settings: {
                    Login: '',
                    Password: '',
                    Url: '',
                    Project: 0,
                    SavedSearches: ''
                }
            });

            this.checkInput('name', '');
            ok(!this.placeholder.find('#name').attr('disabled'), 'name input enabled');
            this.checkInput('url', '');
            this.checkInput('login', '');
            this.checkInput('password', '');
            this.checkInput('project', '0');
            this.checkInput('savedSearches', '');
        });

        test('should render view for existing profile', function () {
            this.view.render(this.profile);

            this.checkInput('name', 'bz');
            ok(this.placeholder.find('#name').attr('disabled'), 'name input disabled');
            this.checkInput('url', 'http:\/\/new-bugzilla\/bugzilla363');
            this.checkInput('login', 'bugzilla@targetprocess.com');
            this.checkInput('password', 'bugzillaadmin');
            this.checkInput('project', '1');
            this.checkInput('savedSearches', 'alla plugin test');
        });

        test('should get profile', function () {
            this.view.render(this.profile);

            var profile = this.view.getProfile();
            ok(profile.Name == this.profile.Name, 'profile name retrieved');
            ok(profile.Settings.Login == this.profile.Settings.Login, 'profile login retrieved');
            ok(profile.Settings.Password == this.profile.Settings.Password, 'profile password retrieved');
            ok(profile.Settings.Url == this.profile.Settings.Url, 'profile url retrieved');
            ok(profile.Settings.Project == this.profile.Settings.Project, 'profile project retrieved');
            ok(profile.Settings.SavedSearches == this.profile.Settings.SavedSearches, 'profile queries retrieved');
        });

        test('should initialize', function () {
            var connectionCheckerInitialized = false;
            this.view.connectionChecker.initialize = function () {
                connectionCheckerInitialized = true;
            };

            this.view.render(this.profile);

            ok(connectionCheckerInitialized, 'connection checker initialized');
        });
    });