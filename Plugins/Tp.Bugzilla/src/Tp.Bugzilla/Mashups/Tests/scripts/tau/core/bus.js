define(["Underscore","tau/core/class","tau/core/bus.reg","tau/core/event"],function(a,b,c,d){function f(a,b){var c=b[1],d=b[0];a.name.match(/()/)&&console.log(a.name,"["+a.id+"]",":",d,c||"")}var e=b.extend({globalBus:null,init:function(b){b=b||{},this.globalBus=b&&b.globalBus?b.globalBus:null,this.name=b&&b.name?b.name:a.uniqueId()+"",a.defaults(b,{id:a.uniqueId("bus_")}),this.id=b.id,c.onCreate(this)},initialize:function(a){this.fire("initialize",a)},destroy:function(){this.fire("destroy"),delete this.globalBus},getGlobalBus:function(){return this.globalBus}});d.implementOn(e.prototype),e.prototype.fire=function(a,b,c){this.globalBus&&this.globalBus.fire(a,b,this);return d.prototype.fire.apply(this,arguments)};return e})