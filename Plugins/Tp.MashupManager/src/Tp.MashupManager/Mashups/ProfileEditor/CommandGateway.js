tau.mashups
    .addDependency("MashupManager/ProfileNameSource")
    .addModule("MashupManager/CommandGateway", function (profileNameSource) {
        function commandGateway(config) {
            this._create(config);
        }

        commandGateway.prototype = {
            _requestUrlBase: '/api/v1/Plugins.asmx/{PluginName}/Commands/{CommandName}',
            _pluginName: null,

            _create: function (config) {
                this._pluginName = new profileNameSource().getPluginName();
            },

            _getUrl: function (commandName) {
                var relativeUrl = this._requestUrlBase.replace(/{PluginName}/g, this._pluginName).replace(/{CommandName}/g, commandName);
                return new Tp.WebServiceURL(relativeUrl).url;
            },

            _post: function (commandName, data, success, fail, error) {
                $.ajax({
                    url: this._getUrl(commandName),
                    data: typeof data === 'string' ? data : JSON.stringify(data),
                    success: success,
                    error: $.proxy(
                        function (response) {
                            this._processError(response, fail, error);
                        },
                        this),
                    type: 'POST',
                    dataType: "json"
                });
            },

            _processError: function (response, fail, error) {
                if (response.getResponseHeader('content-type').indexOf('application/json') != -1) {
                    if (response.status == 400 && fail) {
                        fail(response.responseText);
                    } else if (response.status == 500 && error) {
                        error(response.responseText);
                    } else {
                        alert(response.responseText);
                    }
                } else {
                    var text = $('<div />').html(response.responseText).text();
                    alert(text);
                }
            },

            execute: function (command, data, success, fail, error) {
                this._post(command, data, success, fail, error);
            }
        };

        return commandGateway;
    });