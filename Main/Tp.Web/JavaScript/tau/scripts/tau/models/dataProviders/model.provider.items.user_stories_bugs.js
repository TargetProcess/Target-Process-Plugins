define(["Underscore","tau/models/dataProviders/model.provider.items.base","tau/models/dataProviders/model.provider.items.user_stories","tau/models/dataProviders/model.provider.items.bugs"],function(a,b,c,d){return b.extend({init:function(b){this.config=b,this.providers=[new c(b)];var e=a.pluck(this.config.context.applicationContext.processes[0].practices,"name");a.indexOf(e,"Bug Tracking")!==-1&&this.providers.push(new d(b))},_convertData:function(a){return this._calculateEffortToMaximum(a)},fetch:function(b){var c=this,d=function(a){var d=c._convertData(a);b(d)},e=[];a.forEach(c.providers,function(a){e.push(function(b){a.fetch(function(a){b(null,a)})})}),a.parallel(e,function(b,c){var e=a.flatten(c);d(e)})}})})