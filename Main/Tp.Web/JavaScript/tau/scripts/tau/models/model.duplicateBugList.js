define(["Underscore","tau/core/model-base"],function(_,ModelBase){var entityListModel=ModelBase.extend({_retrieveEntity:function(){var ctx=this.config.context,entityTypeName=ctx.entity.entityType.name,entityId=ctx.entity.id,cmdSettings={id:entityId,fields:["id"]};this.store.get(entityTypeName,cmdSettings,{scope:this,success:this.onEntityRetrieved}).done()},convertServerDataModelData:function(serverData){return{__type:serverData.__type,id:serverData.id,name:serverData.name,state:{name:serverData.entityState.name}}},onEntityRetrieved:function(entity){this.fire("registerStoreRequest"),this.fire("commitTransaction")},"bus duplicateBugRetrieved":function(evtArgs){var result=evtArgs.data.items,entities=_.map(result,this.convertServerDataModelData,this);entities=_.compact(entities);var initData={items:entities};this.bus.fire("dataBind",initData)},onInit:function(){if(this.config.disableModel)return;this._retrieveEntity()}});return entityListModel})