define(["Underscore","jQuery","tau/components/extensions/component.extension.base"],function(a,b,c){return c.extend({"bus beforeInit":function(a){var b=a.data.config;this.options={emptyDataMessage:b.emptyDataMessage||"No data",contentSelector:b.contentSelector||null,emptyElementCssClass:b.emptyElementCssClass||"ui-empty-data-msg"}},"bus dataBind+afterRender":function(a){this.element=a.afterRender.data.element,a.dataBind.data.items.length===0&&this._applyToggleNoData(!0)},"bus hideListContent":function(a){this._applyToggleNoData(!0)},"bus showListContent":function(a){this._applyToggleNoData(!1)},_applyToggleNoData:function(a){this._toggleNoData(a)},_getVisibilityMethodName:function(a){return a?"show":"hide"},_toggleNoData:function(a){var b=this._getVisibilityMethodName(!a),c=this._getVisibilityMethodName(a),d=this.element,e=this.options;d.find(e.contentSelector)[b](),this._getNoDataMessageElement(d,e)[c]()},_getNoDataMessageElement:function(a,c){var d=a.find("."+c.emptyElementCssClass);d.length===0&&(d=b(['<span class="',c.emptyElementCssClass,'">',c.emptyDataMessage,"</span>"].join("")).appendTo(a));return d}})})