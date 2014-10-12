define(["jQuery","Underscore","tau/ui/extensions/board.plus/ui.extension.board.plus.full-counts.base","tau/models/board.plus/model.counts"],function(t,e,n,o){var i=function(t,e,n){t[e]=t[e]?t[e].concat(n):[].concat(n)},s=function(t,e){return t=t||"null",e=e||"null",t+"-"+e};return n.extend({init:function(e){this._super(e),this.countsModel=Object.create(o),this.boardModel={},this.on("refresh",function(){this.countsModel&&this.countsModel.clear(),this.boardModel={}}.bind(this)),this.domWrapper=t.Deferred(),this.on("domWrapper.ready",function(e,n){this.domWrapper=t.Deferred().resolve(n)}.bind(this)),this.countsModel.on("update",function(t,e){this.domWrapper.done(function(t){t.updateAxesMetaInfo(e.counts,e.limits),this.fire("view.axes.counters.updated")}.bind(this))}.bind(this))},postUpdateAxesMetaInfo:function(t,e){this.countsModel&&this.countsModel.update({counts:t,limits:e})},updateCount:function(t){var n=this.countsModel.get(),o=function(t){return{count:1,type:t}}.bind(this),i=function(t,i,s,d){var u=e.find(n.counts,function(e){return e[i]==t});if(u){var a=e.find(u.counts,function(t){return t.type.toLowerCase()==d.toLowerCase()});a?a.count+=s:u.counts.push(o(d))}else if("null"!==t){var c={counts:[o(d)]};c[i]=t,n.counts.push(c)}},s=function(t,n){e.each(t,function(t,o){o=o.split("-"),e.each(t,function(t){i(o[0],"x",n,t.type),i(o[1],"y",n,t.type)})})};s(t.added,1),s(t.removed,-1),this.countsModel.update(n)},updateModel:function(t,n){var o=e.reduce(n,function(t,e){return t.push(s(e.x,e.y)),t},[]),d={removed:{},added:{}};return e.each(n,function(n){var u=s(n.x,n.y),a=t[u]||{},c=n.dataItem,r=c.id,l=a[r];l||(t[u]=a,i(d.added,u,c),t[u][r]=c),e.each(t,function(t,n){t[r]&&(e.contains(o,n)||(i(d.removed,n,c),delete t[r]))})},this),d},processBatchComet:function(t){return e.reduce(t,function(t,e){return e.location&&"card"===e.location.toLowerCase()&&t.push({dataItem:e.data,x:e.x,y:e.y}),t},[])},"bus comet.batch":function(t,n){var o={removed:{},added:{}};e.each(n.items,function(t){var n=this.updateModel(this.boardModel,this.processBatchComet(t.items));e.each(n.removed,function(t,e){i(o.removed,e,t)}),e.each(n.added,function(t,e){i(o.added,e,t)})},this),this.updateCount(o)},"bus boardSettings.limitsReady:last > model.sliceInfo.retrieved:last > slice.ready:last > domWrapper.ready:last > after.slice.cells.updated":function(t,e,n,o,i){this._updateMetaInfoDebouncedHandler(n,o.slice,e,i)},"bus slice.ready":function(t,n){n.base.on("after.cells",function(t,n){e.each(n.result&&n.result.items||[],function(t){var n=s(t.x,t.y);this.boardModel[n]=this.boardModel[n]||{},e.each(t.dynamic.items||[],function(t){this.boardModel[n][t.id]=t},this)},this),this.fire("after.slice.cells.updated")}.bind(this));var o=function(t,n){e.each(n.result.items,function(t){this.updateModel(this.boardModel,t.items)},this)}.bind(this);n.full.on("after.moveBatch",o),n.full.on("after.moveAndPrioritizeBatch",o)}})});