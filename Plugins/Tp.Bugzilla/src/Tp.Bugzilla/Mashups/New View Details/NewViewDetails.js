tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('BugzillaViewDetails/NewBugInfoTemplate')
    .addModule('BugzillaViewDetails/NewViewDetails', function ($, _, template) {
        function NewView() {
        }

        NewView.prototype = {
            render: function (bugView, bugsRepository, pluginsRepository) {
                var bug = null;

                bugView.addBlock('Bugzilla Info',
                    function (contentElement, context) {
                        if (bug) {
                            var rendered = $.tmpl(template, bug);
                            rendered.appendTo($(contentElement));
                        }
                    },
                    $.noop,
                    {
                        getViewIsSuitablePromiseCallback: function (context) {
                            var viewIsSuitableDeferred = $.Deferred();

                            pluginsRepository.pluginStartedAndHasAtLeastOneProfile('Bugzilla', function () {
                                bugsRepository.getBugs([context.entity.id], function (data) {
                                    var isSuitable = data.length > 0;
                                    if (isSuitable) {
                                        bug = data[0];
                                    }
                                    viewIsSuitableDeferred.resolve(isSuitable);
                                }.bind(this));

                            }.bind(this),
                                viewIsSuitableDeferred.reject
                            );

                            return viewIsSuitableDeferred.promise();
                        },
                        positionToInsert: 6
                    });
            }
        };

        return NewView;
    });