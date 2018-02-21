tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('tau/core/class')
    .addDependency('tp/search/services/service.search.command')
    .addModule('tp/search/services/service.search.command.multiIds', function ($, _, Class, SearchCommand) {
        var ServiceSearchMultiIdsCommand = SearchCommand.extend({
            init: function (configurator, params, $resultDeferred) {
                this._super(configurator, params, $resultDeferred);
                this.currentPageFromIndex = params.pageNo * params.pageSize;
                this.currentPageToIndex = this.currentPageFromIndex + params.pageSize;
                this.searchParameters = this._parseSearchString();
            },
            execute: function () {
                this._searchItemsPromise().done(_.bind(this._done, this));
            },
            _parseSearchString: function () {
                var parts = this.params.searchString.split(',');
                var Ids = [];
                _.forEach(parts, function (p) {
                    var token = _.trim(p);
                    var isDigit = /^\d+$/.test(token);
                    if (isDigit) {
                        Ids.push(parseInt(token));
                    }
                });
                return {
                    Ids: Ids
                };
            },
            _searchItemsPromise: function () {
                var store = this.configurator.getStore();
                var $searchItemsDeferred = $.Deferred();
                store.get('general', {
                    fields: ['id', {
                        entityType: ['name']
                    }],
                    $query: {
                        id: {
                            $in: this.searchParameters.Ids
                        }
                    },
                    $orderBy: 'id'
                }).done(function (result) {
                    $searchItemsDeferred.resolve(result[0].data);
                });
                return $searchItemsDeferred.promise();
            },
            _done: function (items) {
                var adapter = this._createResultAdapter(items, _.bind(this._resultFilter, this));
                this._super(_.bind(this._fetchDetailsByIDs, this)(adapter));
            },
            _createResultAdapter: function (items, filter) {
                var store = this.configurator.getStore();
                var adapter = _(items)
                    .chain()
                    .filter(filter)
                    .reduce(_.bind(function (memo, item) {
                        var typeName = item.entityType.name;
                        var memoKey = typeName + 'Ids';
                        if (!memo.hasOwnProperty(memoKey)) {
                            var ref = store.typeMetaInfo(typeName);
                            memoKey = _.titleize(ref.parent) + 'Ids';
                        }

                        if (memo.hasOwnProperty(memoKey)) {
                            if (this._shouldBeShownInCurrentPage(memo.Total)) {
                                memo[memoKey].push(item.id);
                            }
                            ++memo.Total;
                        }

                        return memo;

                    }, this), {
                        AssignableIds: [], CommentIds: [], GeneralIds: [], TestCaseIds: [], TestStepIds: [],
                        ImpedimentIds: [], Total: 0
                    })
                    .value();
                return adapter;
            },
            _shouldBeShownInCurrentPage: function (currentTotal) {
                return currentTotal >= this.currentPageFromIndex && currentTotal < this.currentPageToIndex;
            },
            _resultFilter: function (item) {
                return _.contains(this.searchParameters.Ids, item.id) && item.hasOwnProperty('entityType');
            }
        });

        ServiceSearchMultiIdsCommand.isSuitableCommand = function (searchString) {
            var parts = searchString.split(',');
            if (parts.length > 1) {
                return _.every(parts, function (p) {
                    var token = _.trim(p);
                    var isDigit = /^\d+$/.test(token);
                    return isDigit;
                });
            }
            return false;
        };

        return ServiceSearchMultiIdsCommand;
    });
