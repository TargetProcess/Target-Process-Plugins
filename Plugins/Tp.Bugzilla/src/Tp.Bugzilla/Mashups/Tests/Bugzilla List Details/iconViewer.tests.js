require(["BugzillaListDetails/IconViewer", "tp/plugins/bugzilla/bugsRepository"],
    function (viewer, repository) {
        (function () {
            module('check bugzilla repository', {
                setup: function () {
                    $('body').append('<table class="generalTable"><tbody><tr><th><a href="#">ID</a></th></tr><tr><td>100</td></tr><tr><td>105</td></tr></tbody></table>');

                    this.repository = repository;
                    this.repository.gateway = this.commandGateway;
                    this.viewer = new viewer();
                    this.viewer.pluginsRepository = new PluginsRepositoryMock();
                    this.viewer.bugzillaRepository = new BugsRepositoryMock();

                    this.profilesChecked = false;

                    this.findBugById = function (bugId) {
                        return function (element) {
                            return element.TpId === bugId;
                        };
                    };
                },

                teardown: function () {
                    $('.generalTable').remove();
                }
            });

            test('should not draw icons if bugzilla not started', function () {
                this.viewer.pluginsRepository.getStartedPlugins = function (onBugzillaPluginStarted) {
                    onBugzillaPluginStarted({});
                };
                this.viewer.drawBugIcons();
                ok($('.bugzillaBugIcon').length === 0, 'icons are not shown because Bugzilla plugin is not started');
            });

            test('should not draw icons if there is no profiles in bugzilla', function () {
                this.viewer.pluginsRepository.getStartedPlugins = function (onBugzillaPluginStarted) {
                    onBugzillaPluginStarted([{
                        "category": "New Plugins",
                        "plugins": []
                    }]);
                };
                this.viewer.drawBugIcons();
                ok($('.bugzillaBugIcon').length === 0, 'icons are not shown because there is no profiles in bugzilla');
            });

            test('should draw icons for bugzilla bugs', function () {
                this.viewer.drawBugIcons();
                ok($('.bugzillaBugIcon').length === 1, 'icon for a bugzilla bug shown');
            });

            test('should draw icons for bugzilla bug when update panel with grid is updated', function () {
                var args = {
                    get_panelsUpdated: function () {
                        return [{ id: 'updatePanelGrid'}];
                    }
                };
                this.viewer.drawBugIcons(this, args);
                ok($('.bugzillaBugIcon').length === 1, 'icon for a bugzilla bug shown during update panel update');
            });

            test('should not draw icons for bugzilla bug when update panel that doesn\'t contain bugs grid updated', function () {
                this.viewer.drawBugIcons();
                var args = {
                    get_panelsUpdated: function () {
                        return [{ id: 'someOtherId'}];
                    }
                };
                this.viewer.drawBugIcons(this, args);
                ok($('.bugzillaBugIcon').length === 1, 'icon for a bugzilla bug shown during update panel update');
            })
        })();
    });