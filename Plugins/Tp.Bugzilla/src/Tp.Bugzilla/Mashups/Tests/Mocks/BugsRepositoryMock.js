function BugsRepositoryMock(config) {
	this._create(config);
}

BugsRepositoryMock.prototype = {

    _create: function () {
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

    getBugs: function (data, success) {
        var bugs = $.grep(this.bugzillaBugs, function (element) {
            return $.inArray(element.TpId, data) > -1;
        });

        success(bugs);
    }
};