tau.mashups
	.addDependency("tp/plugins/pluginButtonRemover")
	.addMashup(function (RemoverClass) {
		var remover = new RemoverClass('Mashup Manager');
		remover.hideDeleteButton();
	});