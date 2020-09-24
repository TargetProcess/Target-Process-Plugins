tau.mashups
    .addDependency('tau/core/event')
    .addDependency('tau/mashup.manager/services/service.mashup.manager.base')
    .addDependency('tau/mashup.manager/services/service.mashup.manager.messages')
    .addDependency('tracking/tauspy')
    .addModule('tau/mashup.manager/services/service.mashup.manager', function(Event, ServiceMashupManagerBase,
        messages) {

        var ServiceMashupManager = ServiceMashupManagerBase.extend({
            getProfile: function() {
                return this._getOrCreateProfile();
            },
            getMashupByName: function(mashupName) {
                return this._executeProfileRequiredCommand('GetMashupInfo', {Value: mashupName})
                    .fail(function(error) {
                        this.status.error('An error occurred when loading the mashup: ' + error);
                    }.bind(this));
            },
            addMashup: function(mashup, failHandler) {
                taus.track({
                    action: 'add mashup',
                    name: mashup.Name
                });

                return this._executeSaveCommand('AddMashup', mashup, failHandler)
                    .done(function() {
                        this.fire('mashupAdded', mashup);
                    }.bind(this));
            },
            updateMashup: function(mashup, failHandler) {
                taus.track({
                    action: 'update mashup',
                    name: mashup.Name
                });

                return this._executeSaveCommand('UpdateMashup', mashup, failHandler)
                    .done(function() {
                        this.fire('mashupUpdated', mashup);
                    }.bind(this));
            },
            deleteMashup: function(mashupName) {
                taus.track({
                    action: 'delete mashup',
                    name: mashupName
                });

                return this._executeProfileRequiredCommand('DeleteMashup', {Name: mashupName})
                    .fail(function(error) {
                        this.status.error('An error occurred when deleting the mashup: ' + error);
                    }.bind(this))
                    .done(function() {
                        this.status.success(messages.DELETED_MESSAGE, messages.TIMEOUT);
                        this.fire('mashupDeleted', mashupName);
                    }.bind(this));
            },
            getLibraryRepositories: function() {
                return this._executeProfileRequiredCommand('GetLibraryRepositories', null)
                    .fail(function(error) {
                        this.status.error('An error occurred when getting the mashup library repositories: ' + error);
                    }.bind(this));
            },
            refreshLibrary: function() {
                return this._executeProfileRequiredCommand('RefreshLibrary', null)
                    .fail(function(error) {
                        this.status.error('An error occurred when refreshing the mashup library: ' + error);
                    }.bind(this));
            },
            installPackage: function(mashupPackage) {
                taus.track({
                    action: 'install mashup',
                    name: mashupPackage.PackageName
                });

                return this._executeProfileRequiredCommand('InstallPackage', mashupPackage)
                    .fail(function(error) {
                        this.status.error('An error occurred when installing the mashup: ' + error);
                    }.bind(this))
                    .done(function() {
                        this.status.success(messages.INSTALLED_MESSAGE, messages.TIMEOUT);
                        this.fire('packageInstalled', mashupPackage);
                    }.bind(this));
            },
            getPackageDetailed: function(mashupPackage) {
                return this._executeProfileRequiredCommand('GetPackageDetailed', mashupPackage)
                    .fail(function(error) {
                        this.status.error('An error occurred when getting the mashup details: ' + error);
                    }.bind(this));
            }
        });

        Event.implementOn(ServiceMashupManager.prototype);

        return ServiceMashupManager;
    });
