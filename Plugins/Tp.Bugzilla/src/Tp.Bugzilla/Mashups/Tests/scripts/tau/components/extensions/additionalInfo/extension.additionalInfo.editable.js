define(["jQuery","tau/components/extensions/component.extension.base"],function(a,b){return b.extend({category:"edit",editableIndexes:null,"bus beforeComponentsInitialize":function(a){this.editableIndexes=[],this.createComponentList(a.data),this.attachListeners()},"bus afterRender":function(a){this.$el=a.data.element,this.addEditableBehaviour()},"bus refresh":function(){this.clearComponent()},createComponentList:function(a){this.components=[];for(var b=1;b<a.length;b+=2)this.components.push(a[b].component)},addEditableBehaviour:function(){for(var a=0;a<this.editableIndexes.length;a++)this.doEditableRow(this.editableIndexes[a])},attachListeners:function(){var a=this.components;if(a)for(var b=0;b<a.length;b++)a[b].on("permissionsReady",this.onPermissionsReady,this,{index:b})},removeListeners:function(){var a=this.components;if(a)for(var b=0;b<a.length;b++)a[b].removeListener("permissionsReady",this.onPermissionsReady,this)},onRowClick:function(a){a.stopPropagation(),a.data.component.fire("edit")},doEditableRow:function(b){var c=this.$el.find(" > tbody > tr").eq(b);c.bind("click",{component:this.components[b]},a.proxy(this.onRowClick,this)),c.addClass("ui-additionalinfo_editable_true")},onPermissionsReady:function(a){if(a.data.editable){var b=a.listenerData.index;this.$el?this.doEditableRow(b):this.editableIndexes.push(b)}},clearComponent:function(){this.removeListeners(),delete this.components,delete this.editableIndexes,delete this.$el},destroy:function(){this.clearComponent(),this._super()}})})