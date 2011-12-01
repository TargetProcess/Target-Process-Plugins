tau.mashups
    .addDependency('tp/plugins/bugzilla/bugInfoTemplate')
    .addDependency('BugzillaViewDetails/BugInfoTemplate')
    .addDependency('tp/plugins/bugzilla/bugsRepository')
    .addDependency('tp/plugins/pluginsRepository')
    .addDependency('libs/jquery/jquery')
    .addDependency('libs/jquery/jquery.tmpl')
    .addModule('BugzillaViewDetails/BugInfoViewer', function (template, wrapperTemplate, bugsRepository, pluginsRepository, $) {

        function BugInfoViewer(config) {
            this._ctor(config);
        }

        BugInfoViewer.prototype = {
            _ctor: function (config) {
                this.placeholder = config.placeholder;
                this.template = template;
                this.wrapperTemplate = wrapperTemplate;
                this.bugsRepository = bugsRepository;
                this.pluginsRepository = pluginsRepository;
            },

            render: function () {
                this.pluginsRepository.pluginStartedAndHasAtLeastOneProfile('Bugzilla', $.proxy(function () {
                    this.bugsRepository.getBugs([this._getBugId()], $.proxy(function (data) {
                        if (data.length > 0) {
                            var bug = data[0];
                            var wrapper = $(this.wrapperTemplate);
                            var rendered = $.tmpl(this.template, bug);
                            rendered.appendTo(wrapper.find('._bugInfoContent'));
                            wrapper.appendTo(this.placeholder);
                            wrapper.find('.bugzilla-external-link').click(function (e) {
                                e.preventDefault();
                                window.open($(this).attr('href'));
                            });
                        }
                    }, this));

                }, this));
            },

            _getBugId: function () {
                return new Tp.URL(location.href).getArgumentValue('BugID');
            }
        };

        return BugInfoViewer;
    });