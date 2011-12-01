require(["Bugzilla/BugzillaProfileController"],
    function (profileController) {
        module('test bugzilla profile controller', {
            setup: function () {
                this.placeholder = $('<div />');
                this.controller = new profileController({
                    placeholder: this.placeholder
                });

                this.projects = {
                    "Items": [
                            {
                                "Id": 280,
                                "Name": "#2"
                            },
                            {
                                "Id": 1,
                                "Name": "Private Universe #1"
                            }
                        ]
                };

                this.profile = { "Name": "bz", "Settings": { "Login": "bugzilla@targetprocess.com", "Password": "bugzillaadmin", "PrioritiesMapping": null, "Project": 1, "Queries": "alla plugin test", "SeveritiesMapping": null, "StatesMapping": [{ "Key": "UNCONFIRMED", "Value": { "Id": 7, "Name": "Invalid"} }, { "Key": "NEW", "Value": { "Id": 5, "Name": "Open"} }, { "Key": "In Development", "Value": { "Id": 0, "Name": null} }, { "Key": "RESOLVED", "Value": { "Id": 6, "Name": "Fixed"} }, { "Key": "In Testing", "Value": { "Id": 7, "Name": "Invalid"} }, { "Key": "CLOSED", "Value": { "Id": 8, "Name": "Closed"}}], "Url": "http:\/\/new-bugzilla\/bugzilla363", "UserMapping": [{ "Key": "bugzilla@targetprocess.com", "Value": { "Id": 5, "Name": "Andrew Gray"} }, { "Key": "testuser@mail.com", "Value": { "Id": 5, "Name": "Andrew Gray"}}]} };
            },

            teardown: function () {
            }
        });

        test('should initialize and render view and mappings', function () {
            var that = this;

            var retrievedProfile = null;
            this.controller.view.render = function (profile) {
                retrievedProfile = profile;
            };

            var profilePassedToMappingController = null;
            this.controller.mappingController.render = function (profile) {
                profilePassedToMappingController = profile;
            };

            this.controller.render(this.profile);

            ok(retrievedProfile.Name == that.profile.Name, 'profile name retrieved');
            ok(retrievedProfile.Settings.Login == that.profile.Settings.Login, 'profile login retrieved');
            ok(retrievedProfile.Settings.Password == that.profile.Settings.Password, 'profile password retrieved');
            ok(retrievedProfile.Settings.Url == that.profile.Settings.Url, 'profile url retrieved');
            ok(retrievedProfile.Settings.Project == that.profile.Settings.Project, 'profile project retrieved');
            ok(retrievedProfile.Settings.Queries == that.profile.Settings.Queries, 'profile queries retrieved');
            ok(profilePassedToMappingController.Settings.StatesMapping == that.profile.Settings.StatesMapping, 'profile queries passed to mapping controller');
        });
    });