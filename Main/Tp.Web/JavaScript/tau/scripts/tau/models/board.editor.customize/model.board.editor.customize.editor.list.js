define(["Underscore","tau/models/board.editor.customize/model.board.editor.customize.editor","tau/core/termProcessor","tau/ui/extensions/board.plus/ui.board.plus.utils","tau/utils/utils.customFields.list"],function(e,t,n,r,i){return t.extend({refreshLayout:function(t,s){s.configurator=t;var a=new n,o=r.orderListUnits(s.cardLayout);s.cardLayout=e.map(o,function(t){var n=e.clone(t.unit),r=e.intersection(s.types,e.chain(n.settings).pluck("types").flatten().value()),o=i.getListHeader(void 0!==n.header?n.header:n.name);return n.settings={classes:["dnd_source","maximized"],isDesignMode:!0,metaData:{id:t.id,rank:t.rank,title:n.name,header:o,alignment:t.alignment,typesNames:e.map(r,function(e){return{"long":a.getTerm(e,"names"),"short":a.getTerm(e,"iconSmall"),typeId:e}}),showTypes:s.types.length>r.length}},t.unit=n,t}),this.fire("refreshData",s)}})});