require(["BugzillaViewDetails/BugInfoViewer"],
    function(viewer) {
        (function() {
            module('check bugzilla repository', {
                setup:function() {
                    this.placeholder = $('<div/>');
                    this.viewer = new viewer({ placeholder:this.placeholder });
                    this.data = [
                        {"Classification":"Unclassified", "Component":"TestComponent", "CustomFields":[
                            {"Name":"cf_dropdown", "Values":["---"]},
                            {"Name":"cf_multipleselection", "Values":["ARRAY(0x33ccdb0)"]}
                        ], "Id":"6806", "OS":"Windows", "Platform":"PC", "Reporter":"bugzilla@targetprocess.com", "TpId":2058, "Url":"http:\/\/new-bugzilla\/bugzilla363\/show_bug.cgi?id=6806", "Version":"unspecified"}
                    ];
                },

                teardown:function() {
                }
            });

            test('should show bugzilla bug info', function() {
                this.viewer.pluginsRepository.pluginStartedAndHasAtLeastOneProfile = function(pluginName, success) {
                    success();
                }
                var that = this;
                this.viewer.bugsRepository.getBugs = function(ids, success) {
                    success(that.data);
                }
                this.viewer.render();

                ok(this.placeholder.find('._header').length > 0, 'Bug info block header is shown');
                equal(this.placeholder.find('._id').text(), this.data[0].Id, 'Bug id displayed');
                equal(this.placeholder.find('._reporter').text(), this.data[0].Reporter);
                equal(this.placeholder.find('._os').text(), this.data[0].OS);
                equal(this.placeholder.find('._platform').text(), this.data[0].Platform);
                equal(this.placeholder.find('._component').text(), this.data[0].Component);
                equal(this.placeholder.find('._version').text(), this.data[0].Version);
                equal(this.placeholder.find('._classification').text(), this.data[0].Classification);
//                for (var i = 0; i < this.data[0].CustomFields.length; i++){
//                    equal(this.placeholder.find(''))
//                }
            });
        })();
    });