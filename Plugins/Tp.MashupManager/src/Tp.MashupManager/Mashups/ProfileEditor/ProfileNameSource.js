tau.mashups
    .addModule("MashupManager/ProfileNameSource", function () {
    function profileNameSource(config) {
        this._create(config);
    };

    profileNameSource.prototype = {
        _create: function(config) {
        },

        getPluginName: function() {
            return new Tp.URL(location.href).getArgumentValue('PluginName');
        }
    };
    return profileNameSource;
});