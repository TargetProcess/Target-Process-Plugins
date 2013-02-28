require(["tp/plugins/vcs/SubversionProfileEditorDefaultController", "testConfiguration"],
	function (SubversionProfileEditorDefaultController, testConfiguration) {

			module('check connection tests', {
				setup: function () {
					this.testConfiguration = new testConfiguration();

					this.testConfiguration.renderEditor();

					this.checkConnectionShouldFailForUri = function() {
						this.testConfiguration.commandGateway.shouldReturn("CheckConnection", [
							{ FieldName: "Uri", Message: "IncorrectUri" }
						]);
					},

					this.checkConnectionShouldSucceed = function() {
						this.testConfiguration.commandGateway.shouldReturn("CheckConnection", []);
					};
				},

				teardown: function () {
				}
			});

			test('should hide error message and do not highlight error fields when check connection fails and then succeeds', function () {
				this.checkConnectionShouldFailForUri();
				this.testConfiguration.testMethods.checkConnectionClick();

				this.checkConnectionShouldSucceed();
				this.testConfiguration.testMethods.checkConnectionClick();

				ok(!this.testConfiguration.testMethods.hasError('uri'), 'Uri field should not be marked as error');
			});

			test('should show error message and highlight error fields when check connection fails', function () {
				this.checkConnectionShouldFailForUri();

				this.testConfiguration.testMethods.checkConnectionClick();

				ok(this.testConfiguration.testMethods.hasError('uri'), 'Uri field should be marked as error');
				notEqual(this.testConfiguration.testMethods.hasCheckConnectionFailedMessage(), 'error message is shown if connection check failed');
			});

			test('should show preloader while checking connection', function () {
				ok(!this.testConfiguration.testMethods.hasPreloader(), 'Preloader should not appear at start');

				this.checkConnectionShouldFailForUri();

				this.testConfiguration.commandGateway.pause();
				this.testConfiguration.testMethods.checkConnectionClick();
				ok(this.testConfiguration.testMethods.hasPreloader(), 'Preloader should appear while checking connection');
				this.testConfiguration.commandGateway.resume();
				ok(!this.testConfiguration.testMethods.hasPreloader(), 'Preloader should disapear when checking connection finished');
			});

			test('should show successful status if checking connection succeed', function () {
				this.checkConnectionShouldSucceed();
				this.testConfiguration.testMethods.checkConnectionClick();

				ok(this.testConfiguration.testMethods.hasSuccessMessage(), 'success message should be displayed');
			});

			test('should hide green message if checking connection succeed and then failed', function () {
				this.checkConnectionShouldSucceed();
				this.testConfiguration.testMethods.checkConnectionClick();

				this.checkConnectionShouldFailForUri();
				this.testConfiguration.testMethods.checkConnectionClick();

				ok(!this.testConfiguration.testMethods.hasSuccessMessage(), 'Green success message should be hidden');
			});
	});