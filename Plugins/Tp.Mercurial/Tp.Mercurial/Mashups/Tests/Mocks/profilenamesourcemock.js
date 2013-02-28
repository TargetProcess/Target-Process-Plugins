function ProfileNameSourceMock(config){
    this._create(config);
};

ProfileNameSourceMock.prototype = {
    _create: function(config){
    },

    getProfileName: function(){
        return "Profile#1";
    }
};
