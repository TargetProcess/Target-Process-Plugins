define(["Underscore","tau/models/dataProviders/model.provider.items.base"],function(_,Class){return Class.extend({fetch:function(fnCallback){this._fetch("testCaseRuns","testCaseRun","priority",fnCallback)},_createDetailCommand:function(collectionType){var detailCommand={};return detailCommand[collectionType]=["id","passed","executed","runDate",{testCases:["id","name","tags","numericPriority",{priority:["id","name","importance"]}]}],detailCommand},_convertData:function(data){return data=this._super(data),data=_.compact(data),_.sortBy(data,function(item){var priority=item.testcase.numericPriority||0;return priority})},_convertItem:function(data){var self=this;if(data.testCases.length==0)return null;var testcase=data.testCases[0];testcase.priority.kind=this.importanceProcessor.getKind(testcase.priority.importance);var item={id:data.id,__type:data.__type,name:testcase.name,testcase:{id:testcase.id,__type:testcase.__type,name:testcase.name,tags:this._processTags(testcase),priority:testcase.priority,numericPriority:testcase.numericPriority},passed:data.passed,runDate:data.runDate,executed:data.executed,status:data.passed===!0?"Passed":data.passed===!1?"Failed":"Skipped"};return item}})})