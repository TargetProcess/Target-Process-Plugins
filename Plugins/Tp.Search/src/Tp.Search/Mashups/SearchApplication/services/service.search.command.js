tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('tau/core/class')
    .addDependency('tp/search/services/service.search.resultHandler')
    .addModule('tp/search/services/service.search.command', function ($, _, Class, SearchResultHandler) {
        var ServiceSearchCommand = Class.extend({
            init: function (configurator, params, $resultDeferred) {
                this.configurator = configurator;
                this.params = params;
                this.$resultDeferred = $resultDeferred;
                this.$resultDeferred.done(_.bind(this._trackStats, this));
                this.searchResultHandler = new SearchResultHandler();
            },
            _fetchDetailsByIDs: function (r) {
                var indexProgress = _.defaults(r.IndexProgressData || {}, {
                    CompleteInPercents: 100
                });
                var rData = {
                    info: {
                        pageNo: this.params.pageNo,
                        pageSize: this.params.pageSize,
                        total: r.Total,
                        indexProgress: indexProgress.CompleteInPercents
                    },
                    items: []
                };

                var $r = $.Deferred();

                if (rData.info.total > 0) {

                    var assignableIds = r.AssignableIds;
                    var testStepIds = r.TestStepIds;
                    var generalIds = r.GeneralIds;
                    var impedimentIds = r.ImpedimentIds;
                    var commentIds = r.CommentIds;

                    var storeChain = this.configurator.getStore();

                    if (assignableIds.length) {
                        var $assignableQuery = {
                            fields: [
                                'id',
                                'name',
                                'description',
                                'tags',
                                'createDate',
                                {'entityType': ['id', 'name']},
                                {'entityState': ['id', 'name', 'isFinal']},
                                {'project': ['id', 'name']},
                                {'assignedTeams': [{'team': ['id', 'name', 'icon']}]}
                            ],
                            $query: {
                                id: {$in: assignableIds}
                            }
                        };
                        storeChain = storeChain.find('assignable', $assignableQuery);
                    }

                    if (testStepIds.length) {
                        var $testStepQuery = {
                            fields: [
                                'id',
                                'description',
                                'result',
                                'runOrder',
                                {
                                    'testCase': [
                                        'id',
                                        'name',
                                        {'entityType': ['id', 'name']}
                                    ]
                                }
                            ],
                            $query: {
                                id: {$in: testStepIds}
                            }
                        };
                        storeChain = storeChain.find('teststep', $testStepQuery);
                    }

                    if (generalIds.length) {
                        var $generalQuery = {
                            fields: [
                                'id',
                                'name',
                                'description',
                                'tags',
                                'createDate',
                                {'entityType': ['id', 'name']},
                                {'project': ['id', 'name']}
                            ],
                            $query: {
                                id: {$in: generalIds}
                            }
                        };
                        storeChain = storeChain.find('general', $generalQuery);
                    }

                    if (impedimentIds.length) {
                        var $impedimentQuery = {
                            fields: [
                                'id',
                                'name',
                                'description',
                                'tags',
                                'createDate',
                                {'entityType': ['id', 'name']},
                                {'entityState': ['id', 'name', 'isFinal']},
                                {'project': ['id', 'name']}
                            ],
                            $query: {
                                id: {$in: impedimentIds}
                            }
                        };
                        storeChain = storeChain.find('impediment', $impedimentQuery);
                    }

                    if (commentIds.length) {
                        var $commentQuery = {
                            fields: [
                                'id',
                                'description',
                                'createDate',
                                {'general': ['id', 'name', {'entityType': ['id', 'name']}]}
                            ],
                            $query: {
                                id: {$in: commentIds}
                            }
                        };
                        storeChain = storeChain.find('comment', $commentQuery);
                    }

                    storeChain.done({
                        success: function (r) {
                            var resultItems = [];
                            _.each(r, function (x) {
                                resultItems = resultItems.concat(x.data);
                            });
                            rData.items = _.sortBy(resultItems, function (e) {
                                return (-1 * e.id);
                            });

                            $r.resolve(rData);
                        },
                        failure: function (r) {
                            $r.reject(r);
                        }
                    });
                }
                else {
                    $r.resolve(rData);
                }

                return $r;
            },
            _trackStats: function(result) {
                taus.track({
                    action: 'global search / search',
                    tags: ['global search'],
                    sessionId: this.params.sessionId,
                    entityStateIds: this.params.entityStateIds,
                    entityTypeId: this.params.entityTypeId,
                    isAllProjects: !!this.params.isAllProjects,
                    isAllTeams: !!this.params.isAllTeams,
                    pageNo: this.params.pageNo,
                    pageSize: this.params.pageSize,
                    searchString: this.params.searchString,
                    totalResults: result.data.info.total,
                    '@': ['--exclude-board-context']
                });
            },
            _done: function ($searchDeferredChain) {
                this.searchResultHandler.handle($searchDeferredChain, this.$resultDeferred, this.params.searchString);
            }
        });

        ServiceSearchCommand.allowsToBeFiltered = false;

        return ServiceSearchCommand;
    });
