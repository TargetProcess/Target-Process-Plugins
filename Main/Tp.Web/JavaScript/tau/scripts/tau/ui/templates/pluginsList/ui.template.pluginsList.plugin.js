define(["tau/core/templates-factory","tau/ui/templates/pluginsList/ui.template.pluginsList.profile"],function(a){var b={name:"pluginsList-plugin",markup:['<div class="plugin-block">','   <img src="${iconSrc}" />','   <h4 class="_pluginName">${name}</h4>','   <p class="note-p">${description}</p>','   <div class="separator"></div>','   <div class="profiles-list">','       <div class="tau-profiles-short">','           {{tmpl(profilesShort) "pluginsList-profile"}}',"       </div>",'       <div class="tau-profiles-more" style="display:none">','           {{tmpl(profilesMore) "pluginsList-profile"}}',"       </div>","   </div>",'   <div class="pb-5 tau-profiles-moreLink" style="display:none">','       <a class="more-link">Show more (${profilesMore.length})</a>',"   </div>",'   <div class="pt-5">','       <button type="button" class="button bold" onClick="window.location.href = \'${newProfileUrl}\';">Add Profile</button>',"   </div>","</div>"],dependencies:["pluginsList-profile"]};return a.register(b)})