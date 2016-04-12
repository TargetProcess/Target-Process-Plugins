tau.mashups
    .addDependency('tau/components/component.page.base')
    .addDependency('tau/mashup.manager/views/view.mashup.manager')
    .addModule('tau/mashup.manager/components/component.mashup.manager', function (ComponentPageBase, ViewType) {
        return {
            create: function(componentContext) {
                var componentConfig = {
                    name: "mashup manager page component",
                    extensions: [],
                    ViewType: ViewType
                };
                return ComponentPageBase.create(componentConfig, componentContext);
            }
        };
    });
