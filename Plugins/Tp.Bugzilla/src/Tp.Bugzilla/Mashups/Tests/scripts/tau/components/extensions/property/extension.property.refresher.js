define(["Underscore","tau/components/extensions/component.extension.base"],function(a,b){return b.extend({category:"refresher","bus prepareChanges":function(a){this.bus.fire("markElementToBeUpdated",{element:this.element})},"bus applyChanges":function(a){var b=this;this.bus.on("afterRender",function(a){a.removeListener(),b.bus.fire("updateElement",{element:b.element})}),this.bus.fire("refresh")},"bus propertyChanged":function(){this.fire("refresh")},"bus beforeChangeProperty":function(){this.fire("markElementToBeUpdated",{element:this.element})}})})