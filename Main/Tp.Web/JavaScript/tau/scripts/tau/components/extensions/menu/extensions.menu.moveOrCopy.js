define(["tau/components/extensions/component.extension.base","tau/services/service.actions","tau/core/templates-factory","tau/ui/templates/actions/ui.template.moveOrCopy"],function(a,b,c){var d=0;return a.extend({"bus moveOrCopy":function(a){var b=a.data.generalID;this.moveOrCopy(b)},moveOrCopy:function(a){var e=this;Ext.WindowMgr.zseed=99999;var f=new b,g=function(b){function i(b,c){var d=$("#"+h+" #projectSelector").val();d!=0&&f[b](e.config.context.entity.id,d,function(b){j.close(),c&&c(a,d)},function(){j.close()})}d++;var g=c.get("move-or-copy").bind(e.config,{projects:b.d}).appendTo("body"),h="copyActionPopup"+d;g.find("#copyActionPopup").prop("id",h),g.find(".copy").click(function(){i("copyGeneralToProject")}),g.find(".move").click(function(){var a=function(a,b){e.fire("entityWasMovedToProject",{entity:{id:a}},{project:{id:b}})};i("moveGeneralToProject",a)});var j=showModalPopup(h,{title:"Move or Copy",width:440,height:126})};f.getActiveProjectForEntity(a,g)}})})