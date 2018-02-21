tau.mashups
    .addDependency('tau/core/templates-factory')
    .addDependency('tau/ui/templates/search-element.html')
    .addDependency('tp/search/templates/template.search.filter.entity.states')
    .addModule('tp/search/templates/template.search.filter', function(templatesFactory) {
        var config = {
            name:   'search.filter',
            engine: 'jqote2',
            markup: [
                '<div class="tau-search-result__header">',
                '    <form action="">',
                '        <div class="tau-search-result__wrap">',
                '            <%= fn.sub("search-element", {searchValue: this.params.searchString, inputClass: "i-role-search-string"}) %>',
                '            <button class="tau-search__hidden-button"></button>',
                '        </div>',
                '        <div class="tau-search-result__settings">',
                '            <span class="tau-search-result__settings__title tau-search-result__settings__item"><%= fn.intl.formatMessage("Search in") %></span>',
                '            <select class="tau-select tau-search-result__settings__item i-role-entity-type-filter" style="margin-left: 8px;">',
                '                <option value="" <%= this.params.entityTypeId ? "" : "selected" %>><%= fn.intl.formatMessage("Any entities") %></option>',
                '                <% for (var i = 0; i < this.entityTypes.length; i++) { %>',
                '                    <% var et = this.entityTypes[i]; %>',
                '                    <option value="<%= et.id %>" <%= (et.id == this.params.entityTypeId) ? "selected" : "" %>><%! et.term %></option>',
                '                <% } %>',
                '            </select>',
                '            <select class="tau-select tau-search-result__settings__item i-role-entity-state-filter" style="margin-left: 8px;">',
                '                <%= fn.sub("search.filter.entity.states", {entityStates: this.entityStates, selectedStateIds: this.params.entityStateIds}) %>',
                '            </select>',
                '            <label class="tau-checkbox tau-search-result__settings__item" style="margin-left: 8px;">',
                '                <input type="checkbox" class="i-role-all-projects-filter" <%= this.params.isAllProjects ? "checked" : "" %>>',
                '                <i class="tau-checkbox__icon"></i>',
                '                <span style="margin-left: 0.3em;">',
                '                    <% if (this.isBoardEdition) { %>',
                '                        <%= fn.intl.formatMessage("All projects & teams") %>',
                '                    <% } else { %>',
                '                        <%= fn.intl.formatMessage("All projects") %>',
                '                    <% } %>',
                '                </span>',
                '            </label>',
                '        </div>',
                '    </form>',
                '</div>'
            ]
        };

        return templatesFactory.register(config);
    });
