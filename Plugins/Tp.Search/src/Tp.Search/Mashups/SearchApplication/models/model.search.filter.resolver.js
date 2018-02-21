tau.mashups
    .addDependency('jQuery')
    .addDependency('tau-intl')
    .addDependency('Underscore')
    .addDependency('tau/services/generalConversion')
    .addDependency('tau/const/entityType.names')
    .addModule('tp/search/models/model.search.filter.resolver',
        function ($, intl, _, generalConversionService, entityTypeNames) {

            function getGeneral(store, generalId) {
                return store
                    .getDef(entityTypeNames.GENERAL, {
                        failureGlobal: false,
                        id: generalId,
                        fields: ['id', {entityType: ['id', 'name']}],
                        errorHandler: function (error) {
                            var status = error.data.status;
                            // if there is no entity with such id (404 Not Found response)
                            // or specified id can't be parsed (400 Bad Request response),
                            // then just ignore the error.
                            var shouldCancelPropagation = status !== 404 && status !== 400;
                            return shouldCancelPropagation;
                        }
                    });
            }

            function resolveGeneral(store, general, $tokenResolver) {
                if (general.entityType.name.toLowerCase() === entityTypeNames.PROJECT) {
                    store
                        .getDef(entityTypeNames.PROJECT, {id: general.id, fields: ['id', 'name']})
                        .then(function () {
                            return $tokenResolver.resolve({id: general.id, entity: general.entityType.name});
                        })
                        .fail(function () {
                            return $tokenResolver.resolve({id: null, entity: 'search'});
                        });
                } else {
                    $tokenResolver.resolve({id: general.id, entity: general.entityType.name});
                }
            }

            function tryResolveConverted(store, generalId, $tokenResolver) {
                var generalTypeName = entityTypeNames.GENERAL;
                generalConversionService.getGeneralConversion(store, generalId, generalTypeName)
                    .then(function (conversion) {
                        generalConversionService.trackGetGeneralConversionResult('search not found entity id',
                            generalId, generalTypeName, conversion);
                        if (conversion) {
                            var actual = conversion.actualGeneral;
                            $tokenResolver.resolve({id: actual.id, entity: actual.entityType.name});
                        } else {
                            $tokenResolver.resolve({id: null, entity: 'search'});
                        }
                    });
            }

            return {
                resolveSearchToken: function (searchToken, configurator) {
                    var $tokenResolver = $.Deferred();
                    var store = configurator.getStore();
                    var idMatch = searchToken.match(/^#?(\d+)$/);
                    if (idMatch) {
                        var generalId = parseInt(idMatch[1], 10);
                        getGeneral(store, generalId)
                            .then(function (general) {
                                return resolveGeneral(store, general, $tokenResolver);
                            })
                            .fail(function () {
                                return tryResolveConverted(store, generalId, $tokenResolver);
                            });
                    } else if (/^(?:\*\d+\*|\*\d+|\d+\*)$/.test(searchToken) || searchToken.length > 2) {
                        $tokenResolver.resolve({id: null, entity: 'search'});
                    } else {
                        $tokenResolver.reject(_.escape(intl.formatMessage('Enter more than 2 characters')));
                    }

                    return $tokenResolver;
                }
            };
        });
