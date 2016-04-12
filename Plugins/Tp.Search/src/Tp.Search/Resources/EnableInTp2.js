tau.mashups
    .addDependency('Searcher/SearchTP2')
    .addDependency('tau/service.container')
    .addMashup(function(processor, serviceContainerClass) {
        var parentConfigurator = new serviceContainerClass();
        parentConfigurator.setLoggedUser(window.loggedUser);
        processor.render(parentConfigurator);
    });
