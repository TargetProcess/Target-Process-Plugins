define(["tau/core/model.editable.base","tau/core/dates"],function(a,b){return a.extend({init:function(a){this._super.apply(this,arguments);if(!a.propertyName)throw"PropertyName was not configured";this.propertyName=a.propertyName},getEntityTypeName:function(){return this.config.context.entity.entityType.name},getEntityId:function(){return this.config.context.entity.id},onInit:function(){this.attachToChangePropertiesOfCurrentEntity(this.propertyName),this.store.get(this.getEntityTypeName(),{id:this.getEntityId(),fields:["id",this.propertyName]}).done({success:this.onEntityRetrieved,scope:this})},onEntityRetrieved:function(a){var c=b.parse(this.property=a[0].data[this.propertyName]),d=c?c.format(this.config.context.applicationContext.culture.shortDateFormat):"",e={text:d};this.fire("dataBind",e)}})})