define(["tau/core/templates-factory"],function(a){var b={name:"customField-entity",markup:['<div class="ui-customfield">',"   <table>","   <tbody>","   <tr>",'       <td class="ui-customfield__label">${name}</td>','       <td class="ui-customfield__value"><h2 class="bundle bundle-${value.kind.toLowerCase()}">${value.kind.toLowerCase()}</h2><a href="${value.url}">${value.name}</a></td>',"   </tr>","   </tbody>","   </table>","</div>"]};return a.register(b)})