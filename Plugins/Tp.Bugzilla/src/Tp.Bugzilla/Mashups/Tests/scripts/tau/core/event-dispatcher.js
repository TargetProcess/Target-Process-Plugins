define(["Underscore","tau/core/class"],function(a,b){var c=b.extend({init:function(a){this.components=a,this.eventArgsQueue={},this.dictionaryQueue={}},listen:function(b,c){var d=this;d.dictionaryQueue.hasOwnProperty(b)||(d.dictionaryQueue[b]=a(d.components).clone(),d.eventArgsQueue[b]=[]);var e=function(b){var e=b.sender;b.removeListener();var f=a(d.components).indexOf(e);d.eventArgsQueue[b.name][f]=b.data,d.dictionaryQueue[b.name]=a(d.dictionaryQueue[b.name]).without(e);if(a.isEmpty(d.dictionaryQueue[b.name])){var g={eventName:b.name,argsArr:d.eventArgsQueue[b.name]};c(g)}};a(d.components).each(function(a){a.on(b,e,null,{},1e3)})}});return c})