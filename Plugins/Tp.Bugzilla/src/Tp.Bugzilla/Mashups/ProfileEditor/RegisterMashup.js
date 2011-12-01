tau.mashups
    .addDependency("Bugzilla/ui.widgets")
    .addDependency("Bugzilla/BugzillaInitializer")
    .addMashup(function (globalAnimation, initializer, config) {
        globalAnimation.prototype.turnedOn = true;

        var placeholder = $('#' + config.placeholderId);
        new initializer({ placeholder: placeholder, mashupPath: config.mashupPath }).initialize();
    });