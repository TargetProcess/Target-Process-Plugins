function NavigatorMock(config){
    this._create(config);
};

NavigatorMock.prototype = {
    _create: function(config) {
        this.log = '';
    },

    backToPlugins: function() {
        this.log = this.log + "backToPlugins ";
    },

    reloadPage: function() {
        this.log = this.log + "reloadPage ";
    },

    setProfileNameIfNecessary: function(profileName, profileNameSource) {
//        if (profileNameSource.getProfileName()) {
//            return;
//        }
//        window.location = window.location + '#' + profileName;
    }
};
