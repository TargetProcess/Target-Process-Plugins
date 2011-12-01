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
                    Description: 'Specify Bugzilla-to-TargetProcess states mapping by name (Ex: Resolved -> Fixed)<br/> Required for correct bugs import.',
                    KeyName: 'Bugzilla Status',
                    ValueName: 'TargetProcess State',
                    onAutomap: $.proxy(this.automap, this)
                });

                this.severitiesView = new view({
                    placeholder: this.placeholder,
                    mappingTemplate: 'batched',
                    key: 'SeveritiesMapping',
                    Caption: 'Severities Mapping',
                    Description: 'Specify Bugzilla-to-TargetProcess severities mapping by name (Ex: Critical -> Major)<br/> Required for correct bugs import.',
                    KeyName: 'Bugzilla Severity',
                    ValueName: 'TargetProcess Severity',
                    onAutomap: $.proxy(this.automap, this)
                });

                this.prioritiesView = new view({
                    placeholder: this.placeholder,
                    mappingTemplate: 'batched',
                    key: 'PrioritiesMapping',
                    Caption: 'Priorities Mapping',
                    Description: 'Specify Bugzilla-to-TargetProcess priorities mapping by name (Ex: Good -> Must Have)<br/> Required for correct bugs import.',
                    KeyName: 'Bugzilla Priority',
                    ValueName: 'TargetProcess Priority',
                    onAutomap: $.proxy(this.automap, this)
                });

                this.rolesView = new view({
                    placeholder: this.placeholder,
                    mappingTemplate: 'standalone',
                    key: 'RolesMapping',
                    Caption: 'Roles Mapping',
                    Description: 'Specify Bugzilla-to-TargetProcess roles mapping by name (Ex: Assignee -> Developer)<br/> Required for correct bugs import.',
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
                this.restService.getBugInfo(profile.Settings.Project, function (tpItems) {
                    that._onGetMappingSuccess(tpItems)(
                        {
                            States: profile.Settings.StatesMapping,
                            Priorities: profile.Settings.PrioritiesMapping,
                            Severities: profile.Settings.SeveritiesMapping
                        });
                });
            },

            automap: function () {
                var profile = this.profileRetriever();
                this.connectionChecker.checkConnection($.proxy(this._getAutomapping(profile), this), true);
            },

            _getAutomapping: function (profile) {
                var that = this;
                return function (data) {
                    that.placeholder.find('span#automapPreloader').show();
                    that.restService.getBugInfo(profile.Settings.Project,
                        function (tpInfo) {
                            var dataSource = {
                                States: {
                                    TpItems: tpInfo.states,
                                    ThirdPartyItems: data.Statuses
                                },
                                Severities: {
                                    TpItems: tpInfo.severities,
                                    ThirdPartyItems: data.Severities
                                },
                                Priorities: {
                                    TpItems: tpInfo.priorities,
                                    ThirdPartyItems: data.Priorities
                                },
                                Roles: {
                                    TpItems: profile.roles,
                                    ThirdPartyItems: $.map(profile.Settings.RolesMapping, function (element) {
                                        return element.Key;
                                    })
                                }
                            };
                            that.commandGateway.execute('GetMapping',
                                dataSource,
                                $.proxy(that._onGetMappingSuccess(tpInfo), that));
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