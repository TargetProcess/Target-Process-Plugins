tau.mashups
    .addDependency('tp/plugins/vcs/viewDiff/viewDiffController')
    .addDependency('libs/jquery/jquery')
    .addCSS("../../tau/scripts/tp/codemirror/lib/codemirror.css")
    .addCSS("../../tau/scripts/tp/codemirror/theme/default.css")
    .addMashup(function (ViewDiffController, $, config) {
        new ViewDiffController({ pluginName: 'Mercurial', placeholder: $('#' + config.placeholderId) });
    });
