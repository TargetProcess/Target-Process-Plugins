define(["tau/core/templates-factory","tau/ui/templates/common/ui.template.user","tau/ui/templates/assignmentsList/ui.template.assignmentsList.addBtn"],function(a){var b={name:"assignments-user-list",markup:['<div class="group" roleId="${role.id}">','<table class="role-header">',"   <tr>",'   <td class="role-title">','   {{tmpl "assignments-add-btn"}}',"&nbsp;${role.name}","   </td>",'   <td class="role-effort">',"{{if roleEffort }}",'       <div class="effort">${roleEffort.effort}<span class="point">${$item.effortPoints}</span></div></td>',"{{/if}}",'<td class="role-effort">',"{{if roleEffort }}",'       <div class="remain">${roleEffort.remain}<span class="point">${$item.effortPoints}</span></div>',"{{/if}}","   </td>","</tr>","</table>",'{{tmpl(users) "user"}}',"</div>"].join(""),dependencies:["user","assignments-add-btn"]};return a.register(b)})