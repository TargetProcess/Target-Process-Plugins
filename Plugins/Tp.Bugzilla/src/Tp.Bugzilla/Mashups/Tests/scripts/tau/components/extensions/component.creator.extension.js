define(["Underscore","tau/components/extensions/component.extension.base"],function(a,b){var c=b.extend({getDependencies:function(b){var c=[];a(b).each(function(a){if(a.path)c.push(a.path);else{if(!a.type)throw"Configuration error: type or path is missing";var b="tau/components/component."+a.type;c.push(b)}});return c},childrenLoadedCallback:function(b,c){var d=this,e=[];a(b).each(function(a){a.name||(a.name="",a.type&&(a.name=a.type),a.id&&(a.name+=":"+a.id));var b=a.component.create(a);d.fire("componentCreated",{component:b}),e.push({config:a,component:b})}),d.fire("componentsCreated",e,c)},createDependenciesLoadedCallback:function(b,c){var d=this,e=b.components,f=b.context;return function(){var b=arguments;a(e).each(function(a,c){a.component=b[c],a.context=f}),d.childrenLoadedCallback(e,c)}},"bus createComponents":function(b){var c=b.data,d=b.caller,e=c.components;e=c.components=a.isArray(e)?e:[e];var f=this.getDependencies(e);require(f,this.createDependenciesLoadedCallback(c,d))}});return c})