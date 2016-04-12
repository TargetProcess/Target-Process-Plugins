tau.mashups
    .addDependency('tau/core/templates-factory')
    .addDependency('tau/mashup.manager/ui/templates/ui.template.mashup.manager.package.requiredVersion')
    .addModule('tau/mashup.manager/ui/templates/ui.template.mashup.manager.library', function(templates) {
        var config = {
            name:   'mashup.manager.library',
            engine: 'jqote2',
            markup: [
                '<div class="mashup-details i-role-mashupDetails">',
                    '<div class="p-10 pt-15">',
                        '<div class="library-header">',
                        '<% if (this.mashupPackages.length !== 0){ %>',
                            '<div class="library-refresh-container">',
                                '<a class="library-refresh i-role-libraryRefresh" href="javascript:void(0);">Check for Updates</a>',
                            '</div>',
                        '<% }  %>',
                            '<h3 class="h3 library-header-caption">Mashup Library</h3>',
                        '</div>',
                        '<% if (this.mashupPackages.length === 0){ %>',
                            '<div>',
                                'There are no mashups in your Library. Try <a class="library-refresh i-role-libraryRefresh" href="javascript:void(0);">Check for Updates</a>',
                             '</div>',
                        '<% }else{ %>',
                            '<ul class="library i-role-library">',
                            '<% _.forEach(this.mashupPackages, function(mashupPackage){ %>',
                                '<li>',
                                    '<div class="mashup-info i-role-mashupInfo">',
                                        '<h3 class="mashup-info__name h3"><a class="i-role-packageDetails" href="#mashup.manager=repositories/<%! mashupPackage.repositoryName %>/packages/<%! mashupPackage.name %>"><%! mashupPackage.name %></a></h3>',

                                        '<div class="mashup-info__description-area">',
                                            '<p class="mashup-info__description"><%! mashupPackage.description %>',
                                            '<% if (mashupPackage.extraDescription) { %>',
                                                '<span class="mashup-info__extra-description" title="<%! mashupPackage.extraDescription %>">&#8230</span>',
                                            '<% }; %>',
                                            '</p>',
                                        '<% if (mashupPackage.compatibleTpVersionMinimum) { %>',
                                            '<%= fn.sub(\'mashup.manager.package.requiredVersion\', mashupPackage.compatibleTpVersionMinimum) %>',
                                        '<% }; %>',
                                        '</div>',
                                        '<button data-repositoryName="<%! mashupPackage.repositoryName %>" data-packageName="<%! mashupPackage.name %>" class="mashup-info__install-trigger tau-btn i-role-installPackage" type="button">Install</button>',
                                    '</div>',
                                '</li>',
                            '<% }); %>',
                            '</ul>',
                        '<% }  %>',
                    '</div>',
                '</div>'
            ]
        };

        return templates.register(config);
    });

