tau.mashups
    .addModule('BugzillaViewDetails/NewBugInfoTemplate', function() {
        return '<table class="additional-info-table container-table">'+
            '<tr>' +
                '<td class="ui-additionalinfo__label">Bugzilla Id</td>' +
                '<td class="ui-additionalinfo__value">' +
                    '<a href="${Url}" class="tau-linkentity tau-linkentity_type_full" target="_blank">' +
                        '<span class="tau-linkentity__inner i-role-id">${Id}</span>' +
                        '<span class="external-view"></span>' +
                    '</a>' +
                '</td>' +
            '</tr>' +
            '<tr>' +
                '<td class="ui-additionalinfo__label">Reporter</td>' +
                '<td class="ui-additionalinfo__value i-role-reporter">${Reporter}</td>' +
            '</tr>' +
            '<tr>' +
                '<td class="ui-additionalinfo__label">OS</td>' +
                '<td  class="ui-additionalinfo__value i-role-os">${OS}</td>' +
            '</tr>' +
            '<tr>' +
                '<td class="ui-additionalinfo__label">Component</td>' +
                '<td  class="ui-additionalinfo__value i-role-component">${Component}</td>' +
            '</tr>' +
            '<tr>' +
                '<td class="ui-additionalinfo__label">Version</td>' +
                '<td  class="ui-additionalinfo__value i-role-version">${Version}</td>' +
            '</tr>' +
            '<tr>' +
                '<td class="ui-additionalinfo__label">Classification</td>' +
                '<td  class="ui-additionalinfo__value i-role-classification">${Classification}</td>' +
            '</tr>' +
            '<tr>' +
                '<td class="ui-additionalinfo__label">Platform</td>' +
                '<td  class="ui-additionalinfo__value i-role-platform">${Platform}</td>' +
            '</tr>' +
            '{{if CustomFields.length > 0}}' +
            '{{each CustomFields }}' +
                '<tr>' +
                    '<td class="ui-additionalinfo__label">${Description}</td>' +
                    '<td class="ui-additionalinfo__value i-role-customField">{{each(i, value) Values}}{{if i}}, {{/if}}${value}{{/each}}</td>' +
                '</tr>' +
            '{{/each}}' +
            '{{/if}}' +
            '</table>';
    });