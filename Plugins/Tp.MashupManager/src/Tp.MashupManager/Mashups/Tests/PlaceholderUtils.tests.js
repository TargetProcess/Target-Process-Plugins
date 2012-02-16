require(["MashupManager/PlaceholderUtils"],
    function (utils) {
        (function () {
            module('check url to placeholder parsed correctly', {
                setup: function () {
                    this.utils = new utils({});
                },

                teardown: function () {

                }
            });

            test('check relative url parsed correctly', function () {
                var placeholder = this.utils.urlToPlaceholder('/Project/Planning/UserStory/Edit.aspx');

                ok(placeholder == 'Project_Planning_UserStory_Edit', 'relative url should be parsed to placeholder');
            });

            test('check absolute url parsed correctly', function () {
                var placeholder = this.utils.urlToPlaceholder('http://plan.tpondemand.com/Project/Planning/UserStory/Edit.aspx?UserStoryID=39842');

                ok(placeholder == 'Project_Planning_UserStory_Edit', 'absolute url should be parsed to placeholder');
            });

            test('check that valid placeholder will not be affected', function () {
                var placeholder = this.utils.urlToPlaceholder('Project_Planning_UserStory_Edit');

                ok(placeholder == 'Project_Planning_UserStory_Edit', 'placeholder should not be affected');
            });
        })();
    });