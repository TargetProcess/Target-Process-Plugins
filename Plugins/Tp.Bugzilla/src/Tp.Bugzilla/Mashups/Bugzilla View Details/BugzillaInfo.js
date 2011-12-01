tau.mashups
    .addDependency("BugzillaViewDetails/BugInfoViewer")
    .addDependency("libs/jquery/jquery")
    .addMashup(function (viewer, $, config) {

        var placeholder = $('#' + config.placeholderId);
        new viewer({ placeholder: placeholder }).render();
    });