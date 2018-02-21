tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('tau/components/component.creator')
    .addDependency('tau/core/extension.base')
    .addDependency('tp/search/models/model.search')
    .addDependency('tp/search/templates/template.search')
    .addDependency('tau/ui/behaviour/common/ui.behaviour.progressIndicator')
    .addModule('tp/search/components/component.search',
        function ($, _, creator, ExtensionBase, ModelType, template, ProgressIndicator) {
            return {
                create: function (config) {

                    config = config || {};

                    var creatorConfig = {
                        extensions: [
                            ModelType,
                            ExtensionBase.extend({

                                'bus afterRender': function (e, render) {
                                    var $el = render.element;
                                    ProgressIndicator.get($el).hide();
                                },

                                'bus afterRender > refresh': function (e, render) {
                                    var $el = render.element;
                                    $el.css('opacity', 0.5);
                                    ProgressIndicator.get($el).show();
                                },

                                'bus afterInit + afterRender': function (e, init, render) {
                                    var configurator = init.config.context.configurator;
                                    var ssi = configurator.service('search');
                                    var $node = render.element;
                                    $node.on('click', '.i-role-paging-container .i-role-paging-prev', function (e) {
                                        e.preventDefault();
                                        if ($(e.target).hasClass('tau-disabled')) {
                                            return;
                                        }
                                        var currPageNo = ssi.params().get('pageNo');
                                        ssi.params().set('pageNo', --currPageNo).submit();
                                    });

                                    $node.on('click', '.i-role-paging-container .i-role-paging-next', function (e) {
                                        e.preventDefault();
                                        if ($(e.target).hasClass('tau-disabled')) {
                                            return;
                                        }
                                        var currPageNo = ssi.params().get('pageNo');
                                        ssi.params().set('pageNo', ++currPageNo).submit();
                                    });

                                    $node.on('click', '.i-role-paging-container .i-role-paging-pageno a', function (e) {
                                        e.preventDefault();
                                        var targetPage = $(e.target).data('page');
                                        ssi.params().set('pageNo', targetPage).submit();
                                    });

                                    $node.on('click', '.i-role-search-result-item', function (e) {
                                        var entityId = $(e.currentTarget).data('entity-id');
                                        var entityData = _.findWhere(render.data.items, {id: entityId});

                                        if (entityData) {
                                            taus.track({
                                                action: 'global search / result item click',
                                                tags: ['global search click'],
                                                sessionId: ssi.params().get('sessionId'),
                                                itemId: entityData.id,
                                                itemName: entityData.name,
                                                itemTags: entityData.tags,
                                                itemDescription: entityData.description,
                                                '@': ['--exclude-board-context']
                                            });
                                        }
                                    });
                                }
                            })
                        ],
                        template: config.template || template
                    };

                    return creator.create(creatorConfig, config);
                }
            };
        });
