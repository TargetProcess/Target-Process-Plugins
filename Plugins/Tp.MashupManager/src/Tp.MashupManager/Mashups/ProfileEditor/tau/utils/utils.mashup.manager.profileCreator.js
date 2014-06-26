define([
    'Underscore'
    , 'tp/plugins/profileRepository'
    , 'tp/bus'
], function (_, tpProfileRepository, tpBus) {

    function ProfileCreator() {
        tpProfileRepository.getCurrentProfileName = _.bind(function () {
            return this.PROFILE_NAME;
        }, this);
    };

    ProfileCreator.prototype = {
        PROFILE_NAME: "Mashups",
        create: function () {
            var createDeferred = $.Deferred();
            var profile = {
                Name: this.PROFILE_NAME,
                Settings: {
                    MashupNames: []
                }
            };

            tpBus.subscribe('MashupsManagerProfileView', {
                onProfileSaveSucceed: _.bind(function (profileCreateDeferred, newProfile) {
                    profileCreateDeferred.resolve(newProfile);
                }, this, createDeferred, profile),
                onProfileSaveFailed: _.bind(function (profileCreateDeferred, error) {
                    profileCreateDeferred.reject(error);
                }, this, createDeferred)
            }, true);
            tpProfileRepository.create(profile);

            return createDeferred.promise();
        }
    };

    return ProfileCreator;
});
