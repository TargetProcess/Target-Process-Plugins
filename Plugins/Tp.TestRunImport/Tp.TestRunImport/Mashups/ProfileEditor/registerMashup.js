tau.mashups
	.addDependency("tp/plugins/commandGateway")
	.addDependency("tp/plugins/profileNameSource")
	.addDependency("TestRunImport/testRunImportProfileEditor")
	.addDependency("tp/plugins/profileRepository")
	.addMashup(
	function (commandGateway, profileNameSource, testRunImportProfileEditor, profileRepository, config) {    
	    new testRunImportProfileEditor({
	        config: config,
	        placeHolder: $('#' + config.placeholderId),
	        profileRepository: profileRepository,
	        commandGateway: new commandGateway(),
	        profileNameSource: profileNameSource
	    }).render();
	});
