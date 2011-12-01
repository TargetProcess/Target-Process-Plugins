function PluginsRepositoryMock(config) {
	this._create(config);
}

PluginsRepositoryMock.prototype = {

	_create: function () {
		this.plugins =
			[{
				"category": "New Plugins",
				"plugins": [
					{
						"name": "Bugzilla",
						"description": null,
						"newProfileUrl": "/targetprocess/Admin/EditProfileExt.aspx?acid=56B47A374989E87FC3C87D817A69C46E&PluginName=Bugzilla&Placeholders=profileeditorbugzilla&BackUrl=%2ftargetprocess%2fAdmin%2fPlugins.aspx",
						"profiles": [{
							"name": "bz",
							"pluginName": "Bugzilla",
							"editUrl": "/targetprocess/Admin/EditProfileExt.aspx?acid=56B47A374989E87FC3C87D817A69C46E&PluginName=Bugzilla&ProfileName=bz&Placeholders=profileeditorbugzilla&BackUrl=%2ftargetprocess%2fAdmin%2fPlugins.aspx"
						}]
					}]
			}];

		this.bugzillaBugs =
			[
				{
					"Classification": "Unclassified",
					"Component": "TestComponent",
					"CustomFields": [{ "Name": "cf_cm", "Values": [null]}],
					"Id": "1",
					"OS": "Windows",
					"Platform": "PC",
					"Reporter": "bugzilla@targetprocess.com",
					"TpId": 100,
					"Version": "unspecified"
				},
				{
					"Classification": "Unclassified",
					"Component": "TestComponent",
					"CustomFields": [{ "Name": "cf_cm", "Values": [null]}],
					"Id": "2",
					"OS": "Windows",
					"Platform": "PC",
					"Reporter": "bugzilla@targetprocess.com",
					"TpId": 102,
					"Version": "unspecified"
				},
				{
					"Classification": "Unclassified",
					"Component": "TestComponent",
					"CustomFields": [{ "Name": "cf_cm", "Values": [null]}],
					"Id": "10",
					"OS": "Windows",
					"Platform": "PC",
					"Reporter": "bugzilla@targetprocess.com",
					"TpId": 110,
					"Version": "unspecified"
				},
				{
					"Classification": "Unclassified",
					"Component": "TestComponent",
					"CustomFields": [{ "Name": "cf_cm", "Values": [null]}],
					"Id": "20",
					"OS": "Windows",
					"Platform": "PC",
					"Reporter": "bugzilla@targetprocess.com",
					"TpId": 120,
					"Version": "unspecified"
				}
			];
	},

	pluginStartedAndHasAtLeastOneProfile: function (pluginName, onBuzillaPluginStartedAndHasAtLeastOnePlugin) {
		this.getStartedPlugins($.proxy(function (data) {
			var bugzilla = $.grep(data, function (element) {

				var bugzillaPlugin = $.grep(element.plugins, function (plugin) {
					return plugin.name == 'Bugzilla';
				});

				if (bugzillaPlugin.length === 0) {
					return false;
				}

				var hasProfiles = false;

				$(bugzillaPlugin).each(function (index, plugin) {
					if (plugin.profiles.length > 0) {
						hasProfiles = true;
					}
				});

				return hasProfiles;
			});

			if (bugzilla.length > 0) {
			    onBuzillaPluginStartedAndHasAtLeastOnePlugin();
			}
		}, this));
	},

	getStartedPlugins: function (onBugzillaPluginStarted) {
		onBugzillaPluginStarted(this.plugins);
	}
};