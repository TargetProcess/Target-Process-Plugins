tau.mashups
    .addCSS('ghf_marked.css')
    .addDependency('tau/mashup.manager/application.mashup.manager')    
    .addMashup(function (ApplicationMashupManager, config) {
        new ApplicationMashupManager('#' + config.placeholderId);        
    });
