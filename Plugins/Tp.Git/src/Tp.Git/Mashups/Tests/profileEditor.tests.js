require(["tp/plugins/vcs/SubversionProfileEditorDefaultController", "Git/ProfileEditor"], function (SubversionProfileEditorDefaultController, ProfileEditor) {
    module('show plugin profile tests', {
        setup: function () {
            this._placeHolder = $('<div/>');
            this._profileRepository = new ProfileRepositoryMock();
            this._profileNameSource = new ProfileNameSourceMock();
            this._navigator = new NavigatorMock();
            this._commandGateway = new CommandGatewayMock();
            this._model = {
                "Name": "Profile#1",
                "Settings": {
                    "Login": "test",
                    "Password": "123456",
                    "Uri": "file:\/\/\/D:\/diff\/repos\/RepositoryToTestSvn",
                    "StartRevision": "0",
                    UserMapping: [
                        { Key: 'svnuser1', Value: { Name: 'tpuser1', Id: '1'} },
                        { Key: 'svnuser2', Value: { Name: 'tpuser2', Id: '2'} }
                    ]
                }
            };

            var controller = new SubversionProfileEditorDefaultController({
                profileRepository: this._profileRepository,
                commandGateway: this._commandGateway,
                profileNameSource: this._profileNameSource,
                navigator: this._navigator
            });

            this._createEditor = function (model) {
                return new ProfileEditor({
                    placeHolder: this._placeHolder,
                    model: model || this._model,
                    controller: controller
                });
            };

            this._getEditedProfile = function () {
                return {
                    "Name": "Profile#1edited",
                    "Settings": {
                        "Login": "testedited",
                        "Password": "123456edited",
                        "StartRevision": "5",
                        "Uri": "file:\/\/\/D:\/diff\/repos\/RepositoryToTestSvn",
                        UserMapping: [
                            { Key: 'svnuser1', Value: { Name: 'tpuser1', Id: '1'} },
                            { Key: 'svnuser2', Value: { Name: 'tpuser2', Id: '2'} }
                        ]
                    }
                };
            };

            this.updateProfileEditorWith = function (editedProfile) {
                this._placeHolder.find('#name').val(editedProfile.Name);
                this._placeHolder.find('#uri').val(editedProfile.Settings.Uri);
                this._placeHolder.find('#login').val(editedProfile.Settings.Login);
                this._placeHolder.find('#password').val(editedProfile.Settings.Password);
                this._placeHolder.find('#startRevision').val(editedProfile.Settings.StartRevision);
            };

            this.editUserMapping = function (index, value) {
                var input = this._placeHolder.find('.tpuser:eq(' + index + ')');
                input.val(value);
                input.change();
            };
        },

        teardown: function () {
            delete this._placeHolder;
            delete this._profileRepository;
            delete this._profileNameSource;
            delete this._commandGateway;

            delete this._createEditor;
        }
    });

    test('should display plugin profile', function () {
        var editor = this._createEditor();

        editor.render();

        equals(this._placeHolder.find('#name').val(), 'Profile#1');
        equals(this._placeHolder.find('#uri').val(), 'file:///D:/diff/repos/RepositoryToTestSvn');
        equals(this._placeHolder.find('#login').val(), 'test');
        equals(this._placeHolder.find('#password').val(), '123456');
        equals(this._placeHolder.find('#startRevision').val(), '0');
    });

    test('should create profile on save button click when in create mode', function () {
        this._profileNameSource.getProfileName = function () {
            return null;
        };

        var editor = this._createEditor();
        editor.render();
        ok(this._placeHolder.find('#name').is(':enabled'), 'name field should be enabled');

        var editedProfile = this._getEditedProfile();
        this.updateProfileEditorWith(editedProfile);

        this._placeHolder.find('#save').click();

        deepEqual(this._profileRepository.lastCreatedProfile, editedProfile);
    });

    test('should not be possible to change profile name when editor is in profile edit mode', function () {
        var editor = this._createEditor();
        this._profileRepository.setProfileName('Profile#1');
        editor.render();

        ok(!this._placeHolder.find('#name').enabled(), 'name field should be disabled');
    });

    test('should be possible to specify profile name when editor is in profile create mode', function () {
        this._profileNameSource.getProfileName = function () {
            return null;
        };
        var editor = this._createEditor();
        editor.render();

        ok(this._placeHolder.find('#name').enabled(), 'Name field should be enabled');
    });

    test('should update profile on save button click when in update mode', function () {
        var editor = this._createEditor();
        this._profileRepository.setProfileName('Profile#1');
        editor.render();

        ok(!this._placeHolder.find('#name').is(':enabled'), 'name field should be disabled');

        var editedProfile = this._getEditedProfile();
        this.updateProfileEditorWith(editedProfile);
        editor._onSave();

        deepEqual(this._profileRepository.lastUpdatedProfile, editedProfile);
        equals(this._profileRepository.lastUpdatedProfileName, this._profileNameSource.getProfileName());
    });

    test('should reset userId attribute', function () {
        var editor = this._createEditor();
        editor.render();

        this.editUserMapping(0, 'John Smith');
        equal(this._placeHolder.find('.tpuser:eq(0)').attr('userId'), '-1', 'userId should be reset');
    });

    test('should highlight field with incorrect user mapping', function () {
        var editor = this._createEditor();
        editor.render();

        this.editUserMapping(0, 'John Smith');
        this._placeHolder.find('#save').click();
        ok((this._profileRepository.lastUpdatedProfile == null) && (this._profileRepository.lastCreatedProfile == null),
                'profile shouldn`t be neither created or updated when user mapping is incorrect');
        ok(this._placeHolder.find('.tpuser:eq(0)').hasClass('ui-error'), 'incorrect field should be marked as error');
    });
})

