define(["jQuery","tests/common/testkit","tau/configurator","tau/components/component.property.build.testPlanRun","tau/components/component.finder"],function($,ComponentTestKit,configurator,Component){var run=function(){var TestCase=new ComponentTestKit("[component.property.build.testPlanRun]",Component),projects={p1:"Preved",p2:"Medved"},releases={r1:"Release",r2:"Release2",r3:"Release3"},iterations={i1:"Iter1",i2:"Iter2"},builds={b1:{name:"Build1",project:"p1",release:"r1"},b2:{name:"Build2",project:"p1",release:"r2",iteration:"i1"}},testPlanRuns={tpr1:{name:"TPR1",project:"p1",release:"r2",build:"b2"},tpr2:{name:"TPR2",project:"p1"},tpr3:{name:"TPR3",project:"p1",release:"r2",iteration:"i1"}},data=TestCase.loadFixtures({projects:projects,iterations:iterations,releases:releases,testPlanRuns:testPlanRuns,builds:builds});return TestCase.setData(data),TestCase.setEntity(data.testPlanRun.tpr1),TestCase.selectProject(data.project.p1),TestCase.addTest({name:"should render valid markup",test:function(){var $el=this.$el;equal($el.find(".tau-property__value span").text(),"Build2")}}),TestCase.addTest({name:"should change property",test:function(){var tpr=data.testPlanRun.tpr1;TestCase.getService().registerFindCommand({config:{$skip:0,$limit:20,fields:["id","name",{project:["id","abbreviation"]}],$query:{"Project.Id":data.project.p1.id,"Release.Id":data.release.r2.id}},returnedData:[data.build.b1,data.build.b2]});var $el=this.$el,$trigger=$el.find(".tau-property__value");$trigger.click();var $list=$(".tau-bubble");equal($list.size(),1,"Count of bubbles");var $options=$list.find(".tau-result-list-row");equals($options.length,2,"Options");var $filterField=$list.find(":text.filter-field");TestCase.getService().registerFindCommand({config:{$skip:0,$limit:20,fields:["id","name",{project:["id","abbreviation"]}],$query:{name:{$contains:"preved"},"Project.Id":data.project.p1.id,"Release.Id":data.release.r2.id}},returnedData:[]}),$filterField.val("preved"),$filterField.trigger(jQuery.Event("keyup",{keyCode:jQuery.ui.keyCode.ENTER})),TestCase.getService().verify()}}),TestCase.addTest({name:"should change property, correct find",context:{type:"testPlanRun",id:data.testPlanRun.tpr2.id},test:function(){var tpr=data.testPlanRun.tpr2;TestCase.getService().registerFindCommand({config:{$skip:0,$limit:20,fields:["id","name",{project:["id","abbreviation"]}],$query:{"Project.Id":data.project.p1.id,"Release.Id":null}},returnedData:[data.build.b1,data.build.b2]});var $el=this.$el,$trigger=$el.find(".tau-property__value");$trigger.click();var $list=$(".tau-bubble");equal($list.size(),1,"Count of bubbles");var $options=$list.find(".tau-result-list-row");equals($options.length,2,"Options");var $filterField=$list.find(":text.filter-field");TestCase.getService().registerFindCommand({config:{$skip:0,$limit:20,fields:["id","name",{project:["id","abbreviation"]}],$query:{name:{$contains:"preved"},"Project.Id":data.project.p1.id,"Release.Id":null}},returnedData:[]}),$filterField.val("preved"),$filterField.trigger(jQuery.Event("keyup",{keyCode:jQuery.ui.keyCode.ENTER})),TestCase.getService().verify()}}),TestCase.addTest({name:"should change property, correct find3",context:{type:"testPlanRun",id:data.testPlanRun.tpr3.id},test:function(){var tpr=data.testPlanRun.tpr3;TestCase.getService().registerFindCommand({config:{$skip:0,$limit:20,fields:["id","name",{project:["id","abbreviation"]}],$query:{"Project.Id":data.project.p1.id,"Release.Id":data.release.r2.id,"Iteration.Id":data.iteration.i1.id}},returnedData:[data.build.b1,data.build.b2]});var $el=this.$el,$trigger=$el.find(".tau-property__value");$trigger.click();var $list=$(".tau-bubble");equal($list.size(),1,"Count of bubbles");var $options=$list.find(".tau-result-list-row");equals($options.length,2,"Options");var $filterField=$list.find(":text.filter-field");TestCase.getService().registerFindCommand({config:{$skip:0,$limit:20,fields:["id","name",{project:["id","abbreviation"]}],$query:{name:{$contains:"preved"},"Project.Id":data.project.p1.id,"Release.Id":data.release.r2.id,"Iteration.Id":data.iteration.i1.id}},returnedData:[]}),$filterField.val("preved"),$filterField.trigger(jQuery.Event("keyup",{keyCode:jQuery.ui.keyCode.ENTER})),TestCase.getService().verify()}}),TestCase.addTest({name:"should refresh on every iteration-release change",context:{type:"testPlanRun",id:data.testPlanRun.tpr1.id},test:function(){var entity=data.testPlanRun.tpr1,$el=this.$el;equal($el.find(".tau-property__value span").text(),"Build2"),TestCase.getService().registerSaveCommand({config:{id:entity.id,$set:{release:{id:data.release.r2.id}},fields:["id",{release:["id","name"]},{iteration:["id","name"]},{build:["id","name"]}]},returnedData:{id:entity.id,release:data.release.r2,iteration:null,build:null}}),configurator.getStore().save("testplanrun",{id:entity.id,$set:{release:{id:data.release.r2.id}},fields:["id",{release:["id","name"]},{iteration:["id","name"]},{build:["id","name"]}]}).done(),$el=this.$el,equal($el.find(".tau-property__value").text(),"","Empty after refresh"),$el.find(".tau-property__value").text("bzzz"),TestCase.getService().registerSaveCommand({config:{id:entity.id,$set:{release:{id:data.release.r3.id}},fields:["id",{release:["id","name"]},{iteration:["id","name"]},{build:["id","name"]}]},returnedData:{id:entity.id,release:data.release.r3,iteration:null,build:null}}),configurator.getStore().save("testplanrun",{id:entity.id,$set:{release:{id:data.release.r3.id}},fields:["id",{release:["id","name"]},{iteration:["id","name"]},{build:["id","name"]}]}).done(),$el=this.$el,equal($el.find(".tau-property__value").text(),"","Still refresh"),TestCase.getService().verify()}}),TestCase.run()};return{run:run}})