define(["tau/configurations/baseAssignable.container.config","tau/models/model.extensions","tau/components/extensions/requestsList/extensions.requestList.labelRefresher","tau/components/extensions/entity/extension.requestList.remover"],function(a,b,c,d){var e=a.extend({getAdditionalInfoAliases:function(){return["Owner","User Story","CreationDate","CompletionDate"]},getActionsAliases:function(){return["Add Time","Add Impediment","Attach to Request","Convert","-----","Old View","Old Edit","Print","-----","Delete"]},getPanels:function(e){var f=a.prototype.getPanels.call(this,e);f.push({type:"collapsible",collapsed:!0,header:[{type:"label",text:this.getNames("request"),getBadgeText:b.calculateRequestsCount,badgeFieldName:"count",extensions:[c]}],items:[{type:"requestList"}],extensions:[d]});return f}});return e})