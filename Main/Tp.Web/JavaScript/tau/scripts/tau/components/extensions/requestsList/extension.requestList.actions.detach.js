define(["jQuery","tau/components/extensions/component.extension.base"],function($,ExtensionBase){return ExtensionBase.extend({category:"edit","bus dataBind+afterRender":function(evt){var self=this,data=evt.dataBind.data,$element=evt.afterRender.data.element;if(data.items&&data.items.length){var $cell=$('<td class="action"></td>'),$actionDetach=$('<span class="action-detach button small">Detach</span>');$actionDetach.click($.proxy(self.onActionDetach,self)),$cell.append($actionDetach);var $rows=$element.find("table.base-info-list tbody > tr");$rows.append($cell)}},onActionDetach:function(evt){var self=this,generalId=self.config.context.entity.id,requestId=$(evt.target).parent().parent().data().tmplItem.data.id,actionsService=self.config.context.configurator.getActionsService();actionsService.detachRequest({generalId:generalId,requestId:requestId,success:$.proxy(self.onRequestDetachedSuccess,self)})},onRequestDetachedSuccess:function(){var self=this;self.fire("evictProperty");var generalId=self.config.context.entity.id;self.fire("requestIsDetachedFromEntity",{entity:{id:generalId}})}})})