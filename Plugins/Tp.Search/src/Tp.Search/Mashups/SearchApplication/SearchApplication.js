tau.mashups
.addDependency('jQuery')
.addDependency('application.creator')
.addDependency('tau/core/event')
.addDependency('tau/core/class')
.addDependency('tau/core/bus.reg')
.addDependency('tau/services/service.search')
.addDependency('tau/service.container')
.addDependency('libs/jquery/jquery.ui.tauBubble')
.addDependency('libs/parseUri')
.addDependency('tau/utils/utils.urlBuilder.service.tp2')
.addModule('Searcher/SearchApplication', function ($, AppCreator, Event, Class, busRegistry, ServiceSearch, serviceContainerClass, buble, parseUri, UrlBuilder) {

        var appCreator = function(configurator) {

            var APP_ID = 'sp';
            var appIdSeedUrl = configurator.getExternal().getHashParam(APP_ID).toLowerCase();

            configurator._id = _.uniqueId('search_results_viewer');
            if (!configurator.isBoardEdition){
                configurator.registerService('urlBuilder', new UrlBuilder(configurator));
            }

            var PAGE_NAME = 'entity component';

            var ROUTING_PATTERNS = [
                {
                    pattern: /search/,
                    adapter: function () {
                        this.resolve({ id: null, entity: 'search' });
                    },
                    type: 'tau/components/component.page.search',
                    host: 'tau/components/component.master.empty'
                },
                {
                    pattern: /(\w+)\/([0-9]+)($|\s|&)/,
                    adapter: function (entityType, entityId) {
                        this.resolve({
                            id: parseInt(entityId),
                            entity: {
                                id: parseInt(entityId),
                                type: entityType
                            },
                            action: 'show',
                            handlers: {}
                        });
                    },
                    type: {
                        name: PAGE_NAME,
                        type: 'page.entity'
                    },
                    host: 'tau/components/component.master.empty'
                },
                {
                    pattern: /(\w+)\/([0-9]+)\/(\w+)[&|]*.*/,
                    adapter: function (entityType, entityId, action) {
                        this.resolve({
                            id: parseInt(entityId),
                            entity: {
                                id: parseInt(entityId),
                                type: entityType
                            },
                            action: action,
                            handlers: {}
                        });
                    },
                    type: {
                        name: PAGE_NAME,
                        type: 'page.entity'
                    },
                    host: 'tau/components/component.master.empty'
                }
            ];

            var appConfig = {
                name: 'application board',
                applicationId:APP_ID,
                routes:ROUTING_PATTERNS,
                configurator: configurator,
                options: {
                    routing: {
                        silent: true
                    },
                    comet: { enabled: true }
                },
                integration: {
                    disableProgressIndicator: true,
                    showInPopup: true,
                    keepAlive: false,
                    cssClass: (appIdSeedUrl === 'search' ? 'tau-search-popup' : '')
                }
            };

            return AppCreator
                .create(appConfig)
                .pipe(function(appBus) {
                    var $return = $.Deferred();

                    appBus.on('innerContentRendered', function(e, renderData) {
                        var $el = renderData.element;

                        var searchParam = configurator.getExternal().getHashParam(APP_ID).toLowerCase();
                        var isSearch = ('search' === searchParam);

                        var $popupRoleNode = $el.parents('.ui-popup');

                        if (isSearch) {
                            $popupRoleNode.addClass('tau-search-popup');
                        }
                        else {
                            $popupRoleNode.removeClass('tau-search-popup');
                        }
                    });

                    appBus.initialize(appConfig);

                    $return.resolve(appBus);
                    return $return;
                });
        };
        return appCreator;
});