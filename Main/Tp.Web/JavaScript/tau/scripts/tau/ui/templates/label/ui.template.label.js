define(["tau/core/templates-factory","tau/ui/tags/ui.tag.implode"],function(a,b){var c={name:"label",tags:[b],markup:['<div><span class="ui-label{{if cssClass}} ${cssClass}{{/if}}">${$item.$tau.text_implode(text)}</span></div>'],dependencies:[]};return a.register(c)})