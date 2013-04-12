tau.mashups
.addDependency("libs/jquery/jquery.tmpl")
.addDependency("tau/mashups/TpSearch/ProfileEditor/searcherProfileEditor")
.addMashup(function (jqueryTmpl, searcherProfileEditor, config) {
	new searcherProfileEditor({
		placeHolder: $('#' + config.placeholderId)
	}).render();
});