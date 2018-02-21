tau.mashups
    .addDependency("tp/plugins/commandGateway")
    .addDependency("Bugzilla/RestService")
    .addDependency("Bugzilla/MappingView")
    .addDependency("libs/jquery/jquery")
    .addModule("Bugzilla/MappingController", function (commandGateway, restService, view, $) {
        function mappingController(config) {
            this._ctor(config);
        }

        mappingController.prototype = {
            _ctor: function (config) {
                this.commandGateway = new commandGateway();
                this.restService = new restService();
                this.connectionChecker = config.connectionChecker;
                this.placeholder = config.placeholder;
                this.profileRetriever = config.profileRetriever;
                this.statesView = new view({
                    placeholder: this.placeholder,
                    mappingTemplate: 'batched',
                    key: 'StatesMapping',
                    Caption: 'States Mapping',
                    Description: 'Example: Resolved <span class="mapping-notes-chain">&nbsp;</span> Fixed',
                    KeyName: 'Bugzilla Status',
                    ValueName: 'TargetProcess State',
                    HowTo: '<a href="https://guide.targetprocess.com/settings/processes.html" target="_blank">How to change States in TargetProcess</a>',
                    onAutomap: $.proxy(this.automap, this)
                });

                this.severitiesView = new view({
                    placeholder: this.placeholder,
                    mappingTemplate: 'batched',
                    key: 'SeveritiesMapping',
                    Caption: 'Severities Mapping',
                    Description: 'Example: Critical <span class="mapping-notes-chain">&nbsp;</span> Major',
                    KeyName: 'Bugzilla Severity',
                    ValueName: 'TargetProcess Severity',
                    HowTo: '<a href="https://guide.targetprocess.com/settings/how-to-customize-bug-severity-values.html" target="_blank">How to change Severities for Bugs in TargetProcess</a>',
                    onAutomap: $.proxy(this.automap, this)
                });

                this.prioritiesView = new view({
                    placeholder: this.placeholder,
                    mappingTemplate: 'batched',
                    key: 'PrioritiesMapping',
                    Caption: 'Priorities Mapping',
                    Description: 'Example: Good <span class="mapping-notes-chain">&nbsp;</span> Must Have',
                    KeyName: 'Bugzilla Priority',
                    ValueName: 'TargetProcess Priority',
                    HowTo: '<a href="https://guide.targetprocess.com/settings/business-value.html" target="_blank">How to change Priorities for Bugs in TargetProcess</a>',
                    onAutomap: $.proxy(this.automap, this)
                });

                this.customView = new view({
                    placeholder: this.placeholder,
                    mappingTemplate: 'batched',
                    key: 'CustomMapping',
                    Caption: 'Custom Mapping',
                    Description: 'Example: cf_tpbugid <span class="mapping-notes-chain">&nbsp;</span> BugID',
                    KeyName: 'Bugzilla CustomField',
                    ValueName: 'TargetProcess BugField',
                    HowTo: 'Map TargetProcess Bug Fields with Custom Fields in Bugzilla (avialable from Bugzilla 5.0 with updated tp2.cgi)',
                    onAutomap: $.proxy(this.automap, this)
                });

                this.rolesView = new view({
                    placeholder: this.placeholder,
                    mappingTemplate: 'standalone',
                    key: 'RolesMapping',
                    Caption: 'Map Roles',
                    Description: 'Example: Assignee <span class="mapping-notes-chain">&nbsp;</span> Developer',
                    KeyName: 'Bugzilla Role',
                    ValueName: 'TargetProcess Role'
                });
            },

            render: function (profile) {
                this.statesView.initialize();
                this.connectionChecker.initialize();

                var roles = this._transformDataSource(profile.Settings.RolesMapping, profile.Roles);
                this.rolesView.render('.bugzilla-settings.roles')(roles);

                var that = this;

                var requestsInProgress = 2;

                var tpItems = {};

                var onSuccess = function () {
                    that._onGetMappingSuccess(tpItems)({
                        States: profile.Settings.StatesMapping,
                        Priorities: profile.Settings.PrioritiesMapping,
                        Severities: profile.Settings.SeveritiesMapping,
                        CustomFields: profile.Settings.CustomMapping
                    });
                }

                if (parseInt(profile.Settings.Project) <= 0) {
                    requestsInProgress--;
                } else {
                    that.commandGateway.executeForProfile('GetBugFields', null,
                        function(bugfields) {
                            requestsInProgress--;
                            tpItems.bugfields = bugfields;
                            if (requestsInProgress === 0)
                                onSuccess();
                        });
                }

                this.restService.getBugInfo(profile.Settings.Project, function (tpData) {
                    requestsInProgress--;
                    tpItems.states = tpData.states;
                    tpItems.severities = tpData.severities;
                    tpItems.priorities = tpData.priorities;
                    tpItems.roles = tpData.roles;
                    if (requestsInProgress === 0)
                        onSuccess();
                });
            },

            automap: function () {
                var profile = this.profileRetriever();
                this.connectionChecker.checkConnection($.proxy(this._getAutomapping(profile), this));
            },

            _getAutomapping: function (profile) {
                var that = this;
                return function (data) {
                    that.placeholder.find('span#automapPreloader').show();

                    var dataSource = {
                        States: { ThirdPartyItems: data.Statuses },
                        Severities: { ThirdPartyItems: data.Severities },
                        Priorities: { ThirdPartyItems: data.Priorities },
                        CustomFields: { ThirdPartyItems: data.CustomFields },
                        Roles: {
                            ThirdPartyItems: $.map(profile.Settings.RolesMapping, function (element) {
                                return element.Key;
                            })
                        }
                    };

                    var requestsInProgress = 2;

                    var onSuccess = function () {
                        that.commandGateway.executeForProfile('GetMapping', dataSource,
                            $.proxy(that._onGetMappingSuccess({
                                states: dataSource.States.TpItems,
                                severities: dataSource.Severities.TpItems,
                                priorities: dataSource.Priorities.TpItems,
                                bugfields: dataSource.CustomFields.TpItems,
                                roles: dataSource.Roles.TpItems
                            }, that)));
                    }

                    that.commandGateway.executeForProfile('GetBugFields', null,
                        function (bugfields) {
                            requestsInProgress--;
                            dataSource.CustomFields.TpItems = bugfields;
                            if (requestsInProgress === 0)
                                onSuccess();
                        });

                    that.restService.getBugInfo(profile.Settings.Project,
                        function (tpInfo) {
                            requestsInProgress--;
                            dataSource.States.TpItems = tpInfo.states;
                            dataSource.Severities.TpItems = tpInfo.severities;
                            dataSource.Priorities.TpItems = tpInfo.priorities;
                            dataSource.Roles.TpItems = profile.roles;
                            if (requestsInProgress === 0)
                                onSuccess();
                        }
                    );
                };
            },

            getStatesMapping: function () {
                return this.statesView.getMappings();
            },

            getSeveritiesMapping: function () {
                return this.severitiesView.getMappings();
            },

            getPrioritiesMapping: function () {
                return this.prioritiesView.getMappings();
            },

            getCustomMapping: function () {
                return this.customView.getMappings();
            },

            getRolesMapping: function () {
                return this.rolesView.getMappings();
            },

            _onGetMappingSuccess: function (tpItems) {
                var that = this;
                return function (data) {
                    var states = (typeof data != 'undefined') ? that._transformDataSource(data.States, tpItems.states) : [];
                    that.statesView.render('.bugzilla-map.states')(states);

                    var severities = (typeof data != 'undefined') ? that._transformDataSource(data.Severities, tpItems.severities) : [];
                    that.severitiesView.render('.bugzilla-map.severities')(severities);

                    var priorities = (typeof data != 'undefined') ? that._transformDataSource(data.Priorities, tpItems.priorities) : [];
                    that.prioritiesView.render('.bugzilla-map.priorities')(priorities);

                    var custom = (typeof data != 'undefined') ? that._transformDataSource(data.CustomFields, tpItems.bugfields) : [];
                    that.customView.render('.bugzilla-map.custom')(custom);

                    that.placeholder.find('span#automapPreloader').hide();
                };
            },

            _transformDataSource: function (dataSource, tpItems) {
                if (typeof dataSource == 'undefined') {
                    return [];
                }

                return $(dataSource).map(function (index, keyElement) {
                    return {
                        Key: keyElement.Key,
                        Value: $(tpItems).map(function (tpIndex, tpElement) {
                            return {
                                Id: tpElement.Id,
                                Name: tpElement.Name,
                                Selected: keyElement && keyElement.Value && tpElement.Id == keyElement.Value.Id
                            };
                        })
                    };
                });
            }
        };
        return mappingController;
    });