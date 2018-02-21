tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/core/class')
    .addDependency('tau/utils/utils.date')
    .addDependency('tau/ui/ui.templateFactory')
    .addModule('tp/search/services/service.search.resultHandler', function (_, Class, dateUtils, templateFactory) {
        var ServiceSearchResultHandler = Class.extend({
            init: function () {
                this.preProcessingStrategies = {
                    'comment': function (item) {
                        item.entityType = {
                            name: 'comment'
                        };

                        var tp = templateFactory.getTermProcessor();
                        item.general.entityType.term = tp.getTerm(item.general.entityType.name);
                        return item;
                    },
                    'teststep': function (item) {
                        item.entityType = {
                            name: 'testStep'
                        };
                        var tp = templateFactory.getTermProcessor();
                        item.testCase.entityType.term = tp.getTerm(item.testCase.entityType.name);
                        return item;
                    }
                };
            },
            handle: function ($searchDeferredChain, $resultDeferred, searchString) {
                $searchDeferredChain
                    .fail(function (r) {
                        $resultDeferred.reject({
                            data: {
                                error: true,
                                response: r
                            }
                        });
                    })
                    .done(_.bind(function (r) {
                        var ss = _.trim(searchString).replace(/(\d+)/g, ' $1 ');
                        var keywords = _.without(ss ? ss.split(/\s|\?|\*|\+|-|:|'|"/g) : [], '');
                        keywords = _(keywords)
                            .chain()
                            .filter(function (k) {
                                return k && (/^\d+$/.test(k) || k.length > 1);
                            })
                            .uniq()
                            .value();

                        $resultDeferred.resolve({
                            data: {
                                keywords: keywords,
                                info: r.info,
                                items: _.map(r.items, _.bind(this._adaptItem, this))
                            }
                        });
                    }, this));
            },
            _adaptItem: function (item) {
                var t = item.__type.toLowerCase();

                var preProcessing = this.preProcessingStrategies[t] || _.identity;
                item = preProcessing(item);

                var tp = templateFactory.getTermProcessor();
                item.entityType.term = tp.getTerm(item.entityType.name);
                item.nameExt = item.nameExt || item.name;
                item.descExt = item.descExt || item.description || '';
                item.tags = item.tags ? item.tags.split(', ') : [];
                item.createDate = dateUtils.parse(item.createDate);
                return item;
            }
        });

        return ServiceSearchResultHandler;
    });
