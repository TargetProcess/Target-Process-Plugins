define(["tau/components/component.creator","tau/core/model-base","tau/models/customField/model.customField.dropdown.editable","tau/ui/extensions/stateList/ui.extension.stateList.editable","tau/ui/templates/state/ui.template.state-list"],function(a,b,c,d,e){return{create:function(f){var g={ModelType:b,extensions:[c,d],template:e};return a.create(g,f)}}})