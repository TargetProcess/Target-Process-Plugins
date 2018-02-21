// intlScope:search
tau.mashups
    .addDependency('Underscore')
    .addDependency('tau-intl')
    .addDependency('tau/components/component.container')
    .addDependency('tp/search/configurations/config.search.container')
    .addDependency('tau/core/view-base')
    .addModule('tp/search/views/view.page.search', function (_, intl, containerComponent, FactoryConfig, ViewBase) {
        return ViewBase.extend({
            init: function(config) {
                this._super(config);
            },

            initialize: function() {
                //LEFT BLANK SINCE WORKFLOW CHANGED
            },

            'bus beforeInit': function() {
                var self = this;

                var configurator = self.config.context.configurator;
                configurator.getTitleManager().setTitle(_.escape(intl.formatMessage('Search')));
                var history = configurator.getHistory();
                history.reset();
                history.setCurrent({
                    id: 0,
                    url: '#query/' + encodeURIComponent(self.config.entity),
                    title: _.escape(intl.formatMessage('Back to search results'))
                });

                var config = self.config;
                var containerConfig = _.extend(config, (new FactoryConfig(config)).getConfig(config));

                var c = self.container = containerComponent.create({
                    name: 'board page container',

                    layout: containerConfig.layout,
                    template: containerConfig.template,

                    extensions: _.union([], containerConfig.extensions || []),
                    context: config.context
                });

                c.on('afterInit', self['container afterInit'], self);
                c.on('afterRender', self['container afterRender'], self);
                c.on('componentsCreated', self['container componentsCreated'], self);

                c.initialize(containerConfig);
            },

            'container afterInit': function() {
                this.fireAfterInit();
            },

            'container componentsCreated': function(evt) {
                this.fire(evt.name, evt.data);
            },

            'container afterRender': function(evt) {
                this.fireBeforeRender();
                this.element = evt.data.element;
                this.fireAfterRender();
            },

            lifeCycleCleanUp: function() {
                this.destroyContainer();
                this._super();
            },

            destroyContainer: function() {
                if (!this.container) {
                    return;
                }

                this.container.destroy();
                this.container = null;
            },

            destroy: function() {
                this.destroyContainer();
                this._super();
            }
        });
    });
