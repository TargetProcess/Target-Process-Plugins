define(["tau/core/templates-factory","tau/ui/templates/list_/grid/ui.template.list.grid.row"],function(a){var b={name:"list-grid-group",markup:['   <table>{{tmpl(items) "list-grid-row" }}</table>'],dependencies:["list-grid-row"]};return a.register(b)})