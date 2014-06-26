define([
    'jQuery'
], function ($) {
    var ServiceMashupManagerConfig = function (configurator) {
        this.configDeferred = $.Deferred();
        var restStorage = configurator.getRestStorage();
        restStorage.data('boards').done($.proxy(function (result) {
            if (result.data.length == 0){
                this.configDeferred.resolve({libraryIsAllowed: false});
            }
            this.configDeferred.resolve({libraryIsAllowed: true});
        }, this));
    };

    ServiceMashupManagerConfig.prototype = {
        getConfig: function () {
            return this.configDeferred.promise();
        }
    };
    return ServiceMashupManagerConfig;
});