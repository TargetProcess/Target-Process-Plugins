define(["Underscore","tau/models/propertyLists/model.propertyList.base"],function(a,b){return b.extend({name:"priority-list-model",propertyType:"priority",onInit:function(){var a=this,b=function(b){var c=b[0].data;a.assignable=c,a.store.get(a.propertyType,a.createGetPrioritiesCommand()).done({success:a.onDataRetrieved.tauCreateDelegate(a)})};this.getAssignable(b)},getRequiredFields:function(){return[{entityType:["id"]}]},createGetPrioritiesCommand:function(){return{fields:["id","name","importance",{entityType:["id"]}]}},processData:function(b){var c=this.getCurrentEntityId(),d=this.assignable.entityType.id,e=[];a.each(b,function(a){a.id!==c&&a.entityType.id===d&&e.push({id:a.id,name:a.name,importance:a.importance||0})}),e.sort(function(a,b){return a.importance-b.importance}),this.fire("dataBind",{states:e,completed:!0,nullableValue:!1})}})})