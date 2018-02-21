tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('application.creator')
    .addModule('tp/search/SearchApplication', function ($, _, AppCreator) {
        return function(configurator) {
            var APP_ID = 'searchPopup';
            var SEARCH2_NO_CONFLICT_APP_ID = 'search1Popup';
            var SEARCH_RESULTS_CSS_CLASS = 'tau-cover-view--search';

            configurator._id = _.uniqueId('search_results_viewer');

            var appId = configurator.getFeaturesService().isEnabled('search2')
                ? SEARCH2_NO_CONFLICT_APP_ID
                : APP_ID;

            var PAGE_NAME = 'entity component';
            var ROUTING_PATTERNS = [
                {
                    pattern: /query\/(.+)/,
                    adapter: function (searchString) {
                        var decodedSearchString = decodeURIComponent(searchString);
                        this.configurator.service('search').params().set('searchString', decodedSearchString);
                        this.resolve({ id: null, entity: decodedSearchString });
                    },
                    host: {
                        name: 'master empty',
                        type: 'master.empty'
                    },
                    type: {
                        name: 'page.search',
                        type: 'page.search',
                        namespace: 'tp/search'
                    }
                },
                {
                    pattern: /(\w+)\/([0-9]+)($|\s|&)/,
                    adapter: function (entityType, entityId) {
                        this.resolve({
                            id: parseInt(entityId, 10),
                            entity: {
                                id: parseInt(entityId, 10),
                                type: entityType
                            },
                            action: 'show',
                            handlers: {}
                        });
                    },
                    host: {
                        name: 'master empty',
                        type: 'master.empty'
                    },
                    type: {
                        name: PAGE_NAME,
                        type: 'page.entity'
                    }
                },
                {
                    pattern: /(\w+)\/([0-9]+)\/(\w+)[&|]*.*/,
                    adapter: function (entityType, entityId, action) {
                        this.resolve({
                            id: parseInt(entityId, 10),
                            entity: {
                                id: parseInt(entityId, 10),
                                type: entityType
                            },
                            action: action,
                            handlers: {}
                        });
                    },
                    host: {
                        name: 'master empty',
                        type: 'master.empty'
                    },
                    type: {
                        name: PAGE_NAME,
                        type: 'page.entity'
                    }
                }
            ];

            var integrationConfig = {
                disableProgressIndicator: true,
                showInCoverView: true,
                keepAlive: false
            };

            var searchParam = configurator
                .getHashService()
                .getHashParam(appId)
                .toLowerCase();
            var isSearchResults = _.startsWith(searchParam, 'query/');
            if (isSearchResults) {
                integrationConfig.cssClass = SEARCH_RESULTS_CSS_CLASS;
            }

            if (configurator.isBoardEdition) {
                integrationConfig.showInCoverView = true;
            } else {
                integrationConfig.showInPopup = true;
            }

            var appConfig = {
                name: 'application board',
                applicationId: appId,
                routes: ROUTING_PATTERNS,
                configurator: configurator,
                options: {
                    routing: {
                        silent: true
                    },
                    comet: {enabled: true}
                },
                integration: integrationConfig
            };

            return AppCreator
                .create(appConfig)
                .pipe(function(appBus) {
                    var $return = $.Deferred();

                    appBus.on('innerContentRendered', function(e, renderData) {
                        var $el = renderData.element;
                        var searchParam = configurator
                            .getHashService()
                            .getHashParam(appId).toLowerCase();
                        var isSearchResults = _.startsWith(searchParam, 'query/');
                        $el.parents('.tau-cover-view_page')
                            .toggleClass(SEARCH_RESULTS_CSS_CLASS, isSearchResults);
                    });

                    appBus.initialize(appConfig);

                    $return.resolve(appBus);
                    return $return;
                });
        };
    });
