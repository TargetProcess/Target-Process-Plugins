define(["tau/components/component.creator","tau/models/model.menu.testPlanRun","tau/ui/templates/menu.testPlanRun/ui.template.menu.testPlanRun"],function(creator,Model,template){return{create:function(config){var creatorConfig={extensions:[Model],template:config.template||template};return creator.create(creatorConfig,config)}}})