define(["jQuery","Underscore","tau/components/extensions/component.extension.base"],function(e,t,a){return a.extend({resetLifecycle:!0,init:function(){var e=this;e._super.apply(e,arguments),e._updateMetaInfoDebouncedHandler=t.debounce(e.updateMetaInfoAsync,500)},updateMetaInfoAsync:function(e,a,i,s){var n=this;n._request&&n._request.reject({data:{status:0}});var d=t.pluck(e.x.data,"x"),r=t.pluck(e.y.data,"y"),o={$whereX:d.length?{x:{$in:d}}:void 0,$whereY:r.length?{y:{$in:r}}:void 0};(o.$whereX||o.$whereY)&&(n._request=a.axesCounts(o).done(function(e){n._request=null,s.updateAxesMetaInfo(e.data.items,i),n.fire("view.axes.counters.updated")}))},"bus boardSettings.ready":function(e,a){a=a.boardSettings;var i=t.bind(function(e){this.fire("boardSettings.limitsReady",e.limits||{})},this);a.get({fields:["limits"],listener:this,callback:i}),a.bind({fields:["limits"],listener:this,callback:i})},"bus model.sliceInfo.retrieved + slice.ready + domWrapper.ready + boardSettings.limitsReady":function(e,t,a,i,s){this._updateMetaInfoDebouncedHandler(t,a.slice,s,i)},"bus boardSettings.limitsReady:last > model.sliceInfo.retrieved:last > slice.ready:last > domWrapper.ready:last >  view.card.batch.move.completed":function(e,t,a,i,s){this._updateMetaInfoDebouncedHandler(a,i.slice,t,s)},"bus boardSettings.limitsReady:last > model.sliceInfo.retrieved:last > slice.ready:last > domWrapper.ready:last > boardSettings.limitsReady:last > operation.execute.done":function(e,t,a,i,s){this._updateMetaInfoDebouncedHandler(a,i.slice,t,s)},"bus model.sliceInfo.retrieved:last > slice.ready:last > domWrapper.ready:last > boardSettings.limitsReady":function(e,t,a,i,s){this._updateMetaInfoDebouncedHandler(t,a.slice,s,i)}})});