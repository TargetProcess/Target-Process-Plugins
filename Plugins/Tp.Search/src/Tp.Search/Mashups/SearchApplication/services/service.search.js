tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('tau/ui/ui.templateFactory')
    .addDependency('tau/core/class')
    .addDependency('tp/search/services/service.search.commandFactory')
    .addDependency('tp/search/services/service.search.command.multiIdsParts')
    .addDependency('tp/search/services/service.search.command.text')
    .addModule('tp/search/services/service.search', function ($, _, templateFactory, Class, searchCommandFactory) {
        var Service = Class.extend({
            init: function (config) {
                var x = this;

                x.defaultParams = {
                    searchString: '',
                    entityTypeId: null,
                    entityStateIds: null,
                    pageSize: 10,
                    pageNo: 0
                };

                x.storage = config.storage;
                x._params = config.storage;
                x.configurator = config.configurator;

                x.$paramsChangedCallbacks = $.Callbacks();
                x.$dParamsChanged = $.Deferred();
                x.$dParamsChanged.progress(x.$paramsChangedCallbacks.fire);

                x.$clearCallbacks = $.Callbacks();
                x.$dClear = $.Deferred();
                x.$dClear.progress(x.$clearCallbacks.fire);

                x.$submitCallbacks = $.Callbacks();
                x.$dSubmit = $.Deferred();
                x.$dSubmit.progress(x.$submitCallbacks.fire);

                x.$searchCallbacks = $.Callbacks();
                x.$dSearch = $.Deferred();
                x.$dSearch.progress(x.$searchCallbacks.fire);

                this._generateSessionId();
            },

            asDisposable: function (callback, methodName) {
                var o = this;
                var r = {};
                r.dispose = _.bind(function (o, callback, methodName) {
                    o[methodName](callback);
                }, r, o, callback, methodName);

                return r;
            },

            onParamsChanged: function (callback) {
                var o = this;
                o.$paramsChangedCallbacks.add(callback);
                return o.asDisposable(callback, 'offParamsChanged');
            },

            offParamsChanged: function (callback) {
                this.$paramsChangedCallbacks.remove(callback);
            },

            onClear: function (callback) {
                this.$clearCallbacks.add(callback);
                return this.asDisposable(callback, 'offClear');
            },

            offClear: function (callback) {
                this.$clearCallbacks.remove(callback);
            },

            onSubmit: function (callback) {
                this.$submitCallbacks.add(callback);
                return this.asDisposable(callback, 'offSubmit');
            },

            offSubmit: function (callback) {
                this.$submitCallbacks.remove(callback);
            },

            onSearch: function (callback) {
                this.$searchCallbacks.add(callback);
                return this.asDisposable(callback, 'offSearch');
            },

            offSearch: function (callback) {
                this.$searchCallbacks.remove(callback);
            },

            params: function () {
                var service = this;
                return {
                    get: function (key) {
                        return arguments.length ? service._params[key] : service._params;
                    },

                    set: function (key, val) {
                        var keys = [];
                        var params = {};
                        if (_.isString(key)) {
                            keys = [key];
                            params[key] = val;
                        } else if (_.isObject(key)) {
                            keys = _.keys(key);
                            params = key;
                        } else {
                            throw new Error('Unsupported argument [key]');
                        }

                        // Default pageNo parameter to zero 0 when any search params are changed
                        var isPageNoParameter = (keys.length === 1 && keys[0] === 'pageNo');
                        if (!isPageNoParameter) {
                            params.pageNo = 0;
                        }

                        var k = _.keys(params);
                        var delta = {};
                        _.each(k, function (key) {
                            if (params[key] !== service._params[key]) {
                                delta[key] = params[key];
                            }
                        });

                        service._params = _.extend(service._params, params);

                        service.$dParamsChanged.notify(delta);

                        return this;
                    },

                    submit: function () {
                        var params = service._params;
                        service.$dSubmit.notify(params);
                    },

                    clear: function () {
                        service._params = _.clone(service.defaultParams);
                        service._generateSessionId();
                        service.$dClear.notify(service._params);
                    }
                };
            },

            getSearchDomain: function (parameters) {
                parameters = parameters || this.params().get();

                var configurator = this.configurator;
                var $resultDefer = $.Deferred();
                var store = configurator.getStore();
                var ctxService = configurator.getApplicationContextService();

                configurator.getAppStateStore().get(['acid'])
                    .then(function (r) {
                        return ctxService.getApplicationContext({acid: r.acid});
                    }).then(function (r) {
                        var processIds = _.pluck(r.availableProcesses, 'id');
                        var contextProcessIds = _.pluck(r.processes, 'id');

                        store
                            .find('entityType', {
                                list: true,
                                fields: ['id', 'name'],
                                $query: {isSearchable: 1}
                            })
                            .find('entityState', {
                                list: true,
                                fields: ['id', 'name', {'entityType': ['id', 'name']}, {'process': ['id']}],
                                $query: {process: {id: {$in: processIds}}}
                            })
                            .done(function (r) {
                                var entityTypes = this._adaptEntityTypes(r[0].data);
                                var iterator = parameters.entityTypeId ?
                                    function (d) {
                                        return d.entityType.id == parameters.entityTypeId;
                                    } :
                                    function () {
                                        return true;
                                    };
                                var rawEntityStates = _.filter(r[1].data, iterator);
                                var entityStates = this._adaptEntityStates(rawEntityStates,
                                    parameters.isAllProjects ? processIds : contextProcessIds);

                                $resultDefer.resolve({
                                    entityTypes: entityTypes,
                                    entityStates: entityStates,
                                    params: parameters
                                });
                            }.bind(this));
                    }.bind(this));

                return $resultDefer;
            },

            search: function (params) {
                _.tauDefaults(params, this.defaultParams);

                return {
                    done: _.bind(function (callback) {
                        var $resultDeferred;
                        if (_.isFunction(callback)) {
                            $resultDeferred = $.Deferred();
                            $resultDeferred.always(callback);
                        } else {
                            $resultDeferred = callback;
                        }

                        $resultDeferred.always(this.$dSearch.notify);

                        var SearchCommand = this.getSuitableCommand(params.searchString);
                        new SearchCommand(this.configurator, params, $resultDeferred).execute();
                    }, this)
                };
            },

            getSuitableCommand: function (searchString) {
                return searchCommandFactory.getSuitableCommand(searchString);
            },

            _adaptEntityTypes: function (items) {
                var tp = templateFactory.getTermProcessor();
                return _
                    .chain(items)
                    .filter(function (x) {
                        return x.name.toLowerCase() !== 'solution';
                    }).map(function (x) {
                        x.term = tp.getTerm(x.name);
                        return x;
                    })
                    .reject(function (x) {
                        return !x.term;
                    })
                    .value();
            },

            _adaptEntityStates: function (items, contextProcessIds) {
                return _
                    .chain(items)
                    .reduce(function (memo, d) {
                        memo[d.name] = memo[d.name] || {contextStateIds: [], availableStateIds: []};
                        if (_.contains(contextProcessIds, d.process.id)) {
                            memo[d.name].contextStateIds.push(d.id);
                        }
                        memo[d.name].availableStateIds.push(d.id);
                        return memo;
                    }, {})
                    .pairs()
                    .reject(function (v) {
                        return v[1].contextStateIds.length === 0;
                    })
                    .map(function (v) {
                        var name = v[0];
                        var val = v[1];
                        return {
                            id: val.contextStateIds.join(','),
                            availableId: val.availableStateIds.join(','),
                            name: name
                        };
                    })
                    .sortBy(function (o) {
                        return o.name;
                    })
                    .value();
            },

            // for stats
            _generateSessionId: function () {
                var user = this.configurator.getLoggedUser();
                this._params.sessionId = user.id + '_' + _.UUID();
            }
        });

        return Service;
    });
