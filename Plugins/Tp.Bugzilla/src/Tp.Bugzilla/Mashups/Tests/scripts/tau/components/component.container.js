define(["tau/components/component.creator","tau/views/view.container","tau/components/extensions/component.creator.extension","tau/ui/extensions/layout/ui.extension.layout","tau/ui/extensions/spinner/ui.extension.spinner"],function(a,b,c,d,e){return{create:function(f){var g={extensions:[c,d,e],ViewType:b};return a.create(g,f)}}})