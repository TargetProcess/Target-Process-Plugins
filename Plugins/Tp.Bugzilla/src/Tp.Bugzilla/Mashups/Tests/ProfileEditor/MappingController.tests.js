require(["Bugzilla/MappingController", "Bugzilla/ConnectionChecker"],
    function (mappingController, connectionChecker) {
        module('test mapping controller', {
            setup: function () {
                this.placeholder = $('<div></div>');
                this.connectionChecker = new connectionChecker({
                    placeholder: this.placeholder
                });
                this.mappedByTpStates = [
                    { Key: "UNCONFIRMED", Value: { Id: 0, Name: null} },
                    { Key: "In Development", Value: { Id: 65, Name: "IN DEVELOPMENT"} },
                    { Key: "RESOLVED", Value: { Id: 6, Name: "Fixed"} }
                ];

                this.mappedByTpSeverities = [
                    { Key: "Critical", Value: { Id: 0, Name: null} },
                    { Key: "Blocking", Value: { Id: 1, Name: "BLOCKING"} },
                    { Key: "Small", Value: { Id: 2, Name: "Minor"} }
                ];

                this.mappedByTpPriorities = [
                    { Key: "Must", Value: { Id: 0, Name: null} },
                    { Key: "Urgent", Value: { Id: 1, Name: "URGENT"} },
                    { Key: "Enhancement", Value: { Id: 2, Name: "Nice to have"} }
                ];

                this.profile = {
                    "Name": "bz",
                    "Settings": {
                        "Login": "bugzilla@targetprocess.com",
                        "Password": "bugzillaadmin",
                        "Project": 1,
                        "Queries": "alla plugin test",
                        "Url": "http:\/\/new-bugzilla\/bugzilla363",
                        StatesMapping: this.mappedByTpStates,
                        PrioritiesMapping: this.mappedByTpPriorities,
                        SeveritiesMapping: this.mappedByTpSeverities,
                        RolesMapping: [
                            { "Key": "Assignee", "Value": { "Id": 1, "Name": "Developer"} },
                            { "Key": "Reporter", "Value": { "Id": 4, "Name": "QA Engineer"} }
                        ]
                    }
                };

                var that = this;
                this.profileRetriever = function () {
                    return that.profile;
                };
                this.mappingController = new mappingController({
                    connectionChecker: this.connectionChecker,
                    placeholder: this.placeholder,
                    model: {},
                    projectRetriever: function () {
                        return that.profile.Settings.Project;
                    },
                    profileRetriever: this.profileRetriever
                });

                this.bugzillaItems = { statuses: ["Open", "In Progress", "Closed"], severities: ["Minor", "Major"], prioritites: ["Nice to have", "Great"] };
                this.tpItems = { states: [
                    { Id: 5, Name: "Open" },
                    { Id: 6, Name: "Fixed" },
                    { Id: 7, Name: "Invalid" },
                    { Id: 8, Name: "Closed" },
                    { Id: 65, Name: "IN DEVELOPMENT" }
                ],
                    severities: [
                        { Id: 1, Name: 'BLOCKING' },
                        { Id: 2, Name: 'Minor' },
                        { Id: 3, Name: 'Major' }
                    ],
                    priorities: [
                        { Id: 1, Name: 'URGENT' },
                        { Id: 2, Name: 'Nice to have' },
                        { Id: 3, Name: 'Great' }
                    ]
                };

                this.mappingController.restService = {
                    getBugInfo: function (projectId, success) {
                        success(that.tpItems);
                    }
                };

                this.mappingController.commandGateway = {
                    execute: function (commandName, data, success) {
                        return success({
                            States: that.mappedByTpStates,
                            Priorities: that.mappedByTpPriorities,
                            Severities: that.mappedByTpSeverities
                        });
                    }
                };

                this.connectionChecker.checkConnection = function (success) {
                    success(that.bugzillaItems);
                };
            },

            teardown: function () {
            }
        });

        test('should map states, severities, priorities', function () {
            var checkSelected = function (dropDown, name) {
                if (typeof name == 'undefined') {
                    return $.grep(dropDown.Value, function (element) {
                        return (element.Selected === true);
                    }).length == 0;
                }
                return $.grep(dropDown.Value, function (element) {
                    return (element.Selected === true && element.Name === name);
                }).length == 1;
            };

            var checkDropDownElement = function (element, count, selectedName) {
                ok(element.Value.length === count, 'Count of drop down values should be equals to count of states in TargetProcess');
                ok(checkSelected(element, selectedName), 'Should be selected appropriate item: ' + selectedName);
            };

            this.mappingController.statesView.render = function () {
                return function (data) {
                    ok(data.length === 3, 'Count of mapped states should be 3');

                    checkDropDownElement(data[0], 5);
                    checkDropDownElement(data[1], 5, 'IN DEVELOPMENT');
                    checkDropDownElement(data[2], 5, 'Fixed');
                };
            };

            this.mappingController.severitiesView.render = function () {
                return function (data) {
                    ok(data.length === 3, 'Count of mapped severities should be 3');

                    checkDropDownElement(data[0], 3);
                    checkDropDownElement(data[1], 3, 'BLOCKING');
                    checkDropDownElement(data[2], 3, 'Minor');
                };
            };

            this.mappingController.prioritiesView.render = function () {
                return function (data) {
                    ok(data.length === 3, 'Count of mapped severities should be 3');

                    checkDropDownElement(data[0], 3);
                    checkDropDownElement(data[1], 3, 'URGENT');
                    checkDropDownElement(data[2], 3, 'Nice to have');
                };
            };

            this.mappingController.render(this.profile);
        });

        test('should automap states, severities, priorities', function () {
            this.mappingController.render(this.profile);
            var checkSelected = function (dropDown, name) {
                if (typeof name == 'undefined') {
                    return $.grep(dropDown.Value, function (element) {
                        return (element.Selected === true);
                    }).length == 0;
                }
                return $.grep(dropDown.Value, function (element) {
                    return (element.Selected === true && element.Name === name);
                }).length == 1;
            };

            var checkDropDownElement = function (element, count, selectedName) {
                ok(element.Value.length === count, 'Count of drop down values should be equals to count of states in TargetProcess');
                ok(checkSelected(element, selectedName), 'Should be selected appropriate item: ' + selectedName);
            };

            this.mappingController.statesView.render = function () {
                return function (data) {
                    ok(data.length === 3, 'Count of mapped states should be 3');

                    checkDropDownElement(data[0], 5);
                    checkDropDownElement(data[1], 5, 'IN DEVELOPMENT');
                    checkDropDownElement(data[2], 5, 'Fixed');
                };
            };

            this.mappingController.severitiesView.render = function () {
                return function (data) {
                    ok(data.length === 3, 'Count of mapped severities should be 3');

                    checkDropDownElement(data[0], 3);
                    checkDropDownElement(data[1], 3, 'BLOCKING');
                    checkDropDownElement(data[2], 3, 'Minor');
                };
            };

            this.mappingController.prioritiesView.render = function () {
                return function (data) {
                    equal(data.length, 3, 'Count of mapped severities should be 3');

                    checkDropDownElement(data[0], 3);
                    checkDropDownElement(data[1], 3, 'URGENT');
                    checkDropDownElement(data[2], 3, 'Nice to have');
                };
            };


            this.mappingController.automap();
        });
    });