require(["testConfiguration", "Git/ProfileEditor"], function (testConfiguration, ProfileEditor) {
	(function() {
		module('tp user mapping line tests', {
			setup: function() {
				this.testConfiguration = new testConfiguration();
				this.testConfiguration.renderEditor();
			},

			teardown: function() {
				delete this.testConfiguration;
			}
		});

		test('should not show remove icon by default', function() {
			ok(!this.testConfiguration.testMethods.removeMappingIconPresent());
		});

		test('should show remove when hovering user mapping line', function() {
			this.testConfiguration.testMethods.mouseoverUserMappingLine(0);
			ok(this.testConfiguration.testMethods.removeMappingIconPresent());
		});

		test('should hide remove when hovering user mapping line', function() {
			this.testConfiguration.testMethods.mouseoverUserMappingLine(0);
			this.testConfiguration.testMethods.mouseoutUserMappingLine(0);
			ok(!this.testConfiguration.testMethods.removeMappingIconPresent());
		});

		test('should remove user mapping line by clicking remove icon', function() {
			this.testConfiguration.testMethods.clickRemoveUserMappingIcon(1);

			var actualLinesCount = this.testConfiguration.testMethods.getUserBlocks().length;
			var expectedLinesCount = this.testConfiguration.model().Settings.UserMapping.length + 2;

			equal(actualLinesCount, expectedLinesCount, 'mapping should be deleted from ui');
			var model = this.testConfiguration.model();
			model.Settings.UserMapping.pop();
			deepEqual(this.testConfiguration.testMethods.unbind(), model, 'mapping should be deleted from model');
		});

		test('should remove user mapping line by clicking remove icon that was once hidden', function() {
			this.testConfiguration.testMethods.mouseoverUserMappingLine(0);
			this.testConfiguration.testMethods.mouseoutUserMappingLine(0);
			this.testConfiguration.testMethods.clickRemoveUserMappingIcon(0);

			var actualLinesCount = this.testConfiguration.testMethods.getUserBlocks().length;
			var expectedLinesCount = this.testConfiguration.model().Settings.UserMapping.length + 2;

			equal(actualLinesCount, expectedLinesCount, 'mapping should be deleted from ui');
		});
	})();
})

