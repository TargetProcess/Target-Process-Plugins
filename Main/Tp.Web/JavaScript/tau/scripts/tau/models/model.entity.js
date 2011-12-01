define(["Underscore","tau/configurator","tau/core/model-base","tau/models/dataprocessor/model.processor.context"],function(a,b,c,d){var e=function(){this.callbacks={}};e.prototype={register:function(a,b){this.callbacks[a]=b},fnCallback:function(b){var c=this;return function(){var d=Array.prototype.slice.apply(arguments);c._ready.data[b]=d.length===1?d[0]:d;var e=c.callbacks[b];e&&e.fn.apply(e.scope,d),delete c.callbacks[b],a(c.callbacks).keys().length===0&&c._ready.fn.call(c._ready.scope,c._ready.data)}},ready:function(a){this._ready={fn:a.fn,scope:a.scope,data:{}}}};return c.extend({name:"Entity",onInit:function(){var a=this,c=a.config.context.id;a.store.on({eventName:"afterRemove",type:a.config.context.entityType.name,listener:this},function(b){var d=b.data.cmd.config.id;c==d&&a.fire("deleted",b.data)}),a.store.on({eventName:"beforeRemove",type:a.config.context.entityType.name,listener:this},function(b){var d=b.data.cmd.config.id;c==d&&a.fire("beforeDelete")});var d=new e;d.register("onGlobalSettings"),d.register("onContext"),d.ready({scope:this,fn:this.fnBindData});var f=b.getGlobalSettingsService();f.getGlobalSettings({scope:d,success:d.fnCallback("onGlobalSettings")}),a.store.get("context",{id:c,fields:["id","acid","culture",{loggedUser:["id","email","isAdministrator"]},{processes:["id","name","terms","practices","customFields"]},{selectedProjects:["id","name",{process:["id","name"]}]}]},{scope:d,success:d.fnCallback("onContext")}).done()},fnBindData:function(a){var b=a.onContext;b.data.globalSettings=a.onGlobalSettings,this.onContextRetrieved(b)},onContextRetrieved:function(b){var c={context:{}},e=this,f=e.config.context;a.extend(c.context,{assignable:f}),a.extend(c.context,{entity:f}),a.extend(c.context,{general:f}),a.extend(c.context,{applicationContext:b.data}),e.bus.fire("contextRetrieved",{context:d(c.context)})}})})