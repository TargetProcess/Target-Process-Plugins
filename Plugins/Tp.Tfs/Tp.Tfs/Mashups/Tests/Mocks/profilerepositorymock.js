function ProfileRepositoryMock(config) {
    this._create(config);
};

ProfileRepositoryMock.prototype = {
    lastCreatedProfile: null,
    lastUpdatedProfile: null,
    lastUpdatedProfileName: null,
    successCallback: null,
    errorCallback: null,

    _create: function (config) {
    },

    setProfileName: function (profileName) {
        this.name = profileName;
    },

    getCurrentProfile: function (success) {
        success(this._getProfileStub(typeof this.name == 'undefined' ? 'Profile#1' : this.name));
    },

    _getProfileStub: function (name) {
        if (name == 'Profile#1') {
            return {
                "Name": "Profile#shouldnotbeused",
                "Settings": {
                    "Login": "testshouldnotbeused",
                    "Password": "1234567shouldnotbeused",
                    "Uri": "file:\/\/\/D:\/diff\/repos\/RepositoryToTestSvnshouldnotbeused",
                    "StartRevision": "0shouldnotbeused"
                }
            };
        }

        return null;
    },

    getCurrentProfileName: function () {
        return this.name;
    },

    create: function (profile) {
        this.lastCreatedProfile = profile;
    },

    update: function (profile) {
        this.lastUpdatedProfileName = this.name;
        this.lastUpdatedProfile = profile;
    }
};

