define(["Underscore","jQuery","tau/components/extensions/component.extension.base"],function(a,b,c){return c.extend({category:"edit","bus permissionsReady+afterRender":function(a){var b=this,c=a.afterRender.data.data,d=this.permissions=a.permissionsReady.data;if(d.editable){var e=this.$el=a.afterRender.data.element,f=e.find(".property-text");b.$widget=f,f.editableText({restoreText:!1,onSave:function(a){var c={};c[b.config.propertyName]=a;var d={$set:c};d.$include=[b.config.propertyName],b.bus.fire("save",d)}})}},"bus edit":function(){this.permissions.editable&&this.$el.find(".property-text").editableText("activate")}})})