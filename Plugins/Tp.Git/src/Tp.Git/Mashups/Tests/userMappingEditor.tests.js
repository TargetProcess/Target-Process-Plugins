require(["tp/plugins/vcs/SubversionProfileEditorDefaultController", "Git/ProfileEditor"], function (SubversionProfileEditorDefaultController, ProfileEditor) {

    module('user mapping editor', {
        setup: function () {
            this._placeHolder = $('<div class="plugins"/>');

            this._profileRepository = new ProfileRepositoryMock();
            this._profileNameSource = new ProfileNameSourceMock();
            this._navigator = new NavigatorMock();
            this._commandGateway = new CommandGatewayMock();
            this._tpUsers = [
                { Id: "1", Name: 'tpuser1', Email: 'tpuser1@company.com' },
                { Id: "2", Name: 'tpuser2', Email: 'tpuser2@company.com' }
            ];

            this._model = { Settings: {
                Uri: 'uri',
                Login: 'Login',
                Password: 'Password',
                StartRevision: '5',

                UserMapping: [
                { Key: 'svnuser1', Value: { Name: this._tpUsers[0].Name, Id: this._tpUsers[0].Id} },
                { Key: 'svnuser2', Value: { Name: this._tpUsers[1].Name, Id: this._tpUsers[1].Id} }
            ]
            }
            };

            this._createController = function () {
                return new SubversionProfileEditorDefaultController({
                    profileRepository: this._profileRepository,
                    commandGateway: this._commandGateway,
                    profileNameSource: this._profileNameSource,
                    navigator: this._navigator,
                    tpUsers: this._tpUsers
                });
            };

            this._createEditor = function (model) {
                return new ProfileEditor({
                    placeHolder: this._placeHolder,
                    model: model || this._model,
                    controller: this._createController()
                });
            };

            this._editor = this._createEditor();

            var that = this;
            this._editor.userRepository = {
                getUsers: function (callback) {
                    callback(that._tpUsers);
                }
            };
            
            this._editor.render();

            this.convertToUserMapping = function (userBlock) {
                return {
                    Key: userBlock.find('.svnuser').val(),
                    Value: {
                        Name: userBlock.find('.tpuser').val(),
                        Id: userBlock.find('.tpuser').attr('userId')
                    }
                };
            }

            this.areMappedCorrectly = function (userBlock, userMapping) {
                var actual = this.convertToUserMapping(userBlock);
                deepEqual(actual, userMapping);
            };

            this.addUserMappingsToUi = function (userMapping) {
                var userBlocks = this._placeHolder.find('.users-block').slice(1 + this._model.Settings.UserMapping.length);
                for (var i = 0; i < userMapping.length; i++) {
                    $(userBlocks[i]).find('.svnuser').val(userMapping[i].Key);
                    $(userBlocks[i]).find('.tpuser').val(userMapping[i].Value.Name);
                }
            };

            this.shouldBeEmpty = function (userBlock) {
                var actual = this.convertToUserMapping(userBlock);

                equal(actual.Key, '');
                equal(actual.Value.Name, '');
                equal(actual.Value.Id, -1);
            };
        },

        teardown: function () {

        }
    });

    test('should display user mapping', function () {
        var userBlocks = this._placeHolder.find('.users-block').slice(1);
        this.areMappedCorrectly($(userBlocks[0]), this._model.Settings.UserMapping[0]);
        this.areMappedCorrectly($(userBlocks[1]), this._model.Settings.UserMapping[1]);
    });

    test('should display 3 empty user mapping lines by default', function () {
        var all = this._placeHolder.find('.users-block');
        var emptyUserMappingLines = all.slice(this._model.Settings.UserMapping.length + 1);
        equal(emptyUserMappingLines.length, 3);

        var array = emptyUserMappingLines.toArray();
        for (var i = 0; i < array.length; i++) {
            this.areMappedCorrectly($(array[i]), { Key: '', Value: { Name: '', Id: '-1'} });
        };
    });

    test('should add 3 empty user mapping fields when Add more users button clicked', function () {
        var linesCountBeforeClicked = this._placeHolder.find('.users-block').length;
        this._placeHolder.find('#addMoreUsers').click();
        equal(this._placeHolder.find('.users-block').length - linesCountBeforeClicked, 3, 'empty lines were not append');
    });

    test('should retrieve edited user mapping', function () {
        var newMappings = [
            {
                Key: 'svnuser3',
                Value: {
                    Name: 'tpuser3',
                    Id: "-1"
                }
            },
            {
                Key: 'svnuser4',
                Value: {
                    Name: 'tpuser4',
                    Id: "-1"
                }
            }
        ];
        this.addUserMappingsToUi(newMappings);

        var model = this._model;
        $.each(newMappings, function (index, userMapping) {
            model.Settings.UserMapping.push(userMapping);
        });

        deepEqual(this._editor._getProfileFromEditor().Settings.UserMapping, this._model.Settings.UserMapping);
    });

    test('should show automapping results if connection check succeeded', function () {
        function convertToTpUserMappingInfo(userMapping) {
            return {
                Id: userMapping.Value.Id,
                Name: userMapping.Value.Name,
                Email: userMapping.Value.Name + "@company.com"
            }
        };

        var automapCmdArgs = {
            Connection: {
                Uri: this._model.Settings.Uri,
                Login: this._model.Settings.Login,
                Password: this._model.Settings.Password,
                StartRevision: this._model.Settings.StartRevision,
                UserMapping: []
            },
            TpUsers: [
                convertToTpUserMappingInfo(this._model.Settings.UserMapping[0]),
                convertToTpUserMappingInfo(this._model.Settings.UserMapping[1])
            ]
        };

        var automappingResult = {
            Comment: "All svn users were mapped",
            UserLookups: this._model.Settings.UserMapping
        };

        this._model.Settings.UserMapping = undefined;

        this._commandGateway.shouldReturn("CheckConnection", []);
        this._commandGateway.shouldReturn("Automap%20People", { args: automapCmdArgs, result: automappingResult });

        this._editor = this._createEditor();
        this._editor.render();

        this._placeHolder.find(".automapping .button").click();

        var userBlocks = this._placeHolder.find('.users-block').slice(1);
        this.areMappedCorrectly($(userBlocks[0]), automappingResult.UserLookups[0]);
        this.areMappedCorrectly($(userBlocks[1]), automappingResult.UserLookups[1]);
        this.areMappedCorrectly($(userBlocks[2]), { Key: '', Value: { Name: '', Id: "-1"} });
        this.areMappedCorrectly($(userBlocks[3]), { Key: '', Value: { Name: '', Id: "-1"} });
        this.areMappedCorrectly($(userBlocks[4]), { Key: '', Value: { Name: '', Id: "-1"} });

        equal(userBlocks.length, 5);

        ok(this._placeHolder.find(".automapping-result").css('display') != 'none', 'automapping result should be visible');
        equal(this._placeHolder.find(".automapping-result .warning-message").text(), automappingResult.Comment);
    });

});