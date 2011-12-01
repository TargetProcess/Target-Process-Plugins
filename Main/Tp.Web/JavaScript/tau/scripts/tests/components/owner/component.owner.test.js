define(["jQuery","tau/components/component.property.owner","tests/components/component.specs","tests/common/testData.Generator","tests/common/service.mock","tau/configurator","tests/components/common/common.setup"],function(a,b,c,d,e,f,g){var h=function(){function h(a){return[a.firstName,a.lastName].join(" ")}var i=new d;i.initDefaultData();var j=i.getData(),k=i.getBugs()[0],l={context:{type:"bug",id:k.id}},m=k.owner,n="/TP/Avatar.ashx?UserId="+m.id+"&size=32",o=h(m),p=i.getUsers()[5],q=h(p),r=g.create("[component.owner]",j,b,l),s=[{name:"should render valid markup",test:function(){var a=this.$el;equal(a.find(".user-avatar-container .user-avatar").attr("src"),n,"Avatar src"),equal(a.find(".user-info .user-name").text(),o,"User name")}},{name:"should change owner",preSetup:function(){var a=this.service=new e;f.setService(a)},test:function(){var b=this.$el,c=b.find(".user-info .user-name");c.click();var d=a(".tau-bubble");equal(d.size(),1,"Count of bubbles");var e=d.eq(0).find('.user-name:contains("'+q+'")');this.service.registerSaveCommand({config:{$set:{owner:{id:p.id}},fields:["id",{owner:["id","firstName","lastName"]}],id:k.id},returnedData:{id:k.id,owner:p}}),e.click(),equal(this.$el.find(".user-info .user-name").text(),q,"Owner was changed")}}];c.create(r,l).viewShouldFollowBasicComponentLifeCycle().viewShouldPassTests(s).done()};return{run:h}})