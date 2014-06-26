(function(){
    require.config({
        paths: {
            'tau/mashup.manager': '../../Mashups/Mashup Manager ProfileEditor/tau'
        }
    })
})();
tau.mashups
    .addCSS('../../tau/scripts/tp/codemirror/lib/codemirror.css')
    .addCSS('../../tau/scripts/tp/codemirror/theme/default.css')
    .addCSS('ghf_marked.css')
    .addDependency('tau/mashup.manager/application.mashup.manager')    
    .addMashup(function (ApplicationMashupManager, config) {
        new ApplicationMashupManager('#' + config.placeholderId);        
    });