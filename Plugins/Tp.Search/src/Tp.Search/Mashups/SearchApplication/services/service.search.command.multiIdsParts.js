tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/core/class')
    .addDependency('tp/search/services/service.search.command.multiIds')
    .addDependency('tau/libs/store2/store2')
    .addModule('tp/search/services/service.search.command.multiIdsParts',
        function (_, Class, ServiceSearchMultiIdsCommand, Store2) {
            var ServiceSearchMultiIdPartsCommand = ServiceSearchMultiIdsCommand.extend({
                _parseSearchString: function () {
                    var parts = this.params.searchString.split(',');
                    if (parts.length > 0) {
                        var idParts = [];
                        _.forEach(parts, function (part) {
                            var token = _.trim(part);
                            if (/^\*\d+\*$/.test(token)) {
                                idParts.push({value: token.replace(/\*/g, ''), apiMethod: 'contains'});
                            } else if (/^\*\d+$/.test(token)) {
                                idParts.push({value: token.replace(/^\*/, ''), apiMethod: 'endsWith'});
                            } else if (/^\d+\*$/.test(token)) {
                                idParts.push({value: token.replace(/\*$/, ''), apiMethod: 'startsWith'});
                            }
                        });
                    }
                    return {
                        idParts: idParts
                    };
                },
                _searchItemsPromise: function () {
                    var store2 = new Store2(this.configurator);
                    var whereClause = _.reduce(this.searchParameters.idParts, function (whereClauseMemo, idPart) {
                        if (whereClauseMemo) {
                            whereClauseMemo += ' or ';
                        }
                        whereClauseMemo += 'convert.toString(Id.Value).' +
                            idPart.apiMethod + '("' + idPart.value + '")';
                        return whereClauseMemo;
                    }, '');

                    return store2.findAll('general?&select={id, {entityType.id, entityType.name} as entityType}' +
                        '&where=(' + whereClause + ')&orderBy=id&take=1000&skip=0');
                },
                _resultFilter: function (item) {
                    return item.hasOwnProperty('entityType');
                }
            });

            ServiceSearchMultiIdPartsCommand.isSuitableCommand = function (searchString) {
                var parts = searchString.split(',');
                if (parts.length > 0) {
                    return _.every(parts, function (part) {
                        var token = _.trim(part);
                        var isIdPart = /^(?:\*\d+\*|\*\d+|\d+\*)$/.test(token);
                        return isIdPart;
                    });
                }
                return false;
            };

            return ServiceSearchMultiIdPartsCommand;
        });
