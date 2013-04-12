require(['all.components'], function () {
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
            .addDependency('Searcher/SearchApplication')
            .addMashup(function (_, $, AppCreator, Event, Class, busRegistry, ServiceSearch, ServiceNavigator, ProgressIndicator, searchApplicationCreator) {

        var APP_ID = 'sp';

        var Processor = Class.extend({
            init: function(bus) {
                this.bus = bus;
                Event.subscribeOn(this);
            },

            bindSearchSettingsTemplate: function(configurator, data) {
                return configurator
                        .getTemplateFactory()
                        .register({
                                      name:'search-input-settings',
                                      engine: 'jqote2',
                                      markup: [
                                          '<% var p = this.params; %>',
                                          '<h3>Advanced search</h3>',
                                          '<div class="tau-line">',
                                          '<select class="tau-select i-role-entity-types-filter">',
                                          '    <option value=""<% if (!p.entityTypeId) { %> selected<% } %>>Any entities</option>',
                                          '    <% for(var i = 0; i < this.entityTypes.length; i++) { %>',
                                          '    <% var et = this.entityTypes[i]; %>',
                                          '    <option value="<%= et.id %>"<% if (et.id == p.entityTypeId) { %> selected<% } %>>',
                                          '        <%= et.term %>',
                                          '    </option>',
                                          '    <% } %>',
                                          '</select>',
                                          '</div>',
                                          '<div class="tau-line">',
                                          '<select class="tau-select i-role-entity-states-filter">',
                                          '    <option value=""<% if (!p.entityStateIds) { %> selected<% } %>>Any states</option>',
                                          '    <% for(var s = 0; s < this.entityStates.length; s++) { %>',
                                          '    <% var es = this.entityStates[s]; %>',
                                          '    <option value="<%= es.id %>"<% if (es.id == p.entityStateIds) { %> selected<% } %>>',
                                          '        <%= es.name %>',
                                          '    </option>',
                                          '    <% } %>',
                                          '</select>',
                                          '</div>',
                                          '<div class="tau-line">',
                                          '<label class="tau-checkbox">',
                                          '<input type="checkbox" class="i-role-all-projects-flag"<% if (this.params.isAllProjects) { %> checked="checked"<% } %>>',
                                          '<i></i><span>All projects &amp; teams</span>',
                                          '</label>',
                                          '</div>'
                                      ]
                                  })
                        .get()
                        .bind({}, data);
            },

            setupSearchSettings: function($node, configurator) {
                var self = this;
                var searchServiceInstance = configurator.service('search');

                var onPositionConfig = function (config) {
                    config.at = "right bottom";
                    config.my = "center top";
                    config.offset = '-95px 6px';
                };

                var onArrowPositionConfig = function (config) {
                    config.offset = '0px -7px';
                };

                var targetSelector = '.i-role-search-settings';
                var $nodeSettings = $node.find(targetSelector);

                $nodeSettings.tauBubble({
                    content:$('<span>Loading...</span>'),
                    className: 'tau-bubble-search',

                    zIndex: 999,

                    onPositionConfig:onPositionConfig,
                    onArrowPositionConfig: onArrowPositionConfig,

                    onShow:function (content) {

                        var params = searchServiceInstance.params().get();
                        searchServiceInstance.getSearchDomain(params).done(function(d) {
                            var $filters = self.bindSearchSettingsTemplate(configurator, d);
                            content.find('[role=content]').html($filters);
                        });

                        content.on('change', '.i-role-entity-types-filter', function (e) {
                            var $target = $(e.target);
                            var v = $target.val();

                            var $select = content.find('.i-role-entity-states-filter');
                            var currVal = $select.val();
                            var name = $select.find('option[value="' + currVal + '"]').text();
                            name = _.trim(name);

                            var params = searchServiceInstance.params();
                            params.set('entityTypeId', v ? v : null);
                            params.set('entityStateIds', null);

                            searchServiceInstance.getSearchDomain(params.get()).done(function(r) {
                                $select
                                    .empty()
                                    .append('<option value="">Any states</option>');

                                _(r.entityStates).each(function(s) {
                                    var selectedAttr = '';
                                    if (name === s.name) {
                                        selectedAttr = ' selected';
                                        params.set('entityStateIds', s.id);
                                    }

                                    $select.append('<option value="' + s.id + '"' + selectedAttr + '>' + s.name + '</option>');
                                });
                            });
                        });

                        content.on('change', '.i-role-entity-states-filter', function (e) {
                            var $target = $(e.target);
                            var v = $target.val();
                            searchServiceInstance.params().set('entityStateIds', v);
                        });

                        content.on('change', '.i-role-all-projects-flag', function (e) {
                            var $target = $(e.target);
                            searchServiceInstance.params().set('isAllProjects', $target.prop('checked'));
                        });
                    },

                    onHide:function (content) {
                        content.off('change');
                    }
                });
            },

            resolveSearchToken: function(searchToken, configurator) {
                var $tokenResolver = $.Deferred();

                if (/^\d+$/.test(searchToken)) {
                    configurator
                            .getStore()
                            .get('general', { id: searchToken, fields: ['id', {entityType:['id', 'name']}] })
                            .done({
                                      success: function(r) {
                                          var d = r[0].data;

                                          var store = configurator.getStore();
                                          if ('project' === d.entityType.name.toLowerCase()) {
                                              store.get('project', { id: d.id, fields: ['id', 'name'] })
                                                  .done({
                                                      success: function(r) {
                                                          $tokenResolver.resolve({ id: d.id, entity: d.entityType.name });
                                                      },
                                                      failure: function(r) {
                                                          $tokenResolver.resolve({ id: null, entity: 'search' });
                                                      }
                                                  });
                                          }
                                          else {
                                              $tokenResolver.resolve({ id: d.id, entity: d.entityType.name });
                                          }
                                      },
                                      failure: function() {
                                          $tokenResolver.resolve({ id: null, entity: 'search' });
                                      }
                                  });
                }
                else if (searchToken.length > 2) {
                    $tokenResolver.resolve({ id: null, entity: 'search' });
                }
                else {
                    $tokenResolver.reject('Enter more than 2 chars');
                }

                return $tokenResolver;
            },

            setupSearchTrigger: function($node, configurator) {
                var me = this;

                var $searchInput = $node.find('.i-role-search-string');
                configurator.service('search').onClear(function(p) {
                    $searchInput.val(p.searchString);
                });

                var $trigger = $node.find('.i-role-search-trigger');
                $trigger.click(function(e) {

                    e.preventDefault();

                    $('body').notifyBar('remove');

                    $searchInput.blur();
                    $trigger.prop('disabled', true);

                    var searchString = _.trim($node.find('.i-role-search-string').val());
                    configurator.service('search').params().set('searchString', searchString);

                    me.resolveSearchToken(searchString, configurator)
                            .fail(function(message) {
                                $searchInput.focus();
                                $trigger.prop('disabled', false);

                                $('body').notifyBar({
                                    className: 'error',
                                    html: message,
                                    disableAutoClose: true
                                });

                            })
                            .done(function(r) {
                                var targetUrl = (r.entity === 'search') ? 'search' : (r.entity + '/' + r.id);
                                configurator.service('navigator').to(targetUrl);
                                searchApplicationCreator(configurator).done(function(appBus) {
                                    appBus.on('destroy', function(e) {
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
                    isAllProjects: false
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

                this.setupSearchSettings($node, searchViewConfigurator);
                this.setupSearchTrigger($node, searchViewConfigurator);
            },

            'bus configurator.ready + view.dom.ready': function(e, configurator, render) {
                var $header = render.element.find('.tau-app-header');
                var $searchInput = $([
                    '<form action="">',
                    '<div class="tau-main-search">',
                    '<span class="tau-inline-group tau-search">',
                    '<input type="text" class="tau-in-text i-role-search-string" placeholder="Search">',
                    '<span class="tau-search-settings i-role-search-settings"></span>',
                    '<button class="tau-btn tau-search i-role-search-trigger"></button>',
                    '</span>',
                    '</div>',
                    '</form>'
                ].join(''));

                var $mainMenu = $header.find('.tau-main-menu');
                $mainMenu.before($searchInput);

                this.setupSearchInput($searchInput, configurator);
            }
        });

        busRegistry.getByName('board page container', function(masterBus) {
            var p = new Processor(masterBus);
        });
    });
});
