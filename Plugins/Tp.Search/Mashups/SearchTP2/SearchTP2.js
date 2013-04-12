tau.mashups
        .addDependency('jQuery')
        .addDependency('application.creator')
        .addDependency('tau/core/event')
        .addDependency('tau/core/class')
        .addDependency('tau/core/bus.reg')
        .addDependency('tau/services/service.search')
        .addDependency('tau/services/service.navigator')

        .addDependency('tau/service.container')
        .addDependency('libs/jquery/jquery.ui.tauBubble')
        .addDependency('Searcher/SearchApplication')
        .addDependency('tp/plugins/profileRepository')
        .addCSS('../../tau/css/tau.widgets.css')
        .addCSS('../../tau/css/ui.attachmentsPreview.css')
        .addCSS('../../tau/css.board/tau.board.search.css')
        .addModule('Searcher/SearchTP2', function ($, AppCreator, Event, Class, busRegistry, ServiceSearch, ServiceNavigator, serviceContainerClass, buble, searchApp, profileRepository) {

    var APP_ID = 'sp';
    var Processor = Class.extend({

        bindSearchSettingsTemplate: function(configurator, data) {
            return configurator
                .getTemplateFactory()
                .register({
                    name:'search-input-settings',
                    engine: 'jqote2',
                    markup: [
                        '<% var p = this.params; %>',
                        '<div class="selectType">',
                        '<select class="i-role-entity-types-filter">',
                        '<option value=""<% if (!p.entityTypeId) { %> selected<% } %>>Any entities</option>',
                        '<% for(var i = 0; i < this.entityTypes.length; i++) { %>',
                        '<% var et = this.entityTypes[i]; %>',
                        '<option value="<%= et.id %>"<% if (et.id == p.entityTypeId) { %> selected<% } %>>',
                        '<%= et.term %>',
                        '</option>',
                        '<% } %>',
                        '</select>',
                        '</div>',

                        '<div class="selectType">',
                        '<select class="i-role-entity-states-filter">',
                        '    <option value=""<% if (!p.entityStateIds) { %> selected<% } %>>Any states</option>',
                        '    <% for(var s = 0; s < this.entityStates.length; s++) { %>',
                        '    <% var es = this.entityStates[s]; %>',
                        '    <option value="<%= es.id %>"<% if (es.id == p.entityStateIds) { %> selected<% } %>>',
                        '        <%= es.name %>',
                        '    </option>',
                        '    <% } %>',
                        '</select>',
                        '</div>',

                        '<div class="allProject">',
                        '<input type="checkbox" id="i-role-all-projects-flag" class="i-role-all-projects-flag"<% if (this.params.isAllProjects) { %> checked="checked"<% } %>>',
                        '<label for="i-role-all-projects-flag">All projects &amp; teams</label>',
                        '</div>'
                    ]
                })
                .get()
                .bind({}, data);
        },

        clearSearchSettings: function(searchPopup) {
            searchPopup.find('.i-role-entity-types-filter').remove();
            searchPopup.find('.i-role-entity-states-filter').remove();
            searchPopup.find('.i-role-all-projects-flag').remove();
            searchPopup.off('change');
        },

        setupSearchSettings: function(configurator) {
            var self = this;
            var searchServiceInstance = configurator.service('search');
            var searchPopup = $('.mainSearchPopup');
            searchPopup.find('.selectState').unbind().hide();
            searchPopup.find('.selectType').unbind().hide();
            searchPopup.find('.allProject').unbind().hide();
            searchPopup.find('.searchBtn').unbind().hide();
            searchServiceInstance.getSearchDomain().done(function (d) {
                self.clearSearchSettings(searchPopup);
                var $filters = self.bindSearchSettingsTemplate(configurator, d);
                searchPopup.append($filters);
                searchPopup.on('change', '.i-role-entity-types-filter', function (e) {
                    var $target = $(e.target);
                    var v = $target.val();

                    var $select = searchPopup.find('.i-role-entity-states-filter');
                    var currVal = $select.val();
                    var name = $select.find('option[value="' + currVal + '"]').text();
                    name = _.trim(name);

                    var params = searchServiceInstance.params();
                    params.set('entityTypeId', v ? v : null);
                    params.set('entityStateIds', null);

                    searchServiceInstance.getSearchDomain(params.get()).done(function(r) {
                        var $select = searchPopup.find('.i-role-entity-states-filter');
                        $select.empty();
                        $select.append('<option value="">Any states</option>');

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

                searchPopup.on('change', '.i-role-entity-states-filter', function (e) {
                    var $target = $(e.target);
                    var v = $target.val();
                    searchServiceInstance.params().set('entityStateIds', v);
                });

                searchPopup.on('change', '.i-role-all-projects-flag', function (e) {
                    var $target = $(e.target);
                    searchServiceInstance.params().set('isAllProjects', $target.prop('checked'));
                });
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

        setupSearchTrigger: function(configurator) {
            var me = this;

            configurator.service('search').onClear(function(p) {
                $('#topSearch input[type=text], .search input[type=text]').val(p.searchString);
            });
            $('#search-block > div').removeAttr('onkeypress');

            var subscribe = function($trigger, $searchInput)
            {
                $trigger.removeAttr('onclick').unbind().die();
                $searchInput.removeAttr('onkeydown').unbind().die();
                $trigger.click(function(e) {
                    e.preventDefault();
                    if($searchInput.val() === ''){
                        return false;
                    }
                    $('body').notifyBar('remove');

                    $searchInput.blur();
                    $trigger.prop('disabled', true);

                    var searchString = _.trim($searchInput.val());
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
                                searchApp(configurator).done(function(appBus) {
                                    appBus.on('destroy', function(e) {
                                        e.removeListener();
                                        $trigger.prop('disabled', false);
                                        configurator.service('search').params().clear();
                                        me.setupSearchSettings(configurator);
                                    });
                                });
                            });
                });
            };
            subscribe($('#topSearch input[type=image]'), $('#topSearch input[type=text]'));
            subscribe($('.mainSearchPopup .search input[type=image]'), $('.mainSearchPopup .search input[type=text]'));
            $('.mainSearchPopup .search input[type=text]').on('keypress', function(e){
                if (e.which == 13) {
                    e.preventDefault();
                    $('#topSearch input[type=text]').val($(this).val());
                    $('#topSearch input[type=image]').click();
                    return;
                }
                $('#topSearch input[type=text]').val($(this).val());
            });

            $('#topSearch input[type=text]').on('keypress', function(e){
                if (e.which == 13) {
                    e.preventDefault();
                    $('#topSearch input[type=image]').click();
                }
            });
        },

        setupSearchInput: function(parentConfigurator) {

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

            this.setupSearchSettings(searchViewConfigurator);
            this.setupSearchTrigger(searchViewConfigurator);
        },

        render: function(configurator) {
            this.setupSearchInput(configurator);
        }
    });
    return new Processor();
});
