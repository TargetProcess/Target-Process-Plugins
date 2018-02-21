tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('tau/core/class')
    .addDependency('tp/search/services/service.search.command')
    .addDependency('tp/search/services/service.search.command.text.contextBuilder')
    .addModule('tp/search/services/service.search.command.text',
        function ($, _, Class, SearchCommand, SearchContextBuilder) {
            var ServiceSearchTextCommand = SearchCommand.extend({
                searchCommandTemplate: '{AppPath}/api/v1/Plugins.asmx/Search/Profiles/Now%20running/Commands/Search',
                init: function (configurator, params, $resultDeferred) {
                    this._super(configurator, params, $resultDeferred);
                    this.searchContextBuilder = new SearchContextBuilder(this.configurator, params);
                },
                execute: function () {
                    this._done(this.searchContextBuilder.getContextPromise()
                        .then(_.bind(this._searchTextPromise, this))
                        .then(_.bind(this._fetchDetailsByIDs, this)));
                },
                _searchTextPromise: function (searchContext) {
                    var plgn = this.searchCommandTemplate.replace(/{AppPath}/g, this.configurator.getApplicationPath());
                    var user = this.configurator.getLoggedUser();

                    var post = JSON.stringify({
                        Command: 'Search',
                        Plugin: 'Search',

                        LoggedUserId: user != null ? user.id : null,

                        TeamIds: _.without(searchContext.selectedTeamIdsAvailable, 'null'),
                        IncludeNoTeam: searchContext.includeNoTeam,

                        ProjectIds: _.without(searchContext.selectedProjectIdsAvailable, 'null'),

                        TeamProjectRelations: _(searchContext.teamProjectRelations).map(function (v, k) {
                            return {
                                TeamId: parseInt(k, 10),
                                ProjectIds: v
                            };
                        }),

                        // SearchString: params.searchString,
                        Query: this.params.searchString,
                        EntityTypeId: this.params.entityTypeId,
                        EntityStateIds: _.isArray(this.params.entityStateIds) ?
                            this.params.entityStateIds :
                            _((this.params.entityStateIds || '').split(',')).chain().without('').map(function (n) {
                                return parseInt(n, 10);
                            }).value(),

                        // PageSize: params.pageSize,
                        // Page: params.pageNo,
                        Page: {
                            Number: this.params.pageNo,
                            Size: this.params.pageSize
                        }
                    });

                    return $.ajax({
                        url: plgn,
                        type: 'POST',
                        data: post,
                        dataType: 'json'
                    });
                }
            });

            ServiceSearchTextCommand.isSuitableCommand = function () {
                return true;
            };

            ServiceSearchTextCommand.allowsToBeFiltered = true;

            return ServiceSearchTextCommand;
        });
