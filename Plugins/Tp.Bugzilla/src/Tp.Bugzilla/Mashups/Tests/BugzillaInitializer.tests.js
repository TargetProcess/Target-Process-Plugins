require(["Bugzilla/BugzillaInitializer"],
    function(initializer) {
        module('test Bugzilla initializer', {
            setup:function() {
                this.placeholder = $('<div></div>');

                this.context = {
                    projects:[
                        {
                            "Id":1635,
                            "Name":"123",
                            "Process":{
                                "Id":1,
                                "Name":"All Practices"
                            },
                            "Program":null
                        }
                    ],
                    processes:[
                        {
                            "Id":2,
                            "Name":"Kanban",
                            "Terms":[],
                            "Practices":[
                                {
                                    "Name":"Planning",
                                    "Description":"General Project planning. Supports iterative development, user stories and tasks hierarchy",
                                    "EffortPoints":"Hour",
                                    "IsStoryEffortEqualsSumTasksEffort":false
                                }
                            ],
                            "CustomFields":[]
                        },
                        {
                            "Id":1,
                            "Name":"All Practices",
                            "Terms":[],
                            "Practices":[
                                {
                                    "Name":"Iterations",
                                    "Description":"Supports Iterations-oriented development practice."
                                },
                                {
                                    "Name":"Bug Tracking",
                                    "Description":"Simple Bug Tracking. Bugs list, bugs workflow and assignments"
                                }
                            ],
                            "CustomFields":[]
                        }
                    ]
                };

                this.roles = [
                    {
                        "Id":1,
                        "Name":"Developer",
                        "IsPair":true
                    },
                    {
                        "Id":2,
                        "Name":"Project Manager",
                        "IsPair":false
                    },
                    {
                        "Id":3,
                        "Name":"Top Manager",
                        "IsPair":false
                    },
                    {
                        "Id":4,
                        "Name":"QA Engineer",
                        "IsPair":false
                    },
                    {
                        "Id":6,
                        "Name":"Support Person",
                        "IsPair":false
                    }
                ];

                this.profile = {
                    "Name":"bz",
                    "Settings":{
                        "Login":"bugzilla@targetprocess.com",
                        "Password":"bugzillaadmin",
                        "Project":1,
                        "SavedSearches":"alla plugin test",
                        "Url":"http:\/\/new-bugzilla\/bugzilla363",
                        RolesMapping:[
                            { "Key":"Assignee", "Value":{ "Id":1, "Name":"Developer"} },
                            { "Key":"Reporter", "Value":{ "Id":4, "Name":"QA Engineer"} }
                        ]
                    }
                };

                var that = this;
                this.initializer = new initializer({ placeholder:this.placeholder });
                this.initializer.restService.getContext = function(success) {
                    success(that.context);
                };
                this.initializer.restService.getRoles = function(processes, success) {
                    success(that.roles);
                };
            },

            teardown:function() {
            }
        });

        test('should initialize roles and projects', function() {
            this.initializer.profileController.render = function(profile) {
                ok(profile.Projects.length > 0, 'projects initialized');
                ok(profile.Roles.length > 0, 'roles initialized');
            };
            this.initializer.initialize();
        });

        test('should initialize roles mapping during profile creation', function() {
            this.initializer.profileController.render = function(profile) {
                ok(profile.Settings.RolesMapping.length == 2, 'there are 2 items in the roles mapping');
                ok($.grep(profile.Settings.RolesMapping,
                    function(element) {
                        return element.Key == 'Assignee' && element.Value.Name == 'Developer';
                    }).length > 0, 'Assignee is mapped to Developer');
                ok($.grep(profile.Settings.RolesMapping,
                    function(element) {
                        return element.Key == 'Reporter' && element.Value.Name == 'QA Engineer';
                    }).length > 0, 'Reporter is mapped to QA Engineer');
            };
            this.initializer.initialize();
        });

        test('should initialize roles mapping during profile edit', function() {
            var that = this;
            this.initializer.profileRepository.getCurrentProfile = function(success) {
                success(that.profile);
            };
            this.initializer.profileController.render = function(profile) {
                ok(profile.Settings.RolesMapping.length == 2, 'there are 2 items in the roles mapping');
                ok($.grep(profile.Settings.RolesMapping,
                    function(element) {
                        return element.Key == 'Assignee' && element.Value.Name;
                    }).length > 0, 'Assignee is mapped');
                ok($.grep(profile.Settings.RolesMapping,
                    function(element) {
                        return element.Key == 'Reporter' && element.Value.Name;
                    }).length > 0, 'Reporter is mapped');
            };
            this.initializer.initialize();
        });

        test('should initialize roles mapping when some roles absent', function() {
            this.profile.Settings.RolesMapping = [this.profile.Settings.RolesMapping[0]];
            var that = this;
            this.initializer.profileRepository.getCurrentProfile = function(success) {
                success(that.profile);
            };
            this.initializer.profileController.render = function(profile) {
                ok(profile.Settings.RolesMapping.length == 2, 'there are 2 items in the roles mapping');
                ok($.grep(profile.Settings.RolesMapping,
                    function(element) {
                        return element.Key == 'Assignee' && element.Value.Name;
                    }).length > 0, 'Assignee is mapped');
                ok($.grep(profile.Settings.RolesMapping,
                    function(element) {
                        return element.Key == 'Reporter' && element.Value.Name;
                    }).length > 0, 'Reporter is mapped');
            };
            this.initializer.initialize();
        });

        test('should initialize reporter as Verifier', function() {
            var roles = [
                {
                    "Id":1,
                    "Name":"Developer",
                    "IsPair":true
                },
                {
                    "Id":2,
                    "Name":"Verifier",
                    "IsPair":false
                }
            ];
            this.initializer.restService.getRoles = function(processes, success) {
                success(roles);
            };
            this.initializer.profileController.render = function(profile) {
                ok(profile.Projects.length > 0, 'projects initialized');
                ok(profile.Roles.length > 0, 'roles initialized');
                ok($.grep(profile.Settings.RolesMapping,
                    function(element) {
                        return element.Key == 'Assignee' && element.Value.Name == 'Developer';
                    }).length > 0, 'Assignee is mapped to Developer');
                ok($.grep(profile.Settings.RolesMapping,
                    function(element) {
                        return element.Key == 'Reporter' && element.Value.Name == 'Verifier';
                    }).length > 0, 'Reporter is mapped to Verifier');
            };
            this.initializer.initialize();
        });
    });