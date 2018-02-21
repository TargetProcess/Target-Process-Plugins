/*eslint indent:0,max-len:0*/
tau.mashups
    .addDependency('tau/core/templates-factory')
    .addDependency('tau/mashup.manager/ui/templates/ui.template.mashup.manager.package.requiredVersion')
    .addModule('tau/mashup.manager/ui/templates/ui.template.mashup.manager.package', function(templates) {
        var config = {
            name: 'mashup.manager.package',
            engine: 'jqote2',
            markup: [
                '<div class="mashup-details i-role-mashupDetails">',
                    '<div class="p-10 pt-15">',
                        '<div class="mashup-info-full">',

                            '<div class="mashup-info-full-header">',
                                '<% if (this.compatibleTpVersionMinimum) { %>',
                                '<%= fn.sub("mashup.manager.package.requiredVersion", this.compatibleTpVersionMinimum) %>',
                                '<% }; %>',
                                '<button data-repositoryName="<%! this.repositoryName %>" data-packageName="<%! this.name %>" class="mashup-info-full__install-trigger tau-btn i-role-installPackage" type="button">Install</button>',
                            '</div>',

                            '<div class="markdown"><%= this.readmeHtml %></div>',

                            '<a class="mashup-info-full-files-link" target="_blank" href="https://github.com/TargetProcess/TP3MashupLibrary/tree/master/<%! this.name %>">View Files on GitHub</a>',
                        '</div>',
                    '</div>',
                '</div>'
            ]
        };

        return templates.register(config);
    });
