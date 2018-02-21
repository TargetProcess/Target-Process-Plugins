tau.mashups
    .addDependency('Underscore')
    .addModule('tau/mashup.manager/utils/utils.mashup.manager.mashup.data.converter', function(_) {

        function findFileForDisplay(mashup) {
            var jsFiles = _.filter(mashup.Files, function(file) {
                return /.+\.js$/i.test(file.FileName);
            });
            if (jsFiles.length === 1) {
                return jsFiles[0];
            }

            var configFile = _.find(jsFiles, function(file) {
                return /^.+\.config\.js\s*$/i.test(file.FileName);
            });
            if (configFile) {
                return configFile;
            }

            var mashupNameLowerCased = mashup.Name.toLowerCase();
            var mashupNameFile = _.find(jsFiles, function(file) {
                return file.FileName.toLowerCase().indexOf(mashupNameLowerCased) >= 0;
            });
            if (mashupNameFile) {
                return mashupNameFile;
            }

            return jsFiles.length ? jsFiles[0] : null;
        }

        function convertServerUserInfo(userInfo) {
            return userInfo ? {id: userInfo.Id, name: userInfo.Name} : undefined;
        }

        function convertClientUserInfo(userInfo) {
            return userInfo ? {Id: userInfo.id, Name: userInfo.name} : undefined;
        }

        function convertServerDate(date) {
            return date ? new Date(date) : undefined;
        }

        function convertClientDate(date) {
            return (date && date.getTime) ? date.getTime() : undefined;
        }

        return {
            createNewMashupData: function(script, loggedUser) {
                return {
                    //name: undefined,
                    placeholders: 'footerplaceholder',
                    script: script,
                    //fileName: undefined,

                    mashupMetaInfo: {
                        isEnabled: true,
                        //packageName: undefined,
                        creationDate: new Date(),
                        createdBy: loggedUser
                        //lastModificationDate: undefined,
                        //lastModifiedBy: undefined,
                    }
                };
            },

            convertServerMashupDataToClientFormat: function(mashup) {
                var fileForDisplay = findFileForDisplay(mashup) || {Content: ''};
                var mashupMetaInfo = mashup.MashupMetaInfo || {};

                return {
                    name: mashup.Name,
                    placeholders: mashup.Placeholders,
                    script: fileForDisplay.Content,
                    fileName: fileForDisplay.FileName,

                    mashupMetaInfo: {
                        isEnabled: mashupMetaInfo.IsEnabled,
                        packageName: mashupMetaInfo.PackageName,
                        creationDate: convertServerDate(mashupMetaInfo.CreationDate),
                        createdBy: convertServerUserInfo(mashupMetaInfo.CreatedBy),
                        lastModificationDate: convertServerDate(mashupMetaInfo.LastModificationDate),
                        lastModifiedBy: convertServerUserInfo(mashupMetaInfo.LastModifiedBy)
                    }
                };
            },

            convertClientMashupDataToServerFormat: function(mashupInfo, updatedInfo) {
                var mashupMetaInfo = mashupInfo.mashupMetaInfo || {};
                var updatedMetaInfo = updatedInfo.mashupMetaInfo || {};

                var mashup = {
                    Name: updatedInfo.name,
                    Placeholders: updatedInfo.placeholders,

                    Files: [
                        {
                            FileName: updatedInfo.fileName,
                            Content: updatedInfo.script
                        }
                    ],

                    MashupMetaInfo: {
                        IsEnabled: updatedMetaInfo.isEnabled,
                        PackageName: mashupMetaInfo.packageName,
                        CreationDate: convertClientDate(mashupMetaInfo.creationDate),
                        CreatedBy: convertClientUserInfo(mashupMetaInfo.createdBy),
                        LastModificationDate: convertClientDate(updatedMetaInfo.lastModificationDate),
                        LastModifiedBy: convertClientUserInfo(updatedMetaInfo.lastModifiedBy)
                    }
                };

                if (mashupInfo.name) {
                    mashup.OldName = mashupInfo.name;
                }

                return mashup;
            }
        };
    });
