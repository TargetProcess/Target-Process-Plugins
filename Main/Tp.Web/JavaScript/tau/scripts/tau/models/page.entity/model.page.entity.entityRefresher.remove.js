define(["Underscore","tau/core/extension.base"],function(_,ExtensionBase){return ExtensionBase.extend({"bus entity.ready + configurator.ready":function(evt,entity,configurator){var store=configurator.getStore(),fire=_.bind(this.fire,this);store.unbind(this),store.on({eventName:"beforeRemove",type:entity.entityType.name,listener:this},function(evtArgs){var deletedEntityId=evtArgs.data.cmd.config.id;entity.id==deletedEntityId&&fire("beforeDelete")}),store.on({eventName:"afterRemove",type:entity.entityType.name,listener:this},function(evtArgs){var e=evtArgs.data,deletedEntityId=e.cmd?e.cmd.config.id:e.obj?e.obj.id:e.id;entity.id==deletedEntityId&&fire("deleted",evtArgs.data)})},"bus configurator.ready > destroy":function(evt,configurator){configurator.getStore().unbind(this)}})})