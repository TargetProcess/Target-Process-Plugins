define(["require","jQuery","Underscore","tau/components/extensions/component.extension.base"],function(e){var t=e("jQuery"),n=e("Underscore"),i=e("tau/components/extensions/component.extension.base");return i.extend({"bus afterInit":function(e,t){var i=t.config.context,s=n.deepClone(i.access);s.canAddTeams=!1,s.canAddProjects=!1,s.showSearch=!0,s.submitLabel="Share",s.minProjectsCount=0,this.fire("dataBind",s)},"bus afterRender":function(e,t){this.element=t.element;var n=this.element,i=n.find(".i-role-section-teams"),s=n.find(".i-role-section-projects");n.on("click",".i-role-item :checkbox, .i-role-select-all-item :checkbox",function(){var e=this._getProjectsAndTeams(i),t=this._getProjectsAndTeams(s);this.fire("item.access.context.selection.changed",{teamIds:e,projectIds:t})}.bind(this)),this.fire("$el.ready",n),this.fire("$teamSection.ready",i),this.fire("$projectSection.ready",s),this.fire("$sections.ready",n.find(".tau-teams, .tau-projects"))},"bus menu.item.access.context.update":function(e,t){var n=!t.isActive;this.element.toggleClass("tau-disabled",n),this.element.find("input,button").prop("disabled",n),this._updateCheckboxes(this.element.find(".i-role-section-teams"),t.teamIds),this._updateCheckboxes(this.element.find(".i-role-section-projects"),t.projectIds)},_updateCheckboxes:function(e,i){e.find(".i-role-item :checkbox").each(function(e,s){t(s).prop("checked",n.contains(i,parseInt(s.value,10)))})},_getProjectsAndTeams:function(e){function t(e){return parseInt(e.value,10)}return n.map(e.find(".i-role-list :checked"),t)}})});