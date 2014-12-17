tau.mashups
        .addDependency('Underscore')
        .addDependency('jQuery')
        .addDependency('application.creator')
        .addDependency('tau/core/event')
        .addDependency('tau/core/class')
        .addDependency('tau/core/bus.reg')
        .addDependency('tau/services/service.search')
        .addDependency('tau/services/service.navigator')
        .addDependency('tau/ui/behaviour/common/ui.behaviour.progressIndicator')
        .addDependency('tau/models/model.search-resolver')
        .addDependency('Searcher/SearchApplication')
        .addDependency('tp3/mashups/topmenu')
        .addMashup(function (_, $, AppCreator, Event, Class, busRegistry, ServiceSearch, ServiceNavigator, ProgressIndicator, SearchResolverUtils, searchApplicationCreator, topMenu) {

    var APP_ID = 'sp';

    var Processor = Class.extend({
        init: function(bus) {
            this.bus = bus;
            Event.subscribeOn(this);
        },

        setupSearchTogglePanel: function ($searchPanel) {
            var showSearchIcon = $searchPanel.find('.tau-icon-search'),
                closeSearchIcon = $searchPanel.find('.tau-search-close'),
                searchInput = $searchPanel.find('.i-role-search-string');
            showSearchIcon.click(function () {
                $searchPanel.addClass('tau-menu-item-search-open');
                // bug #62842 - increased delay for Chrome 27
                setTimeout(function () {searchInput.focus();}, 200);
            });
            closeSearchIcon.click(function () {
                $searchPanel.removeClass('tau-menu-item-search-open');
            });
        },

        setupSearchTrigger: function($node, configurator) {
            var $searchInput = $node.find('.i-role-search-string');
            configurator.service('search').onClear(function(p) {
                $searchInput.val(p.searchString);
            });

            var $trigger = $node.find('.i-role-search-form');
            $trigger.submit(function(e) {

                e.preventDefault();

                $('body').notifyBar('remove');

                $searchInput.blur();
                $trigger.prop('disabled', true);

                var searchString = _.trim($node.find('.i-role-search-string').val());
                configurator.service('search').params().set('searchString', searchString);

                SearchResolverUtils.resolveSearchToken(searchString, configurator)
                        .fail(function(message) {
                            $searchInput.focus();
                            $trigger.prop('disabled', false);

                            $('body').notifyBar({
                                className: 'tau-system-message-error',
                                html: '<h3>' + message + '</h3>',
                                disableAutoClose: true
                            });

                        })
                        .done(function(r) {
                            var targetUrl = (r.entity === 'search') ? 'search' : (r.entity + '/' + r.id);
                            configurator.service('navigator').to(targetUrl);
                            configurator.isBoardEdition = true;
                            searchApplicationCreator(configurator).done(function(appBus) {
                                appBus.on('destroy', function() {
                                    $trigger.prop('disabled', false);
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
                '<span class="tau-icon-search tau-extension-board-tooltip" data-title="Global search"></span>',
                '<div class="tau-main-search">',
                '    <form action="" class="i-role-search-form">',
                '     <span class="tau-inline-group tau-search">',
                '       <input type="text" class="tau-in-text i-role-search-string" placeholder="Search">',
                '     </span>',
                '     <i class="tau-search-close" title="Close"></i>',
                ' </form>',
                '</div>'
            ].join(''));

            $searchInput = topMenu.addItem({ html: $searchInput, cssClass: 'tau-menu-item-search i-role-mashup-header-search', indexSection:2}).$element;

            this.setupSearchInput($searchInput, configurator);
        }
    });

    busRegistry.getByName('board page container', function(masterBus) {
        new Processor(masterBus);
    });
});