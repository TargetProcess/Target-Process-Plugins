define(["jQuery","tau/components/extensions/component.extension.base","tau/ui/templates/list_/ui.template.list"],function(a,b,c){return b.extend({"bus afterRender":function(b){var c=this,d=b.data.element;d.delegate("[role=title]","click",function(b){var d=a(b.target),e=d.parents("[role=group]:first"),f=e.next("[role=list-inner]");e.toggleClass("tau-list__group__collapse_collapsed_true"),f.toggleClass("tau-list__group__list_collapsed_true"),c.bus.fire("stateChanged")})}})})