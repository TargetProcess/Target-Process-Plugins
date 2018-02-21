tau.mashups
    .addDependency('tau/components/component.creator')
    .addDependency('tp/search/extensions/extension.search.filter')
    .addDependency('tau/components/extensions/error/extension.errorBar')
    .addDependency('tp/search/templates/template.search.filter')
    .addModule('tp/search/components/component.search.filter',
        function (ComponentCreator, SearchFilterExtension, ErrorBar, template) {
            return {
                create: function (config) {
                    config['queue.bus'] = true;

                    var creatorConfig = {
                        template: template,
                        'queue.bus': true,
                        extensions: [
                            SearchFilterExtension,
                            ErrorBar
                        ]
                    };

                    return ComponentCreator.create(creatorConfig, config);
                }
            };
        });
