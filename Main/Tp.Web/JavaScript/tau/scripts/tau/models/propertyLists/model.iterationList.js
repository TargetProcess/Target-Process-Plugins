define(["Underscore","tau/models/propertyLists/model.propertyList.base","tau/configurator","tau/core/dates","libs/date.js/build/date"],function(a,b,c,d){return b.extend({name:"iteration-list",propertyType:"iteration",onInit:function(){var a=this,b=function(){a.store.get("project",{id:a.config.context.applicationContext.selectedProjects[0].id,nested:!0,fields:["id",{iterations:["id","name","startDate","endDate",{release:["id","name"]}]}]}).done({success:a.onDataRetrieved.tauCreateDelegate(a)})};b()},getRequiredFields:function(){return[{entityState:["id"]}]},"bus beforeSave":function(a){var b=a.data.cmd.fields=a.data.cmd.fields||[];b.push({release:["id","name"]})},processData:function(b){var e=this.config,f=e.context.applicationContext.culture.shortDateFormat,g=this.getCurrentEntityId(),h=[],i=e.mode||"short",j=null,k=null,l=c.getCurrentDate(),m=function(a){a.id!==g&&h.push(a)};i==="full"?j=m:j=function(a){var b=d.parse(a.startDate),c=d.parse(a.endDate);a.id!==g&&(b>=l||c>=l)&&h.push(a)},a.each(b.iterations,j),h.length==0&&(e.mode=="full",j=m,a.each(b.iterations,j));var n=[],o={};a(h).each(function(a){var b=o[a.release.id]||{id:a.release.id,name:a.release.name};b.items=b.items||[];var c=d.parse(a.startDate),e=d.parse(a.endDate),g=l>=c&&l<=e,h=a.name+(g?" (current)":"");g&&(k=a),b.items.push({id:a.id,name:h,description:"From "+c.format(f)+" to "+e.format(f)}),o[b.id]||(n.push(b),o[b.id]=b)});var p=b.iterations.length,q=g?p-1:p,r={nullableValue:g!==null&&this.propertyIsNotEmpty(),states:n,completed:0===h.length-q};k&&(r.defaultValue=k.id),this.fire("dataBind",r)}})})