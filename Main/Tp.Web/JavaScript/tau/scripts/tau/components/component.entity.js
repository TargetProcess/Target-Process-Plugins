define(["tau/components/component.creator","tau/models/model.entity","tau/views/view.entity","tau/components/extensions/storeFreeze.extension","tau/components/extensions/practice.convert.extension","tau/components/extensions/error/extension.errorBar","tau/components/extensions/extension.storeErrorEmiter","tau/ui/extensions/ui.extension.loading","tau/components/extensions/extension.refresher","tau/components/extensions/entity/extension.refresher.project"],function(a,b,c,d,e,f,g,h,i,j){return{create:function(k,l){l=l||{};var m=l.extensions||[];m.push(d),m.push(e),m.push(f),m.push(g),m.push(h),m.push(i),m.push(j);var n={name:"entity component",ModelType:b,ViewType:c,extensions:m};return a.create(n,k)}}})