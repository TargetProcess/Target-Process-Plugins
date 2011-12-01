define(["tau/components/component.tabsHeader","tau/utils/utils.routing","tests/components/common/common.setup","tests/components/component.specs","tests/components/component1","tests/components/component2"],function(a,b,c,d){var e=function(){var b=function(){return{context:{type:"bug",id:1},settings:{children:[{path:"tests/components/component1"},{path:"tests/components/component2",selected:!0}]}}},e=[{name:"should has child controls",test:function(){var a=this.$el;equals(a.find(".component1").length,1,"Component 1 child control is added"),ok(!a.find(".component1").parent().hasClass("selected"),"Component 1 is not selected"),equals(a.find(".component2").length,1,"Component 2 child control is added"),ok(a.find(".component2").parent().hasClass("selected"),"Component 2 is selected")}}],f=e.concat([{name:"should select tab on click",test:function(){var a=this.$el,b=a.find(".component1").parent(),c=a.find(".component2").parent(),d=!1,e=-1;this.component.on("tabSelected",function(a){var b=a.data;d=!0,e=b.index}),ok(!b.hasClass("selected"),"Component 1 is not selected before click"),ok(c.hasClass("selected"),"Component 2 is selected before click"),b.click(),ok(b.hasClass("selected"),"Component 1 is selected on click"),ok(!c.hasClass("selected"),"Component 2 is not selected on click"),ok(d,'The "tabSelected" event is fired on click'),equals(e,0,"Selected tab index"),e=-1,a.find(".entity-tabs").click(),equals(e,-1,"Selected tab is not changed")}}]),g=c.create("[component.tabsHeader]",{},a);d.create(g,b()).viewShouldFollowBasicComponentLifeCycle().viewShouldFollowContainerComponentLifeCycle().viewShouldPassTests(f).done()};return{run:e}})