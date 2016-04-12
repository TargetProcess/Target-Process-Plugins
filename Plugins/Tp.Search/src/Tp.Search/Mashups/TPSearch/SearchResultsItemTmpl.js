tau.mashups
.addModule("tau/mashups/TPSearch/SearchResultsItemTmpl", function () {
	return '<br><div>' +
	'<b>{{if General }}Comment for ${General.EntityType.Name} - #${General.Id} {{html General.Name}}{{else}}${EntityType.Name} - #${Id} {{html Name}}{{/if}}</b>' +
	'<div>{{html Description}}</div>' +
	'</div>';
});
