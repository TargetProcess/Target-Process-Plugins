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
    .addDependency('tau/services/service.search')
    .addDependency('tau/services/service.navigator')
    .addDependency('tau/models/model.search-resolver')
    .addDependency('Searcher/SearchApplication')
    .addDependency('tp3/mashups/topmenu')
    .addDependency('tau-intl')
    .addDependency('jquery.notifyBar')
    .addDependency('jquery.tauResetableInput')
    .addMashup(function(_, $, AppCreator, Event,
        Class, busRegistry, ServiceSearch, ServiceNavigator,
        SearchResolverUtils, searchApplicationCreator,
        topMenu, intl) {

        var APP_ID = 'sp';
        var OPEN_SEARCH_CLASS = 'tau-menu-item-search--open';

        var Processor = Class.extend({
            init: function(bus) {
                this.bus = bus;
                Event.subscribeOn(this);
            },

            setupSearchTogglePanel: function($searchPanel) {
                var showSearchIcon = $searchPanel.find('.i-role-show'),
                    closeSearchIcon = $searchPanel.find('.i-role-close'),
                    searchInput = $searchPanel.find('.i-role-search-string').tauResetableInput();

                showSearchIcon.click(function() {
                    $searchPanel.addClass(OPEN_SEARCH_CLASS);
                    // bug #62842 - increased delay for Chrome 27
                    setTimeout(function() {
                        searchInput.focus();
                    }, 200);
                });
                closeSearchIcon.click(function() {
                    $searchPanel.removeClass(OPEN_SEARCH_CLASS);
                });
            },

            setupSearchTrigger: function($node, configurator) {
                var $searchInput = $node.find('.i-role-search-string');
                configurator.service('search').onClear(function(p) {
                    $searchInput.val(p.searchString);
                    $searchInput.trigger('input');
                });

                var $trigger = $node.find('.i-role-search-form');
                // $trigger.on('submit') has problems in IE11:
                // sometimes IE stops submitting form with single input on ENTER key press;
                // therefore we listen directly for ENTER key press
                $trigger.on('keydown', function(e) {
                    if (e.keyCode !== $.ui.keyCode.ENTER) {
                        return;
                    }

                    e.preventDefault();

                    $('body').notifyBar('remove');

                    $searchInput.blur();

                    var searchString = _.trim($node.find('.i-role-search-string').val());
                    configurator.service('search').params().set('searchString', searchString);

                    SearchResolverUtils.resolveSearchToken(searchString, configurator)
                        .fail(function(message) {
                            $searchInput.focus();

                            $('body').notifyBar({
                                className: 'tau-system-message--error',
                                html: '<h3>' + message + '</h3>',
                                disableAutoClose: true
                            });

                        })
                        .done(function(r) {
                            var targetUrl = (r.entity === 'search') ? 'search' : (r.entity + '/' + r.id);
                            configurator.service('navigator').to(targetUrl);
                            configurator.isBoardEdition = true;
                            searchApplicationCreator(configurator).done(function(appBus) {
                                //TODO Bug#121572 - investigate why 'destroy' does not fire after some actions
                                appBus.on('destroy', function() {
                                    configurator.service('search').params().clear();
                                });
                            });
                        });
                });
            },

            setupSearchInput: function($node, parentConfigurator) {
                var searchSettings = {
                    searchString: '',
                    entityTypeId: null,
                    entityStateIds: null,
                    isAllProjects: false,
                    isAllTeams: false
                };

                var searchViewConfigurator = parentConfigurator.createChild();
                searchViewConfigurator.getHashService().setFakeWindow();
                searchViewConfigurator.registerService('navigator', new ServiceNavigator(searchViewConfigurator, {
                    parameterName: APP_ID
                }));

                searchViewConfigurator.registerService('search', new ServiceSearch({
                    storage: searchSettings,
                    configurator: searchViewConfigurator
                }));

                this.setupSearchTrigger($node, searchViewConfigurator);
                this.setupSearchTogglePanel($node);
            },

            'bus configurator.ready': function(e, configurator) {
                var $searchInput = $([
                    '<span ' +
                    '   class="i-role-show tau-icon-general tau-icon-search tau-extension-board-tooltip" ' +
                    '   data-title="' + _.escape(intl.formatMessage('Global search')) + '"></span>',
                    '<div class="tau-main-search">',
                    '    <div class="tau-main-search__wrap">',
                    '        <form action="" class="i-role-search-form">',
                    '            <div class="tau-search">',
                    '                <div class="tau-search__wrap">',
                    '                    <input type="text" class="tau-search__input i-role-search-string" placeholder="' + _.escape(intl.formatMessage('Search')) + '">',
                    '                    <span class="tau-icon-general tau-icon-close-gray i-role-close" title="' + _.escape(intl.formatMessage('Close')) + '"></span>',
                    '                </div>',
                    '            </div>',
                    '        </form>',
                    '        <span class="tau-icon-general tau-icon-r-direction i-role-close" title="Close"></span>',
                    '    </div>',
                    '</div>'
                ].join(''));

                $searchInput = topMenu.addItem({
                    html: $searchInput,
                    cssClass: 'tau-menu-item-search i-role-mashup-header-search',
                    indexSection: 2
                }).$element;

                this.setupSearchInput($searchInput, configurator);
            }
        });

        busRegistry.getByName('board page container').then(function(masterBus) {
            new Processor(masterBus);
        });
    });
