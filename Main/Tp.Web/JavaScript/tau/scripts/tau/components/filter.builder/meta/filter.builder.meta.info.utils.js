define(["require","Underscore","react","jsx!../editors/text","jsx!../editors/boolean","jsx!../editors/autocomplete","./entity.filter.transformers"],function(e){var t=e("Underscore"),n=e("react"),r=e("jsx!../editors/text"),i=e("jsx!../editors/boolean"),o=e("jsx!../editors/autocomplete"),a=e("./entity.filter.transformers");return{canAddField:{onlyOnce:function(e){return t.isEmpty(e())},always:t.constant(!0)},createDefaultFieldStrategy:function(e,n){return e=e||[],function(r){var i=t.reduce(e,function(e,t){var n=r.findFieldDefinition(t);return n&&e.push(n),e},[]);return i.length?i:(i=r.getAllFieldDefinitions(),i.length<=n?i:[])}},createTextField:function(e,t){return{name:e,label:t,transformer:a.text,getEditor:function(e){return n.createElement(r,e)},canAddFilter:this.canAddField.always}},createToggleField:function(e,r){return{name:e,label:r,transformer:a.boolean,getEditor:function(e){return n.createElement(i,e)},canAddFilter:this.canAddField.onlyOnce,getDefaultValue:t.constant(!0)}},createAutocompleteField:function(e,r,i){return{name:r,label:i,transformer:a.object,getEditor:function(r){return n.createElement(o,t.extend(r,{suggestionService:e}))},canAddFilter:this.canAddField.always}}}});