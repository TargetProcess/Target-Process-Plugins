define(["tau/core/templates-factory"],function(a){var b={name:"cell-name",markup:['   <span class="tau-title" title="${name}">${name}</span>',"{{if tags.length}}",'   <span class="tau-tags">${tags[0]}</span>','   {{if tags.length > 1}}<span class="tau-tags">&#133;</span>{{/if}}',"{{/if}}"]};return a.register(b)})