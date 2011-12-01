define(["jQuery","tau/components/component.property.release","tests/components/component.specs","tests/common/testData.Generator","tests/common/service.mock","tau/configurator","tests/components/common/common.setup"],function(a,b,c,d,e,f,g){var h=function(){var h=new d;h.initDefaultData();var i=h.getData(),j=h.getProjects(),k=h.getBugs()[0],l=h.getReleases(),m={manualContext:!0,context:{entity:{entityType:{name:k.__type},id:k.id},applicationContext:{selectedProjects:[j[0]],culture:{name:"en-US",timePattern:"g:i A",shortDateFormat:"M/d/yyyy",longDateFormat:"dddd, MMMM dd, yyyy",decimalSeparator:".",__type:"culture"},processes:h.getProcesses()}}},n=k.release,o=l[3],p=g.create("[component.release]",i,b,m),q=[{name:"should render valid markup",test:function(){var a=this.$el;equal(a.find(".property-text").text(),n.name,"Release name"),equal(a.find(".external-view").attr("href"),["/TP/View.aspx?id=",n.id].join(""),"Release name")}},{name:"should change release",preSetup:function(){var a=this.service=new e;f.setService(a)},test:function(){var b=this.$el,c=b;c.click();var d=a(".tau-bubble");equal(d.size(),1,"Count of bubbles");var e=d.eq(0).find('.drop-down-option:contains("'+o.name+'")');equal(d.find(".action-link.clear").size(),1,"Backlog button is available"),this.service.registerSaveCommand({config:{$set:{release:{id:o.id}},fields:["id",{iteration:["id","name"]}],id:k.id},returnedData:{id:k.id,release:o}}),equal(d.find(".action-link.more").text(),"show old","Show old is  available"),equal(d.find(".action-link.clear").text(),"backlog","Backlog button is  available"),e.click(),equal(this.$el.find(".property-text").text(),o.name,"Release was changed")}}];c.create(p,m).viewShouldFollowBasicComponentLifeCycle().viewShouldPassTests(q).done();var r=h.getBugs()[1],s={manualContext:!0,context:{entity:{entityType:{name:r.__type},id:r.id},applicationContext:{selectedProjects:[j[0]],culture:{name:"en-US",timePattern:"g:i A",shortDateFormat:"M/d/yyyy",longDateFormat:"dddd, MMMM dd, yyyy",decimalSeparator:".",__type:"culture"},processes:h.getProcesses()}}},t=g.create("[component.release] empty",i,b,s);c.create(t,s).viewShouldFollowBasicComponentLifeCycle().viewShouldPassTests([{name:"should render valid markup for empty release",test:function(){var b=this.$el;equal(b.find(".property-text").text(),"","Release name"),b.click();var c=a(".tau-bubble");equal(c.find(".action-link.clear").size(),0,"Backlog button is not available")}}]).done(),h.clear(),h.initDefaultData(),h.removeReleases();var i=h.getData(),u=g.create("[component.release] not releases",i,b,s);c.create(u,s).viewShouldFollowBasicComponentLifeCycle().viewShouldPassTests([{name:"should show no releases found",test:function(){var b=this.$el;equal(b.find(".property-text").text(),"","Release name"),b.click();var c=a(".tau-bubble");equal(c.find(".action-link.clear").size(),0,"Backlog button is not available"),equal(c.find(".empty-message").text(),"No releases found","Empty message is valid")}}]).done();var h=new d,j=h.getProjects(),v=h.getBugs();j[0].releases=[v[0].release],h.getProjects(),h.registerData(v),h.registerData(j);var w=h.getData(),p=g.create("[component.release]",w,b,m);c.create(p,m).viewShouldPassTests([{name:"show old should be hidden",test:function(){var b=this.$el,c=b;c.click();var d=a(".tau-bubble");equal(d.find(".action-link.more").size(),0,"Show old is not visible")}}]).done();var h=new d,j=h.getProjects(),v=h.getBugs();v[0].release=null,j[0].releases=[j[0].releases[0],j[0].releases[1]],h.getProjects(),h.registerData(v),h.registerData(j);var x=h.getData(),p=g.create("[component.release]",x,b,m);c.create(p,m).viewShouldPassTests([{name:"show old should be visible",test:function(){var b=this.$el,c=b;c.click();var d=a(".tau-bubble");equal(d.find(".action-link.more").size(),1,"Show old is visible")}}]).done()};return{run:h}})