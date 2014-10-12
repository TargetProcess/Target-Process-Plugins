define(["Underscore","jQuery","tau/core/extension.base"],function(e,t,i){return i.extend({ANY_TEAM_OR_PROJECT:"Any",NO_TEAM_OR_PROJECT:"None","bus afterRender":function(e,t){this.fire("$el.ready",t.element)},"bus $el.ready":function(e,i){var a=this,s=i.find(".i-role-action-submit");a.fire("$submitButton.ready",s),s.on("click",function(){a.fire("filter.submitted")}),this.fire("$teamSection.ready",i.find(".tau-teams")),this.fire("$projectSection.ready",i.find(".tau-projects")),this.fire("$sections.ready",i.find(".tau-teams, .tau-projects")),this.fire("$assignSection.ready",i.find(".tau-teams-projects-updater")),i.on("click",".i-role-selecttrigger",function(e){var i=t(e.currentTarget).data(),s="team"==i.entityType?[i.entityId]:[a.ANY_TEAM_OR_PROJECT],r="project"==i.entityType?[i.entityId]:[a.ANY_TEAM_OR_PROJECT];a.fire("model.setFilter",{selectedTeamsIds:s,selectedProjectsIds:r})})},"bus $el.ready:last + filter.submitted":function(e,t){this.fire("model.setFilter",this.getCurrentState(t))},"bus $el.ready + $submitButton.ready":function(e,t,i){t.on("change",":checkbox",function(){i.hasClass("tau-teams-projects-submit_highlighted_true")||i.animateWithCss({cssClassName:"tau-teams-projects-submit_highlighted_true"})})},"bus configurator.ready:last + $teamSection.ready:last + team.add.success":function(e,t,i,a){var s=i.find(":checkbox[value="+a.id+"]").length;s||(this._addToList(i,t.getTemplateFactory().get("board.context.selector.teams.list"),a),i.removeClass("tau-managed-category_isempty_true"),this.fire("team.add.completed"))},"bus configurator.ready:last + $projectSection.ready:last + project.add.success":function(e,t,i,a){var s=i.find(":checkbox[value="+a.id+"]").length;s||(this._addToList(i,t.getTemplateFactory().get("board.context.selector.projects.list"),a),i.removeClass("tau-managed-category_isempty_true"),this.fire("project.add.completed"))},getCurrentState:function(e){var t={},i=this,a=function(){return this.value==i.NO_TEAM_OR_PROJECT?i.NO_TEAM_OR_PROJECT:parseInt(this.value,10)},s=e.find(".tau-teams .i-role-list :checked"),r=e.find(".tau-teams .i-role-list :checkbox");return t.selectedTeamsIds=!s.length||s.length>1&&s.length==r.length?[this.ANY_TEAM_OR_PROJECT]:s.map(a).toArray(),s=e.find(".tau-projects .i-role-list :checked"),r=e.find(".tau-projects .i-role-list :checkbox"),t.selectedProjectsIds=!s.length||s.length>1&&s.length==r.length?[this.ANY_TEAM_OR_PROJECT]:s.map(a).toArray(),t},_addToList:function(t,i,a){a.selected=!0,a.isSupportView=!0;var s=i.bind({},a),r=t.find(".i-role-list"),n=r.find(".i-role-item:first");n.length?n.after(s):r.append(s),r.parent().scrollTo(0,{duration:500}),s.addClass("tau-added"),e.delay(function(){s.removeClass("tau-added")},1500)}})});