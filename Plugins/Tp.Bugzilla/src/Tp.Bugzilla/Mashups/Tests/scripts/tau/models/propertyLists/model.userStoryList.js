define(["Underscore","tau/models/propertyLists/model.propertyList.base"],function(a,b){return b.extend({name:"userStory-list",propertyType:"userStory",onInit:function(){var a=this,b=function(){a.store.get("userStory",{fields:["id","name",{project:["id","name"]}]}).done({success:a.onDataRetrieved.tauCreateDelegate(a)})};b()},processData:function(b){var c=this.getCurrentEntityId(),d={};a.each(b,function(a){a.id!=c&&(d.hasOwnProperty(a.project.id)||(d[a.project.id]=a.project,d[a.project.id].items=[]),d[a.project.id].items.push(a))});var e={states:this.sortProjects(a.values(d)),completed:!0,nullableValue:c!==null&&this.config.context.entity.entityType.name.toLowerCase()!="task"&&this.propertyIsNotEmpty()};this.fire("dataBind",e)}})})