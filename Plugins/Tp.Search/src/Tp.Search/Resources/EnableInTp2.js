require(['tau/core/tau', 'all.components'], function(utils) {
    if (window.location.href.indexOf('board.aspx') != -1) {
        return;
    }
    var ieVersion = utils.getIEVersion();
    if (ieVersion < 9 && ieVersion !== undefined) {
        return;
    }
    tau.mashups
        .addDependency('Searcher/SearchTP2')
        .addDependency('tau/service.container')
        .addMashup(function(processor, serviceContainerClass) {
            var parentConfigurator = new serviceContainerClass();
            parentConfigurator.setLoggedUser(window.loggedUser);
            processor.render(parentConfigurator);
        });
});