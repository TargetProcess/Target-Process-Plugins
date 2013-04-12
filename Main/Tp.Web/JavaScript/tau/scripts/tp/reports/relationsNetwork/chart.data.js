define(["Underscore","tp/reports/dataRepository","tp/reports/settings/definitionBuilder"],function(_,DataRepository,definitionBuilder){function pipe(promises,callback){$.when.apply(null,promises).pipe(function(){var args=arguments,responses=_.map(promises,function(promise,i){return args[i][0]||args[0]});callback(responses)})}var cardsDetailsRepository=new DataRepository({url:"/slice/v1/matrix/cardsDetails"});return{fetch:function(items,acid,callback){function select(property){return function(d){return d[property]}}function typeGroup(d){return definitionBuilder.getTypeGroup(d.type)}function type(d){return d.type.toLowerCase()}function buildDefinitionItemsPart(type){return{id:type,data:"{"+definitionBuilder.cell.data(type)+"}",ordering:null}}function buildDefinition(items){var cardsIds=_.chain(items).map(select("ids")).flatten().value();return{CardsIds:cardsIds,base64:!0,take:cardsIds.length,where:null,definition:{global:{acid:acid},x:null,y:null,cells:{items:_.chain(items).map(select("type")).map(buildDefinitionItemsPart).value()}}}}var promises=_.chain(items).groupBy(type).map(function(data,type){return{type:type,ids:_.chain(data).map(select("id")).value()}}).groupBy(typeGroup).filter(function(items,group){return group!="undefined"}).map(buildDefinition).map(function(definition){return $.extend($.Deferred(),{definition:definition})}).value();pipe(promises,function(responses){var fetched=_.chain(responses).map(select("items")).flatten().map(select("data")).value();callback(fetched)}),_.each(promises,function(promise){cardsDetailsRepository.get(promise.definition,promise.resolve)})}}})