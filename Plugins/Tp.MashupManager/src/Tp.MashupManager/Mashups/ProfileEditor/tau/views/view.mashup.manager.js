/*global tau*/
tau.mashups
    .addDependency('Underscore')
    .addDependency('tau/core/view-base')
    .addDependency('tau/components/component.container')
    .addDependency('tau/mashup.manager/configurations/configuration.mashup.manager')
    .addModule('tau/mashup.manager/views/view.mashup.manager',
        function(_, ViewBase, ComponentContainer, ConfigurationMashupManager) {
            return ViewBase.extend({
                initialize: function() {
                    //LEFT BLANK SINCE WORKFLOW CHANGED
                },

                'bus beforeInit': function() {
                    var configurator = this.config.context.configurator;
                    configurator.getTitleManager().setTitle('Mashup Manager');

                    var appConfig = this.config;
                    var containerConfig = _.extend(appConfig, (new ConfigurationMashupManager()).getConfig(appConfig));
                    appConfig.context.actionData = appConfig.actionData;

                    this.container = ComponentContainer.create({
                        name: 'mashup manager page container',

                        layout: containerConfig.layout,
                        template: containerConfig.template,

                        extensions: _.union([], containerConfig.extensions || []),
                        context: appConfig.context
                    });

                    this.container.on('afterInit', this['container afterInit'], this);
                    this.container.on('afterRender', this['container afterRender'], this);
                    this.container.on('componentsCreated', this['container componentsCreated'], this);

                    this.container.initialize(containerConfig);
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
                    if (this.container) {
                        this.container.destroy();
                        this.container = null;
                    }
                },

                destroy: function() {
                    this.destroyContainer();
                    this._super();
                }
            });
        }
    );
