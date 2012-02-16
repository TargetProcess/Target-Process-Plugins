require(["Bugzilla/RestService"],
    function (restService) {
        module('test rest service', {
            setup: function () {
                this.restService = new restService();
                this.restService._cache = {};
                this.restService._cache.Users = [
                    { User: { Id: 1} },
                    { User: { Id: 3} }
                ];

                this.tpStates ={ Items: [
                            { "Id": 1, "Name": "Open", "EntityType": { "Id": 4, "Name": "UserStory"} },
                            { "Id": 2, "Name": "Done", "EntityType": { "Id": 4, "Name": "UserStory"} },
                            { "Id": 5, "Name": "Open", "EntityType": { "Id": 8, "Name": "Bug"} },
                            { "Id": 6, "Name": "Fixed", "EntityType": { "Id": 8, "Name": "Bug"} },
                            { "Id": 7, "Name": "Invalid", "EntityType": { "Id": 8, "Name": "Bug"} },
                            { "Id": 10, "Name": "Done", "EntityType": { "Id": 14, "Name": "TestPlanRun"} },
                            { "Id": 15, "Name": "Open", "EntityType": { "Id": 16, "Name": "Impediment"} },
                            { "Id": 16, "Name": "Resolved", "EntityType": { "Id": 16, "Name": "Impediment"} },
                            { "Id": 22, "Name": "Done", "EntityType": { "Id": 9, "Name": "Feature"} },
                            { "Id": 65, "Name": "IN DEVELOPMENT", "EntityType": { "Id": 8, "Name": "Bug"} }
                        ]
                };

                this.context = {
                    "SelectedProjects": { Items: [
                        {
                            "Id": 1,
                            "Name": "Project without Bug Tracking",
                            "Process": {
                                "Id": 2,
                                "Name": "Kanban"
                            },
                            "Program": null
                        },
                        {
                            "Id": 2,
                            "Name": "Project 1",
                            "Process": {
                                "Id": 1,
                                "Name": "All Practices"
                            },
                            "Program": null
                        },
                        {
                            "Id": 3,
                            "Name": "Project 2",
                            "Process": {
                                "Id": 1,
                                "Name": "All Practices"
                            },
                            "Program": null
                        }
                    ]
                    },
                    "Processes": { Items: [
                        {
                            "Id": 2,
                            "Name": "Kanban",
                            "Terms": [],
                            "Practices": { Items: [
                                {
                                    "Name": "Planning",
                                    "Description": "General Project planning. Supports iterative development, user stories and tasks hierarchy",
                                    "EffortPoints": "Hour",
                                    "IsStoryEffortEqualsSumTasksEffort": false
                                }
                            ]
                            },
                            "CustomFields": []
                        },
                        {
                            "Id": 1,
                            "Name": "All Practices",
                            "Terms": [],
                            "Practices": { Items: [
                                {
                                    "Name": "Iterations",
                                    "Description": "Supports Iterations-oriented development practice."
                                },
                                {
                                    "Name": "Bug Tracking",
                                    "Description": "Simple Bug Tracking. Bugs list, bugs workflow and assignments"
                                }
                            ]
                            },
                            "CustomFields": []
                        }
                    ]
                    }
                }
            },

            teardown: function () {
            }
        });

        test('should filter users and take only users from specified project team', function () {
            var that = this;
            this.restService._onAllUsersRecieved(function (data) {
                that.restService.result = data;
            })({ Items: [
                { Id: 1 },
                { Id: 2 },
                { Id: 3 },
                { Id: 4 }
            ]
            });

            ok(this.restService.result.Items.length === 2, 'should filter users by project team');
            ok(this.restService.result.Items[0].Id === 1, 'filtered users should contain user with id 1');
            ok(this.restService.result.Items[1].Id === 3, 'filtered users should contain user with id 2');
        });

        test('should get states and take only ones for specifies project\'s process', function () {
            this.restService._onEntityStatesRecieved()(this.tpStates);

            var result = this.restService._cache.states;

            ok(result.length === 4, 'Should keep only bug states');
            ok(result[0].Id === 5 && result[0].Name === 'Open', 'State should have id and name');
            ok(result[1].Id === 6 && result[1].Name === 'Fixed', 'State should have id and name');
            ok(result[2].Id === 7 && result[2].Name === 'Invalid', 'State should have id and name');
            ok(result[3].Id === 65 && result[3].Name === 'IN DEVELOPMENT', 'State should have id and name');
        });

        test('should filter projects with bug tracking practice on', function () {
            var filteredProjects;
            var success = function (context) {
                filteredProjects = context.projects;
            };

            this.restService._onGetContext(success)(this.context);

            ok(filteredProjects.length === 2, 'Projects should be filtered by bug tracking practice');
            ok(filteredProjects[0].Name === 'Project 1', 'Project 1 should be present in filtered result');
            ok(filteredProjects[1].Name === 'Project 2', 'Project 2 should be present in filtered result');
        });

        test('should filter out duplicate roles', function () {
            var success = function (data) {
                ok(data.length == 2, 'duplicate roles filtered out');
            }
            this.restService._onRolesReceived(success)(
                {
                    Items: [
                        {
                            Role: {
                                Id: 1,
                                Name: 'Developer'
                            }
                        },
                        {
                            Role: {
                                Id: 2,
                                Name: 'QA Engineer'
                            }
                        },
                        {
                            Role: {
                                Id: 1,
                                Name: 'Developer'
                            }
                        }
                    ]
                });
        })
    });