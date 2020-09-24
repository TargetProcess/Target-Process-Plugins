tau.mashups
    .addDependency('tau/core/templates-factory')
    .addModule('tau/mashup.manager/ui/templates/ui.template.mashup.manager.list', function(templates) {
        var trashIcon = '<svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">' +
                '<path d="M7 1C6.44775 1 6 1.44775 6 2H3C2.44775 2 2 2.44775 2 3C2 3.55225 2.44775 4 3 4H13C13.5522 4 14 3.55225 14 3C14 2.44775 13.5522 2 13 2H10C10 1.44775 9.55225 1 9 1H7Z" fill="#AE304A"/>\n' +
                '<path d="M13 6H3L3.66528 13.1844C3.7605 14.2131 4.62354 15 5.65674 15H10.3433C11.3765 15 12.2395 14.2131 12.3347 13.1844L13 6Z" fill="#AE304A"/>' +
            '</svg>';

        var config = {
            name:   'mashup.manager.list',
            engine: 'jqote2',
            markup: [
                '<div>',
                    '<div class="mashups-list-item library-item i-role-libraryItem">',
                        '<a href="#mashup.manager=library" class="mashupName mashup-library-link i-role-libraryButton">Library</a>' +
                    '</div>',
                    '<div class="p-10">',
                        '<button onclick="javascript:void(0);return false;" class="tau-btn i-role-addMashup">Add New Mashup</button>',
                    '</div>',
                    '<div class="separator"></div>',
                    '<div class="mashups-list-item">',
                    '<% _.forEach(this, function(mashupName){ %>',
                        '<a href="#mashup.manager=mashups/<%! mashupName %>" data-mashupName="<%! mashupName %>" class="mashupName i-role-mashup">',
                            '<%! mashupName %>',
                            '<div class="delete-mashup tau-btn tau-btn--icon i-role-deleteMashup" onclick="javascript:void(0);return false;">',
                                '<span class="tau-btn__icon">' +
                                    trashIcon +
                                '</span>',
                            '</div>',
                        ' </a>',
                    '<% }); %>',
                    '</div>',
                '</div>'
            ]
        };

        return templates.register(config);
    });
