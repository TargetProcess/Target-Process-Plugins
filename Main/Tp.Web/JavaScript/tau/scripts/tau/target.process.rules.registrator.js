define(["Underscore","tau/core/types.targetprocess"],function(a,b){return{typeContainSimpleField:function(b,c){return a.find(b.simpleFields,function(a){return a===c})},registerRules:function(c){this.registeredEntityTypes={},this.rules=c,a.each(b.getDictionary(),this.registerType,this)},registerType:function(b){var c=this.registeredEntityTypes,d=this.rules;if(!c.hasOwnProperty(b.name)){c[b.name]=!0,a.each(b.aliases,function(a){c[a]=!0});var e="endDate";b.refs.hasOwnProperty("entityState")&&this.typeContainSimpleField(b,e)&&d.register({type:b.name,changes:["entityState"],fields:["endDate"]})}}}})