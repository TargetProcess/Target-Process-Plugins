require(['all.components'], function () {
    if (window.location.href.indexOf('board.aspx') != -1) return;
    if ($.browser.msie && parseInt($.browser.version, 10) < 9){
        return;
    }
    tau.mashups
        .addDependency('Searcher/SearchTP2')
        .addDependency('tau/service.container')
        .addMashup(function (processor, serviceContainerClass) {
            var parentConfigurator = new serviceContainerClass();
            parentConfigurator.setLoggedUser(window.loggedUser);
            processor.render(parentConfigurator);
        });
});