define(["jQuery","tau/ui/extensions/list/teamprojects/ui.extension.list.teamprojects.action.detach.base"],function(e,t){return t.extend({_detach:function(t){var i=this.config.context.configurator,o=i.getStore(),r=this.config.context.entity,n=e.Deferred();return o.removeFromList("program",{id:r.id,$set:{projects:{id:t.entityId}}}).done({success:function(){o.evictProperties(r.id,"program",["projects"]),o.get("program",{id:r.id,fields:["id",{projects:["id"]}]}).done(n.resolve.bind(n))}}),n.promise()}})});