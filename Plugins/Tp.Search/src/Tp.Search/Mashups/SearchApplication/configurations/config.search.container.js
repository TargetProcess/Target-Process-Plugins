tau.mashups
    .addDependency('tau/core/class')
    .addDependency('tp/search/extensions/extension.search.filter.hider')
    .addModule('tp/search/configurations/config.search.container', function (Class, SearchFilterHiderExtension) {
        return Class.extend({
            getConfig: function () {
                return {
                    children: [
                        {
                            type: 'container',
                            name: 'search results container',
                            layout: 'selectable',
                            template: {
                                name: 'search-results-container',
                                markup: '<div class="tau-container i-role-container-target"></div>'
                            },
                            children: [
                                {
                                    type: 'search.filter',
                                    namespace: 'tp/search',
                                    selector: '.i-role-container-target'
                                },
                                {
                                    type: 'search',
                                    namespace: 'tp/search',
                                    selector: '.i-role-container-target'
                                }
                            ],
                            extensions: [SearchFilterHiderExtension]
                        }
                    ]
                };
            }
        });
    });
