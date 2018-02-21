tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/core/extension.base')
    .addModule('tp/search/models/model.search', function (_, ExtensionBase) {
        return ExtensionBase.extend({
            init: function (config) {
                this._super(config);
                var configurator = config.context.configurator;
                var searchService = configurator.service('search');

                var refreshTrigger = function () {
                    return this.fire('refresh');
                }.bind(this);

                searchService.onSubmit(refreshTrigger);
                this.destructor = function () {
                    return searchService.offSubmit(refreshTrigger);
                };
            },

            destroy: function () {
                this.destructor();
                this._super();
            },

            'bus afterInit': function (e) {
                var configurator = e.data.config.context.configurator;
                var searchService = configurator.service('search');

                var params = searchService.params().get();
                searchService.search(params).done(function (result) {
                    result.data.items = _.map(result.data.items, function (entity) {
                        var linkEntity = this._getLinkEntity(entity);
                        entity.url = configurator.getUrlBuilder().getNewViewUrl(
                            linkEntity.id, linkEntity.entityType.name, true);
                        return entity;
                    }.bind(this));

                    result.data.srcParams = params;
                    result.data.featuresService = configurator.getFeaturesService();

                    this.fire('dataBind', result.data);
                }.bind(this));
            },

            _getLinkEntity: function (entity) {
                switch (entity.entityType.name.toLowerCase()) {
                    case 'comment':
                        return entity.general;
                    case 'teststep':
                        return entity.testCase;
                    default:
                        return entity;
                }
            }
        });
    });
