tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('tau/core/event')
    .addDependency('tau/mashup.manager/services/service.mashup.manager.base')
    .addModule('tau/mashup.manager/services/service.mashup.manager', function ($, _, Event, ServiceMashupManagerBase) {
        var ServiceMashupManager = ServiceMashupManagerBase.extend({
            _orderMashupsInProfile: function(profile){
                if (profile.Settings.MashupNames && profile.Settings.MashupNames.length > 0) {
                    profile.Settings.MashupNames = profile.Settings.MashupNames.sort(function (a, b) {
                        return a.toLocaleLowerCase().localeCompare(b.toLocaleLowerCase());
                    });
                }
                return profile;
            },
            getProfile: function () {
                return this._getOrCreateProfile();
            },
            getMashupByName: function (mashupName) {
                return this._executeProfileRequiredCommand('GetMashupInfo', {Value: mashupName})
                    .fail(_.bind(function (error) {
                        this.status.error('An error occurred when loading the mashup: ' + error);
                    }, this));
            },
            addMashup: function (mashup, failHandler) {
                return this._executeSaveCommand('AddMashup', mashup, failHandler)
                    .done(_.bind(function(){
                        this.fire('mashupAdded', mashup);
                    }, this));
            },
            updateMashup: function (mashup, failHandler) {
                return this._executeSaveCommand('UpdateMashup', mashup, failHandler)
                    .done(_.bind(function(){
                        this.fire('mashupUpdated', mashup);
                    }, this));
            },
            deleteMashup: function (mashupName) {
                return this._executeProfileRequiredCommand('DeleteMashup', {
                        Name:mashupName
                    })
                    .fail(_.bind(function (error) {
                        this.status.error('An error occurred when deleting the mashup: ' + error);
                    }, this))
                    .done(_.bind(function () {
                        this.status.success('Mashup has been deleted successfully');
                        this.fire('mashupDeleted', mashupName);
                    }, this));
            },
            getLibraryRepositories: function () {
                return this._executeProfileRequiredCommand('GetLibraryRepositories', null)
                    .fail(_.bind(function (error) {
                        this.status.error('An error occurred when getting the mashup library repositories: ' + error);
                    }, this));
            },
            refreshLibrary: function () {
                return this._executeProfileRequiredCommand('RefreshLibrary', null)
                    .fail(_.bind(function (error) {
                        this.status.error('An error occurred when refreshing the mashup library: ' + error);
                    }, this));
            },
            installPackage: function (mashupPackage) {
                return this._executeProfileRequiredCommand('InstallPackage', mashupPackage)
                    .fail(_.bind(function (error) {
                        this.status.error('An error occurred when installing the mashup: ' + error);
                    }, this))
                    .done(_.bind(function () {
                        this.status.success('Mashup has been installed successfully');
                        this.fire('packageInstalled', mashupPackage);
                    }, this));
            },
            getPackageDetailed: function (mashupPackage) {
                return this._executeProfileRequiredCommand('GetPackageDetailed', mashupPackage)
                    .fail(_.bind(function (error) {
                        this.status.error('An error occurred when getting the mashup details: ' + error);
                    }, this));
            }
        });

        Event.implementOn(ServiceMashupManager.prototype);

        return ServiceMashupManager;
    });