/*eslint max-len: [1, 210, 4]*/
/*jshint maxlen: 210*/
tau.mashups
    .addDependency('tau/core/templates-factory')
    .addDependency('tau/utils/utils.date')
    .addDependency('tau/configurator')
    .addModule('tau/mashup.manager/ui/templates/ui.template.mashup.manager.mashup.metainfo',
        function(templates, dateUtils, configurator) {
            var config = {
                name: 'mashup.manager.mashup.metainfo',
                engine: 'jqote2',
                customFunctions: {
                    printActionInfo: function(date, user) {
                        var userInfo;
                        if (user) {
                            var urlBuilder = configurator.getUrlBuilder();
                            var url = urlBuilder.getApplicationPath() + '/' +
                                urlBuilder.getBoardPageRelativePath() + urlBuilder.getRelativeViewUrl(user.id, 'user');
                            userInfo = 'by <a target="_blank" href="' + url + '">#' + user.id + ' ' + user.name + '</a>';
                        }

                        return [
                            dateUtils.format.date['short'](date),
                            userInfo
                        ].filter(function(str) {
                            return !!str;
                        }).join(' ');
                    },
                    printCreationInfo: function(data) {
                        var mashupMetaInfo = data.mashupMetaInfo;
                        return this.printActionInfo(mashupMetaInfo.creationDate, mashupMetaInfo.createdBy);
                    },
                    printLastModificationInfo: function(data) {
                        var mashupMetaInfo = data.mashupMetaInfo;
                        return this.printActionInfo(mashupMetaInfo.lastModificationDate, mashupMetaInfo.lastModifiedBy);
                    }
                },
                markup: [
                    '<% var mashupMetaInfo = this.mashupMetaInfo; %>',
                    '<span class="small">',

                    '<% if (mashupMetaInfo.packageName || mashupMetaInfo.creationDate || mashupMetaInfo.createdBy) { %>',
                    '<% if (mashupMetaInfo.packageName) { %>',
                    '<a target="_blank" href="https://github.com/TargetProcess/TP3MashupLibrary/tree/master/<%! mashupMetaInfo.packageName %>">Installed from library</a> ',
                    '<%= fn.printCreationInfo(this) %>',
                    '<% } else if (mashupMetaInfo.isDraft) { %>',
                    'New unsaved mashup',
                    '<% } else { %>',
                    'Created <%= fn.printCreationInfo(this) %>',
                    '<% } %>',
                    '<br />',
                    '<% } %>',

                    '<% if (mashupMetaInfo.lastModificationDate || mashupMetaInfo.lastModifiedBy) { %>',
                    'Last modified <%= fn.printLastModificationInfo(this) %>',
                    '<% } %>',

                    '</span>'
                ]
            };

            return templates.register(config);
        });
