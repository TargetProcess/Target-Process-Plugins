tau.mashups
    .addDependency('tau/core/templates-factory')
    .addModule('tp/search/templates/template.search.filter.entity.states', function(templatesFactory) {
        var config = {
            name:   'search.filter.entity.states',
            engine: 'jqote2',
            markup: [
                '<option value="" <%= this.selectedStateIds ? "" : "selected" %>>',
                '    <%= fn.intl.formatMessage("Any states") %>',
                '</option>',
                '<% for (var i = 0; i < this.entityStates.length; i++) { %>',
                '    <% var es = this.entityStates[i]; %>',
                '    <% var selected = (es.id === this.selectedStateIds || es.name === this.selectedStateName) ? "selected" : ""; %>',
                '    <option value="<%= es.id %>" available="<%= es.availableId %>" <%= selected %>>',
                '        <%! es.name %>',
                '    </option>',
                '<% } %>'
            ]
        };

        return templatesFactory.register(config);
    });
