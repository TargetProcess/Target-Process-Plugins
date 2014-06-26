require([
    'Underscore',
    'BugzillaViewDetails/NewViewDetails'],
    function (_, viewer) {
        (function () {
            module('check bugzilla repository', {
                setup: function () {
                    this.viewer = new viewer();
                    this.data = [
                        {"Classification": "Unclassified", "Component": "TestComponent", "CustomFields": [
                            {"Name": "cf_dropdown", "Values": ["---"]},
                            {"Name": "cf_multipleselection", "Values": ["ARRAY(0x33ccdb0)"]}
                        ], "Id": "6806", "OS": "Windows", "Platform": "PC", "Reporter": "bugzilla@targetprocess.com", "TpId": 2058, "Url": "http:\/\/new-bugzilla\/bugzilla363\/show_bug.cgi?id=6806", "Version": "unspecified"}
                    ];
                },

                teardown: function () {
                }
            });

            test('should show bugzilla bug info', function () {
                var pluginsRepository = {
                    pluginStartedAndHasAtLeastOneProfile: function (pluginName, success) {
                        success();
                    }
                };

                var bugsRepository = {
                    getBugs: function (ids, success) {
                        success(this.data);
                    }.bind(this)
                };

                var renderer = {
                    addBlock: function (name, renderContent, renderHeader, options) {
                        options.getViewIsSuitablePromiseCallback({entity: {id: 6806}})
                            .done(function () {
                                var $el = $('<div></div>');
                                renderContent($el);

                                var data = this.data[0];
                                ok($el.find('.additional-info-table').length > 0, 'Bug info block header is shown');
                                equal($el.find('.i-role-id').text(), data.Id, 'Bug Id is displayed');
                                equal($el.find('.i-role-reporter').text(), data.Reporter, 'Bug reporter is displayed');
                                equal($el.find('.i-role-os').text(), data.OS, 'Bug OS is displayed');
                                equal($el.find('.i-role-platform').text(), data.Platform, 'Bug Platform is displayed');
                                equal($el.find('.i-role-component').text(), data.Component, 'Bug Component is displayed');
                                equal($el.find('.i-role-version').text(), data.Version, 'Bug Version is displayed');
                                equal($el.find('.i-role-classification').text(), data.Classification, 'Bug Classification is displayed');

                                var cfs = $el.find('.i-role-customField');
                                equal(cfs.length, data.CustomFields.length, 'All Custom Fields are shown');
                                equal(cfs.eq(0).text(), data.CustomFields[0].Values[0], 'First Custom Field is shown');
                                equal(cfs.eq(1).text(), data.CustomFields[1].Values[0], 'Second Custom Field is shown');
                            }.bind(this));
                    }.bind(this)
                };

                this.viewer.render(renderer, bugsRepository, pluginsRepository);
            });
        })();
    });