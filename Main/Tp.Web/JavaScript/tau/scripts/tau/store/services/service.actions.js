define(["jQuery","Underscore","tau/core/class"],function($,_,Class){var ActionsService=Class.extend({init:function(config){this.config=config},getActionUrl:function(action){return this.config.appPath+"/PageServices/ActionsService.asmx/"+action},getActiveProjectForEntity:function(generalId,success,error){var data={generalId:generalId};this.sendRequest("GetActiveProjectForEntity",data,success,error)},copyGeneralToProject:function(generalId,projectId,success,error){var data={generalId:generalId,projectId:projectId};this.sendRequest("CopyGeneralToProject",data,success,error)},getProjectIds:function(success,error){var data={};this.sendRequest("GetProjectIds",data,success,error)},moveGeneralToProject:function(generalId,projectId,success,error){var data={generalId:generalId,projectId:projectId};this.sendRequest("MoveGeneralToProject",data,success,error)},attachGeneralToRequest:function(generalId,requestId,success,error){var data={generalId:generalId,requestId:requestId};this.sendRequest("AttachGeneralToRequest",data,success,error)},detachRequest:function(operationConfig){var noop=function(){},data={generalId:operationConfig.generalId,requestId:operationConfig.requestId};operationConfig=_(operationConfig).defaults({success:noop,error:noop}),this.sendRequest("DetachRequestFromGeneral",data,operationConfig.success,operationConfig.error)},getEntityTypesToConvert:function(entityId){return this.sendRequestDef("GeneralCanConvertTo",{generalId:entityId}).pipe(function(result){return _.map(result.d,function(v){return{id:v.ID,name:v.EntityTypeName}})})},convertGeneralToTask:function(generalId,userStoryId,success,error){var data={generalId:generalId,userStoryId:userStoryId};this.sendRequest("ConvertGeneralToTask",data,success,error)},convertGeneralToType:function(generalId,entityTypeId,success,error){var data={generalId:generalId,entityTypeId:entityTypeId};this.sendRequest("ConvertGeneralToType",data,success,error)},splitUserStory:function(generalId,success,error){var data={generalId:generalId};this.sendRequest("SplitUserStory",data,success,error)},sendRequest:function(actionName,data,success,error){$.ajax(this.getActionUrl(actionName),{contentType:"application/json",type:"POST",data:JSON.stringify(data),success:success,error:error})},sendRequestDef:function(actionName,data){return $.ajax({url:this.getActionUrl(actionName),contentType:"application/json",type:"POST",data:JSON.stringify(data)})}});return ActionsService})