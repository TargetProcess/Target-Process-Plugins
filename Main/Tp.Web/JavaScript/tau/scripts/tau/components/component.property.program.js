define(["tau/components/component.property"],function(PropertyComponent){return{create:function(config){return config.propertyName="program",config.showUrl=!1,config.alignElementSelector=".property-text",config.editorComponentConfig={type:"state-list",listType:"program",expandable:!0,filter:!0,showEmptyDataMessage:!0,defaultValueLabel:"current",fullModeLabel:"show old",clearValueLabel:"reset",showReset:!0},PropertyComponent.create(config)}}})