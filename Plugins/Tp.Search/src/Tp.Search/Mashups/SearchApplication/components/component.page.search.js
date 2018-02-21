tau.mashups
    .addDependency('tau/components/component.page.base')
    .addDependency('tp/search/views/view.page.search')
    .addModule('tp/search/components/component.page.search', function (BaseCreator, ViewType) {
        return {
            create: function (componentContext) {
                var componentConfig = {
                    name: 'search page component',
                    turnOffErrorEmiter: true,
                    extensions: [],
                    ViewType: ViewType
                };

                componentContext['queue.bus'] = true;

                return BaseCreator.create(componentConfig, componentContext);
            }
        };
    });
