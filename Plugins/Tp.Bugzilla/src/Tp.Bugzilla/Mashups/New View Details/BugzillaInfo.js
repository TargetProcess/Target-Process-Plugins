tau.mashups
    .addDependency('jQuery')
    .addDependency('tp/bug/view')
    .addDependency('tp/plugins/bugzilla/bugsRepository')
    .addDependency('tp/plugins/pluginsRepository')
    .addDependency('BugzillaViewDetails/NewViewDetails')
    .addMashup(function ($, bugView, bugsRepository, pluginsRepository, Viewer) {
        new Viewer().render(bugView, bugsRepository, pluginsRepository);
    });