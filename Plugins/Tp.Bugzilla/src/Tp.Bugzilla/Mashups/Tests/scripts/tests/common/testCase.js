define(["Underscore","tau/configurator","tau/core/class","tests/common/service.mock"],function(a,b,c,d){return c.extend({init:function(a){this.name=a},initModule:function(c,e,f,g){module(this.name,{setup:function(){this.initialData=c.data||[],this.serviceMock=new d,b.clear(),b.setApplicationPath(c.applicationPath||"/TP"),b.setCurrentDate(c.currentDate||new Date(1307600546192)),b.setService(this.serviceMock),b.setInitialData(this.initialData),f&&f();var g=a.clone(c);this.component=e.create(g),this.component.on("afterRender",function(a){this.element=a.data.element},this),this.component.initialize(c.componentConfig)},teardown:function(){b.clear(),g&&g(),this.component.destroy(),delete this.initialData}})},test:function(a,b){test(a,b)}})})