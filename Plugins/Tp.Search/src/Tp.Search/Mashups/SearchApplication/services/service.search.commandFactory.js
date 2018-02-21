tau.mashups
    .addDependency('tp/search/services/service.search.command.multiIds')
    .addDependency('tp/search/services/service.search.command.multiIdsParts')
    .addDependency('tp/search/services/service.search.command.text')
    .addModule('tp/search/services/service.search.commandFactory',
        function (SearchMultiIdsCommand, SearchMultiIdsPartsCommand, SearchTextCommand) {
            var serviceSearchCommandFactory = {
                getSuitableCommand: function (searchString) {
                    if (SearchMultiIdsCommand.isSuitableCommand(searchString)) {
                        return SearchMultiIdsCommand;
                    } else if (SearchMultiIdsPartsCommand.isSuitableCommand(searchString)) {
                        return SearchMultiIdsPartsCommand;
                    } else {
                        return SearchTextCommand;
                    }
                }
            };

            return serviceSearchCommandFactory;
        });
