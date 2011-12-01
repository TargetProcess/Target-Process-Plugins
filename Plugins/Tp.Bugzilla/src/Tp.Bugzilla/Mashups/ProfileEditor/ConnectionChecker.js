tau.mashups
    .addDependency("tp/plugins/commandGateway")
    .addDependency("tp/plugins/errorMessageContainer")
    .addDependency("libs/jquery/jquery")
    .addModule("Bugzilla/ConnectionChecker", function (commandGateway, errorMessageContainer, $) {
        function connectionChecker(config) {
            this._ctor(config);
        }

        connectionChecker.prototype = {
            _ctor: function (config) {
                this.placeholder = config.placeholder;
                this.commandGateway = new commandGateway();
                this.errorMessageContainer = new errorMessageContainer({placeholder: this.placeholder, generalErrorMessage: 'Unable to establish connection', generalErrorContainer: '#failedConnection'});
                this.profileRetriever = config.profileRetriever;
                this.loaderSelector = config.loaderSelector;
                this.quiet = config.quiet;
            },

            initialize: function () {
                this.preloader = this.placeholder.find(this.loaderSelector);
                this.successfulConnection = this.placeholder.find('#successfulConnection');
            },

            checkConnection: function (success, failure) {
                this.hideConnectionSuccessfulMessage();
                this.errorMessageContainer.clearErrors();
                this.preloader.show();
                var profile = this.profileRetriever();
                this.commandGateway.execute("CheckConnection",
                    profile,
                    $.proxy(this._onCheckConnectionSuccess(success), this),
                    $.proxy(this._onCheckConnectionFail(failure), this),
                    $.proxy(this._onCheckConnectionError, this));
            },

            _onCheckConnectionSuccess: function (success) {
                return function (data) {
                    this._showConnectionSuccess();
                    this.preloader.hide();
                    if (success) {
                        success(data);
                    }
                };
            },

            _onCheckConnectionFail: function (failure) {
                return function (responseText){
                    this.preloader.hide();

                    var data = JSON.parse(responseText);
                    this.errorMessageContainer.addRange(data);
                    this.errorMessageContainer.render();
                    if (failure){
                        failure(data);
                    }
                };
            },

            _onCheckConnectionError: function (responseText) {
                this.errorMessageContainer.add({ FieldName: null, Message: responseText });
                this.errorMessageContainer.render();
                this.preloader.hide();
            },

            _showConnectionSuccess: function () {
                this.errorMessageContainer.clearErrors();
                if (!this.quiet || this.quiet !== true) {
                    this.successfulConnection.show();
                }
            },

            hideConnectionSuccessfulMessage: function () {
                this.successfulConnection.hide();
            }
        };
        return connectionChecker;
    });