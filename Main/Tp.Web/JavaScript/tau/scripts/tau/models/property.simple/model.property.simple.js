define(["tau/core/model-base"],function(ModelBase){return ModelBase.extend({"bus beforeInit":function(evt,initConfig){var configurator=initConfig.config.context.configurator,entity=initConfig.config.context.entity,fieldName=initConfig.config.editorComponentConfig.name;this.attachToChangePropertiesOfCurrentEntity(fieldName);var self=this;configurator.getStore().get(entity.entityType.name,{id:entity.id,fields:[fieldName]}).done({success:function(r){var data=r[0].data;self.fire("dataBind",{name:fieldName,value:data[fieldName],output:data[fieldName]})}})}})})