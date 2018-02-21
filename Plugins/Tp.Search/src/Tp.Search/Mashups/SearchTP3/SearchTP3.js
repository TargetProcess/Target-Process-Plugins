// intlScope: search-plugin
/*global tau*/
/*jshint maxlen:175*/
/*eslint max-len:[1,175,4]*/
tau.mashups
    .addDependency('Underscore')
    .addDependency('jQuery')
    .addDependency('application.creator')
    .addDependency('tau/core/event')
    .addDependency('tau/core/class')
    .addDependency('tau/core/bus.reg')
    .addDependency('tp/search/services/service.search')
    .addDependency('tau/services/service.navigator')
    .addDependency('tp/search/models/model.search.filter.resolver')
    .addDependency('tp/search/SearchApplication')
    .addDependency('tp/search/searchfield')
    .addDependency('tau-intl')
    .addDependency('jquery.notifyBar')
    .addDependency('jquery.tauResetableInput')
    .addMashup(function (_, $, AppCreator, Event, Class, busRegistry, ServiceSearch, ServiceNavigator,
        SearchResolverUtils, searchApplicationCreator, searchTemplate) {

        var APP_ID = 'searchPopup';
        var SEARCH2_NO_CONFLICT_APP_ID = 'search1Popup';
        var isSearchRunning = false;
        var searchApp = null;

        var Processor = Class.extend({
            init: function (bus) {
                this.bus = bus;
                Event.subscribeOn(this);
            },

            setupSearchTogglePanel: function ($searchPanel) {
                $searchPanel.find('.i-role-search-string').tauResetableInput();
            },

            setupSearchTrigger: function ($node, configurator) {
                this.$searchInput = $node.find('.i-role-search-string');
                configurator.service('search').onClear(function (p) {
                    this.$searchInput.val(p.searchString);
                    this.$searchInput.trigger('input');
                }.bind(this));

                var $trigger = $node.find('.i-role-search-form');
                // $trigger.on('submit') has problems in IE11:
                // sometimes IE stops submitting form with single input on ENTER key press;
                // therefore we listen directly for ENTER key press
                $trigger.on('keydown', function (e) {
                    if (e.keyCode !== $.ui.keyCode.ENTER) {
                        return;
                    }
                    e.preventDefault();

                    var searchString = _.trim($node.find('.i-role-search-string').val());

                    SearchResolverUtils.resolveSearchToken(searchString, configurator)
                        .fail(function (message) {
                            this.$searchInput.focus();
                            $('body').notifyBar({
                                className: 'tau-system-message--error',
                                html: '<h3>' + message + '</h3>',
                                disableAutoClose: true
                            });
                        }.bind(this))
                        .done(function (r) {
                            var isSearchResults = (r.entity === 'search');
                            var startUrl = (isSearchResults) ? 'query/' + encodeURIComponent(searchString) : (r.entity + '/' + r.id);
                            this.startSearch(configurator, startUrl);
                        }.bind(this));
                }.bind(this));

                configurator.getHashService().onHashChange(function () {
                    this._onHashChanged(configurator);
                }.bind(this));

                // first check on load
                this._onHashChanged(configurator);
            },

            startSearch: function (configurator, startUrl) {
                if (isSearchRunning || _.isEmpty(startUrl)) {
                    return;
                }
                isSearchRunning = true;

                $('body').notifyBar('remove');
                this.$searchInput.blur();

                configurator.service('navigator').to(startUrl);
                searchApplicationCreator(configurator)
                    .done(function (appBus) {
                        searchApp = appBus;
                        //TODO Bug#121572 - investigate why 'destroy' does not fire after some actions
                        appBus.on('destroy', function () {
                            configurator.service('search').params().clear();
                            configurator.service('navigator').toEmptyUrl();
                            isSearchRunning = false;
                            searchApp = null;
                        });
                    });
            },

            setupSearchInput: function ($node, parentConfigurator) {
                var searchSettings = {
                    searchString: '',
                    entityTypeId: null,
                    entityStateIds: null,
                    isAllProjects: false,
                    isAllTeams: false
                };

                var appId = parentConfigurator.getFeaturesService().isEnabled('search2')
                    ? SEARCH2_NO_CONFLICT_APP_ID
                    : APP_ID;

                var searchViewConfigurator = parentConfigurator.createChild();
                searchViewConfigurator.registerService('navigator', new ServiceNavigator(searchViewConfigurator, {
                    parameterName: appId
                }));

                searchViewConfigurator.registerService('search', new ServiceSearch({
                    storage: searchSettings,
                    configurator: searchViewConfigurator
                }));

                this.setupSearchTrigger($node, searchViewConfigurator);
                this.setupSearchTogglePanel($node);
            },

            _onHashChanged: function (configurator) {
                var url = configurator.service('navigator').getCurrent();

                if (_.isEmpty(url) && !_.isEmpty(searchApp)) {
                    searchApp.fire('popup.close');
                } else if (!isSearchRunning) {
                    this.startSearch(configurator, url);
                }
            },

            'bus configurator.ready': function (e, configurator) {
                if (!configurator.getFeaturesService().isEnabled('search')) return;
                this.setupSearchInput(searchTemplate, configurator);
            }
        });

        busRegistry.getByName('board page container').then(function (masterBus) {
            new Processor(masterBus);
        });
    });
