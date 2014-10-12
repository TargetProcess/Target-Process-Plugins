define(["tau/core/class","tau/models/page.entity/entity.context.factory","tau/models/choice/model.choice.copy.to.project","tau/ui/extensions/choice/ui.extension.choice.copy.to.project","tau/ui/templates/choice/project/ui.template.choice.project.option","tau/core/termProcessor"],function(e,o,t,n,i,c){return e.extend({createCopyToProjectBubbleConfig:function(e){var o=e.applicationContext,s=e.subscribeToComponentEvents,r=e.navigateToEntityDetails,a=e.eventBus,p=new c(o.getTerms());return{destroyExistingBubble:!0,onPositionConfig:this._getPositionConfig,onArrowPositionConfig:this._getArrowPositionConfig,applyBubbleClasses:!1,closeOnStoreActions:!1,className:"tau-lookup-styled-drop-down i-role-card-context-menu-copyToProject",stackName:"entity.menu.copy.to.project",showEvent:"__show__empty",hideEvent:"__hide__empty",cleanupOnHide:!0,zIndex:99999,context:o,componentsConfig:{components:[{type:"choice",filter:!0,filterPlaceholder:"Search Project",templateOption:i.name,model:t,extensions:[n],handleComponentEvents:function(e,o){e("afterSave",function(e,t){this._onCopyCompleted(a,o,p,r,t)}.bind(this)),e("saveFailed",function(e,o){a.fire("error",o)}),s&&s(e)}.bind(this)}]}}},_onCopyCompleted:function(e,o,t,n,i){o.destroyBubble();var c=t.getTerm(i.entityType),s=i.selectedProjectData,r=s?s.projectName:"",a=["<h3>Success!</h3>","<p>",c,' copied to project "',r,'"</p>'].join("");e.fire("notification",{id:"entity.copied.to.project",$node:$(a),type:"success",delay:5e3,disableAutoClose:!1}),n&&n(i)}})});