tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('tau/core/class')
    .addModule('tp/search/services/service.search.command.text.contextBuilder', function ($, _, Class) {
        var ServiceSearchContextBuilder = Class.extend({
            init: function (configurator, params) {
                this.configurator = configurator;
                this.params = params;
            },
            getContextPromise: function () {
                var contextDeferred = $.Deferred();
                this._getInitialContextPromise().done(_.bind(function (context) {
                    var $byTeams = $.Deferred();
                    var $byProjs = $.Deferred();

                    var storeChain = this.configurator.getStore();
                    var selectedIndirectProjs = _.difference(context.selectedProjectIds,
                        context.selectedProjectIdsAvailable);
                    var selectedIndirectTeams = _.difference(context.selectedTeamIds, context.selectedTeamIdsAvailable);

                    var targetTeamsSet = context.isAllProjects ? context.selectedTeamIds : selectedIndirectTeams;
                    targetTeamsSet = _.without(targetTeamsSet, 'null');

                    var targetProjectsSet = context.isAllTeams ? context.selectedProjectIds : selectedIndirectProjs;
                    targetProjectsSet = _.without(targetProjectsSet, 'null');

                    var needHandleTeams = (targetTeamsSet.length !== 0);
                    var needHandleProjects = (targetProjectsSet.length !== 0);

                    if (needHandleTeams) {
                        storeChain = this._handleTeams(storeChain, targetTeamsSet, $byTeams);
                    } else {
                        $byTeams.resolve({});
                    }

                    if (needHandleProjects) {
                        storeChain = this._handleProjects(storeChain, targetProjectsSet, $byProjs);
                    } else {
                        $byProjs.resolve({});
                    }

                    if (needHandleTeams || needHandleProjects) {
                        storeChain.done();
                    }

                    $.when($byTeams, $byProjs).then(function (projectsByTeams, teamsByProjects) {
                        var r = {};

                        var mergeObjects = function (v, k) {
                            var x = _.union((r[k] || []), v);
                            x.length && (r[k] = x);
                        };

                        _.each(projectsByTeams, mergeObjects);
                        _.each(teamsByProjects, mergeObjects);

                        context.teamProjectRelations = _(r).chain()
                            .keys()
                            .reduce(function (memo, teamId) {
                                teamId = parseInt(teamId, 10);
                                var projects = memo[teamId];

                                // rules
                                if ((context.isAllProjects || context.isAllTeams) &&
                                    _.contains(context.selectedTeamIdsAvailable, teamId)) {
                                    // indirect PROJECT selection
                                    projects = _.difference(projects, context.selectedProjectIdsAvailable);
                                } else if (context.isAllTeams && !_.contains(context.selectedTeamIdsAvailable, teamId)) {
                                    // indirect TEAM selection
                                    // projects = projects;
                                } else if (_.contains(selectedIndirectTeams, teamId)) {
                                    // indirect TEAM selection
                                    projects = _.intersection(projects, context.selectedProjectIds);
                                } else if (_.contains(context.selectedTeamIdsAvailable, teamId)) {
                                    // indirect PROJECT selection
                                    projects = _.intersection(projects, selectedIndirectProjs);
                                } else {
                                    projects = [];
                                    delete memo[teamId];
                                }

                                (projects.length) ? (memo[teamId] = projects) : (delete memo[teamId]);

                                return memo;
                            }, r)
                            .value();

                        contextDeferred.resolve(context);
                    });
                }, this));

                return contextDeferred.promise();
            },

            _getInitialContextPromise: function () {

                var ctxService = this.configurator.getApplicationContextService();
                var user = this.configurator.getLoggedUser();

                var $projects = $.Deferred();
                var $teams = $.Deferred();

                var $context = $.Deferred();
                var $teamProjectsAvailable = $.Deferred();

                var needAllProjects = this.params.isAllProjects;
                var needAllTeams = this.params.isAllTeams;

                if (needAllProjects) {
                    $.when($teamProjectsAvailable).done(function (r) {
                        var projectIds = _(r.projects).pluck('id');
                        $projects.resolve({
                            isAllProjects: true,
                            selectedProjectIds: projectIds,
                            selectedProjectIdsAvailable: projectIds
                        });
                    });
                } else {
                    $.when($context)
                        .done(function (r) {
                            $projects.resolve({
                                selectedProjectIds: r.selectedProjectIds,
                                selectedProjectIdsAvailable: r.selectedProjectIdsAvailable
                            });
                        });
                }

                if (needAllTeams) {
                    $.when($teamProjectsAvailable).done(function (r) {
                        var teamIds = _(r.teams).pluck('id');
                        $teams.resolve({
                            isAllTeams: true,
                            includeNoTeam: true,
                            selectedTeamIds: teamIds,
                            selectedTeamIdsAvailable: teamIds
                        });
                    });
                } else {
                    $.when($context)
                        .done(function (r) {
                            $teams.resolve({
                                includeNoTeam: r.appContext.teamContext.no,
                                selectedTeamIds: r.selectedTeamIds,
                                selectedTeamIdsAvailable: r.selectedTeamIdsAvailable
                            });
                        });
                }

                if (needAllProjects || needAllTeams) {
                    ctxService.getTeamsProjectsAvailable(user.id)
                        .done($teamProjectsAvailable.resolve);
                }

                if (!needAllProjects || !needAllTeams) {
                    var acidStore = this.configurator.getAppStateStore();
                    acidStore.get({
                        fields: ['acid']
                    })
                        .then(function (r) {
                            return ctxService.getApplicationContext({acid: r.acid});
                        })
                        .done($context.resolve);
                }

                return $.when($projects, $teams).then(function (projects, teams) {
                    return _.extend(projects, teams);
                });
            },

            _handleTeams: function (storeChain, targetTeamsSet, $byTeams) {
                storeChain = storeChain.find(
                    'teams',
                    {
                        fields: ['id', {teamProjects: ['id', {project: ['id']}]}],
                        $query: {id: {$in: targetTeamsSet}},
                        list: true
                    },
                    {
                        success: function (src) {
                            var teams = src.data;

                            var teamProjectRelationsByTeams = _(teams).reduce(function (memo, t) {
                                var projects = _(t.teamProjects).map(function (tp) {
                                    return tp.project.id;
                                });

                                projects.length && (memo[t.id] = projects);

                                return memo;
                            }, {});

                            $byTeams.resolve(teamProjectRelationsByTeams);
                        }
                    });
                return storeChain;
            },

            _handleProjects: function (storeChain, targetProjectsSet, $byProjs) {
                storeChain = storeChain.find(
                    'projects',
                    {
                        fields: ['id', {teamProjects: ['id', {team: ['id']}]}],
                        $query: {id: {$in: targetProjectsSet}},
                        list: true
                    },
                    {
                        success: function (src) {
                            var projs = src.data;

                            var teamProjectRelationsByProjs = _(projs).reduce(function (memo, p) {
                                var teams = _(p.teamProjects).map(function (tp) {
                                    return tp.team.id;
                                });

                                memo = _(teams).reduce(function (m, teamId) {
                                    m[teamId] = m[teamId] || [];
                                    m[teamId].push(p.id);
                                    return m;
                                }, memo);

                                return memo;
                            }, {});

                            $byProjs.resolve(teamProjectRelationsByProjs);
                        }
                    });
                return storeChain;
            }
        });

        return ServiceSearchContextBuilder;
    });
