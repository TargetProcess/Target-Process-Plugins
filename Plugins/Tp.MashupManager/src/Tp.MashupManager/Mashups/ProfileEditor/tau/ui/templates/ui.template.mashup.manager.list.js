tau.mashups
    .addDependency('tau/core/templates-factory')
    .addModule('tau/mashup.manager/ui/templates/ui.template.mashup.manager.list', function(templates) {
        var config = {
            name:   'mashup.manager.list',
            engine: 'jqote2',
            markup: [
                '<div>',
                    '<div class="mashups-list-item library-item i-role-libraryItem">',
                        '<a href="#mashup.manager=library" class="mashupName mashup-library-link i-role-libraryButton">Library</a>' +
                    '</div>',
                    '<div class="p-10">',
                        '<button onclick="javascript:void(0);return false;" class="button big i-role-addMashup">Add New Mashup</button>',
                    '</div>',
                    '<div class="separator"></div>',
                    '<div class="mashups-list-item">',
                    '<% _.forEach(this, function(mashupName){ %>',
                        '<a href="#mashup.manager=mashups/<%! mashupName %>" data-mashupName="<%! mashupName %>" class="mashupName i-role-mashup">',
                            '<%! mashupName %>',
                            '<button class="delete-mashup button danger i-role-deleteMashup" onclick="javascript:void(0);return false;">',
                            '<span class="delete-icon"></span></button>',
                        ' </a>',
                    '<% }); %>',
                    '</div>',
                '</div>'
            ]
        };

        return templates.register(config);
    });
