define(["require","Underscore","tau/services/customize/unitEditors/unitEditorFactory"],function(t){var i=t("Underscore"),n=t("tau/services/customize/unitEditors/unitEditorFactory");return{openUnitEditor:function(t,r){return function(e,o){var u=new n(o.configurator,o.hostBus,o.unitEditorEventHandler),a=i.pick(e,["cardDataForUnit","unit","$unit","$card"]);return i.extend(a,{unitEditorType:t,unitEditorOptions:i.deepClone(r)}),u.createEditorView(a)}},isUnitEditable:function(t,n){if(!n.data)return!1;var r=t.interactionConfig;return r?Boolean(i.isFunction(r.isEditable)?r.isEditable(n):r.isEditable):!1}}});