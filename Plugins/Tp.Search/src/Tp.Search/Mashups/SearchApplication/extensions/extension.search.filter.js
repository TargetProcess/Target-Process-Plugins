tau.mashups
    .addDependency('Underscore')
    .addDependency('jQuery')
    .addDependency('tp/search/models/model.search.filter.resolver')
    .addDependency('tau/core/extension.base')
    .addDependency('tp/search/templates/template.search.filter.entity.states')
    .addDependency('jquery.tauResetableInput')
    .addModule('tp/search/extensions/extension.search.filter',
        function (_, $, SearchResolver, ExtensionBase, entityStateOptions) {

            var formFieldsSelector = [
                '.i-role-search-string',
                '.i-role-entity-type-filter',
                '.i-role-entity-state-filter',
                '.i-role-all-projects-filter'
            ].join(',');

            return ExtensionBase.extend({
                'bus afterInit': function (e, afterInitArg) {
                    var configurator = afterInitArg.config.context.configurator;
                    var searchService = configurator.service('search');
                    var params = searchService.params().get();

                    searchService
                        .getSearchDomain(params)
                        .done(function (d) {
                            d.isBoardEdition = configurator.isBoardEdition;
                            this.fire('dataBind', d);
                        }.bind(this));
                },

                'bus afterRender': function (e, afterRenderArg) {
                    var configurator = afterRenderArg.view.config.context.configurator;
                    var $node = afterRenderArg.element;
                    var searchService = configurator.service('search');
                    this._searchServiceCallback = this._toggleSearchForm.bind(this, $node);

                    $node.on('submit', 'form', function (e) {
                        e.preventDefault();
                        this._triggerNavigate($node, configurator);
                    }.bind(this));
                    $node.find('.i-role-search-string').tauResetableInput();

                    $node.on('change', '.i-role-entity-type-filter', function (e) {
                        e.preventDefault();
                        searchService.params().set('entityTypeId', $(e.target).val());
                        this._toggleSearchForm($node, false);

                        this._onSearchParametersChangedAsync(searchService, $node)
                            .done(function () {
                                return this._triggerSearch($node, configurator);
                            }.bind(this));
                    }.bind(this));

                    $node.on('change', '.i-role-entity-state-filter,.i-role-all-projects-filter', function (e) {
                        e.preventDefault();
                        var params = searchService.params();
                        var allCheckboxValue = $node.find('.i-role-all-projects-filter').prop('checked');
                        params.set('isAllProjects', allCheckboxValue);
                        params.set('isAllTeams', !configurator.isBoardEdition || allCheckboxValue);
                        this._toggleSearchForm($node, false);

                        this._onSearchParametersChangedAsync(searchService, $node)
                            .done(function () {
                                return this._triggerSearch($node, configurator);
                            }.bind(this));
                    }.bind(this));
                },

                _toggleSearchForm: function ($node, shouldEnable) {
                    var $fields = $node.find(formFieldsSelector);
                    if (shouldEnable === false) {
                        $fields.attr('disabled', 'disabled');
                    } else {
                        $fields.removeAttr('disabled');
                    }
                },

                _triggerSearch: function ($node, configurator) {
                    this.fire('clearError');
                    var searchString = _.trim($node.find('.i-role-search-string').val());

                    SearchResolver
                        .resolveSearchToken(searchString, configurator)
                        .fail(function (message) {
                            this._toggleSearchForm($node, true);
                            this.fire('error', {message: message});
                        }.bind(this))
                        .done(function (d) {
                            if (d.entity === 'search') {
                                var searchService = configurator.service('search');

                                searchService.offSearch(this._searchServiceCallback);
                                searchService.onSearch(this._searchServiceCallback);

                                var allCheckboxValue = $node.find('.i-role-all-projects-filter').prop('checked');
                                var entityStateSelect = $node.find('.i-role-entity-state-filter');
                                var entityStateIds = entityStateSelect.val();

                                if (allCheckboxValue === true && entityStateIds) {
                                    entityStateIds = entityStateSelect
                                        .find('option[value="' + entityStateIds + '"]')
                                        .attr('available');
                                }

                                searchService
                                    .params()
                                    .set('searchString', searchString)
                                    .set('entityTypeId', $node.find('.i-role-entity-type-filter').val() || null)
                                    .set('entityStateIds', entityStateIds)
                                    .set('isAllProjects', allCheckboxValue)
                                    .set('isAllTeams', !configurator.isBoardEdition || allCheckboxValue)
                                    .submit();

                                configurator.service('navigator').to('query/' + encodeURIComponent(searchString));
                            } else {
                                configurator.service('navigator').to(d.entity + '/' + d.id);
                            }
                        }.bind(this));
                },

                _triggerNavigate: function ($node, configurator) {
                    this.fire('clearError');
                    var searchString = _.trim($node.find('.i-role-search-string').val());

                    SearchResolver
                        .resolveSearchToken(searchString, configurator)
                        .fail(function (message) {
                            this._toggleSearchForm($node, true);
                            this.fire('error', {message: message});
                        }.bind(this))
                        .done(function (d) {
                            if (d.entity === 'search') {
                                configurator.service('navigator').to('query/' + encodeURIComponent(searchString));
                            } else {
                                configurator.service('navigator').to(d.entity + '/' + d.id);
                            }
                        });
                },

                _onSearchParametersChangedAsync: function (searchService, $node) {
                    var params = searchService.params();
                    params.set('entityStateIds', null);

                    return searchService
                        .getSearchDomain(params.get())
                        .done(function (r) {
                            var $select = $node.find('.i-role-entity-state-filter');
                            var stateIds = $select.val();
                            var stateOptionText = $select.find('option[value="' + stateIds + '"]').text();
                            var selectedStateName = _.trim(stateOptionText);

                            var $options = entityStateOptions.render({
                                entityStates: r.entityStates,
                                selectedStateName: selectedStateName,
                                selectedStateIds: null
                            });

                            $select.empty().append($options);
                        });
                }
            });
        });
