tau.mashups
    .addDependency('tp/plugins/vcs/viewDiff/viewDiffController')
    .addDependency('libs/jquery/jquery')
    .addMashup(function (ViewDiffController, $, config) {
        new ViewDiffController({ pluginName: 'Subversion', placeholder: $('#' + config.placeholderId) });
    });
