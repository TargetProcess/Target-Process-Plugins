tau.mashups
    .addDependency("MashupManager/CommandGateway")
    .addDependency("libs/jquery/jquery")
    .addModule("MashupManager/ProfileRepository", function (commandGateway) {
        function profileRepository(config) {
            this._commandGateway = new commandGateway();
            this._defaultErrorHandler = config.defaultErrorHandler;
        };

        profileRepository.prototype = {

            getProfile: function(success, fail, error) {
                this._commandGateway.execute('GetProfileInfo', null, success, fail, error || this._defaultErrorHandler);
            },

            getMashupByName: function(name, success, fail, error){
                this._commandGateway.execute('GetMashupInfo', name, success, fail, error || this._defaultErrorHandler);
            },

            addMashup: function(mashup, success, fail, error){
                this._commandGateway.execute('AddMashup', mashup, success, fail, error || this._defaultErrorHandler);
            },

            updateMashup: function(mashup, success, fail, error){
                this._commandGateway.execute('UpdateMashup', mashup, success, fail, error || this._defaultErrorHandler);
            },

            deleteMashup: function(mashup, success, fail, error){
                this._commandGateway.execute('DeleteMashup', mashup, success, fail, error || this._defaultErrorHandler);
            }
        };
        return profileRepository;
    });