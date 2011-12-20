tau.mashups
    .addDependency("MashupManager/CommandGateway")
    .addDependency("libs/jquery/jquery")
    .addModule("MashupManager/ProfileRepository", function (commandGateway) {
        function profileRepository(config) {
            this._commandGateway = new commandGateway();
            this._defaultErrorHandler = config.defaultErrorHandler;
        }

        ;

        profileRepository.prototype = {

            getProfile: function(success, fail, error) {
                var orderMashups = function(profile) {
                    if (profile.Settings.MashupNames && profile.Settings.MashupNames.length > 0) {
                        profile.Settings.MashupNames = profile.Settings.MashupNames.sort(function (a, b) {
                            return a.toLocaleLowerCase().localeCompare(b.toLocaleLowerCase());
                        });
                    }

                    success(profile);
                };

                this._commandGateway.execute('GetProfileInfo', null, orderMashups, fail, error || this._defaultErrorHandler);
            },

            getMashupByName: function(name, success, fail, error) {
                this._commandGateway.execute('GetMashupInfo', name, success, fail, error || this._defaultErrorHandler);
            },

            addMashup: function(mashup, success, fail, error) {
                this._commandGateway.execute('AddMashup', mashup, success, fail, error || this._defaultErrorHandler);
            },

            updateMashup: function(mashup, success, fail, error) {
                this._commandGateway.execute('UpdateMashup', mashup, success, fail, error || this._defaultErrorHandler);
            },

            deleteMashup: function(mashup, success, fail, error) {
                this._commandGateway.execute('DeleteMashup', mashup, success, fail, error || this._defaultErrorHandler);
            }
        };
        return profileRepository;
    });