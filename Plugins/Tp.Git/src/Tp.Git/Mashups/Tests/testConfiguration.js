tau.mashups
	.addDependency("tp/plugins/vcs/SubversionProfileEditorDefaultController")
	.addDependency("Git/ProfileEditor")
	.addDependency("testMethods")
	.addModule("testConfiguration", function (SubversionProfileEditorDefaultController, SubversionProfileEditor, testMethods) {

	    function testConfiguration(config) {
	        this._create(config);
	    }

	    testConfiguration.prototype = {
	        _profileRepository: null,
	        _profileNameSource: null,
	        _navigator: null,
	        commandGateway: null,

	        _create: function (config) {
	            this.setupMocks();
	        },

	        setupMocks: function () {
	            this._profileRepository = new ProfileRepositoryMock({name: 'Profile#1'});
	            this._profileNameSource = new ProfileNameSourceMock();
	            this._navigator = new NavigatorMock();
	            this.commandGateway = new CommandGatewayMock();
	        },

	        _profile: function (name, login, password, uri, startRevision, userMapping) {
	            return {
	                "Name": name,
	                "Settings": {
	                    "Login": login,
	                    "Password": password,
	                    "Uri": uri,
	                    "StartRevision": startRevision,
	                    UserMapping: userMapping
	                }
	            };
	        },

	        model: function () {
	            return this._profile(
					"Profile#1", "test", "123456", "file:\/\/\/D:\/diff\/repos\/RepositoryToTestSvn", "0", [
						{ Key: 'svnuser1', Value: { Name: 'tpuser1', Id: '1'} },
						{ Key: 'svnuser2', Value: { Name: 'tpuser2', Id: '2'} }
					]);
	        },

	        otherModel: function () {
	            return this._profile(
					"Profile#1edited", "testedited", "123456edited", "file:\/\/\/D:\/diff\/repos\/RepositoryToTestSvnedited", "5", [
						{ Key: 'svnuser1', Value: { Name: 'tpuser1', Id: '1'} },
						{ Key: 'svnuser2', Value: { Name: 'tpuser2', Id: '2'} }
					]);
	        },

	        renderEditor: function (model) {
	            var controller = new SubversionProfileEditorDefaultController({
	                profileRepository: this._profileRepository,
	                commandGateway: this.commandGateway,
	                profileNameSource: this._profileNameSource,
	                navigator: this._navigator
	            });

	            var placeHolder = $('<div />');

	            var editor = new SubversionProfileEditor({
	                placeHolder: placeHolder,
	                model: model || this.model(),
	                controller: controller
	            });

	            editor.render();

	            this.testMethods = new testMethods({ placeHolder: placeHolder, editor: editor });
	        }
	    };
	    return testConfiguration;
	});

	tau.mashups
	.addDependency("libs/jquery/jquery")
	.addModule("testMethods", function () {


	    function testMethods(config) {
	        this._create(config);
	    }

	    testMethods.prototype = {
	        _create: function (config) {
	            this._editor = config.editor;
	            this._placeHolder = config.placeHolder;
	        },

	        checkConnectionClick: function () {
	            this._placeHolder.find('#checkConnection').click();
	        },

	        hasError: function (fieldName) {
	            return this._placeHolder.find('#uri').hasClass('error');
	        },

	        hasCheckConnectionFailedMessage: function () {
	            return this._placeHolder.find('#failedConnection').css('display') != 'none';
	        },

	        hasPreloader: function () {
	            return this._placeHolder.find('span.preloader').css('display') != 'none';
	        },

	        hasSuccessMessage: function () {
	            return this._placeHolder.find('.svn-settings p.message-ok').length > 0;
	        },

	        getUserBlocks: function () {
	            return this._placeHolder.find('.users-block').slice(1);
	        },

	        getUserBlock: function (index) {
	            return $(this.getUserBlocks()[index]);
	        },

	        removeMappingIconPresent: function () {
	            return this._placeHolder.find('.remove-mapping').length > 0;
	        },

	        mouseoverUserMappingLine: function (index) {
	            this.getUserBlock(index).mouseenter();
	        },

	        mouseoutUserMappingLine: function (index) {
	            this.getUserBlock(index).mouseleave();
	        },

	        clickRemoveUserMappingIcon: function (index) {
	            this.mouseoverUserMappingLine(index);
	            this.getUserBlock(index).find('.remove-mapping a').click();
	        },

	        unbind: function () {
	            return this._editor._getProfileFromEditor();
	        }
	    };
	    return testMethods;
	});