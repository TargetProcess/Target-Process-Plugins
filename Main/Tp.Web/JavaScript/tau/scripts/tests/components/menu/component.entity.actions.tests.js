define(["Underscore","jQuery","tau/configurations/factory.container.config","tau/components/component.entity.actions","tests/components/component.specs","tests/common/testData","tests/components/common/common.setup","tests/common/applicationContext"],function(a,b,c,d,e,f,g,h){var i=function(){function c(a){var c=g.create("[component.entity.actions]["+a+"]",{},d),f=[{name:"should render valid markup",test:function(){var a=this.$el,c=a.find(".tau-entity-actions-target");ok(c.hasClass("tau-bubble-target")),c.click();var d=b("body").find(".tau-bubble__inner > .ui-menu-actions > .drop-down-list > .ui-menu__item");ok(d.size()>0,"Menu items are shown on click")}}],i={context:{type:a,id:15}},j=new h(i);i.context.applicationContext=j,e.create(c,i).viewShouldFollowDataComponentLifeCycle().viewShouldPassTests(f).done()}function f(c){var f=c.entityTypeName,i=c.practiceName,j=c.menuItemTitle,k=c.practiceEnabled,l={context:{type:f,id:15}},m=new h(l);if(!k)m.processes[0].practices=a(m.processes[0].practices).select(function(a){return a.name!==i});else{var n=a(m.processes[0].practices).select(function(a){return a.name===i});if(!n.length)throw{message:"["+i+"] practice not found"}}l.context.applicationContext=m;var o=g.create("[component.entity.actions]["+f+"]["+i+":"+k+"]",{},d,null,m),p=[{name:"should render valid markup",test:function(){var a=this.$el,c=a.find(".tau-entity-actions-target");ok(c.hasClass("tau-bubble-target")),c.click();var d=b("body").find(".tau-bubble__inner > .ui-menu-actions > .drop-down-list > .ui-menu__item"),e=d.find(":contains('"+j+"')"),f=k?1:0;equals(e.size(),f,"Practice is applied")}}];e.create(o,l).viewShouldPassTests(p).done()}c("bug"),c("userStory"),c("feature"),c("release"),f({entityTypeName:"userStory",practiceName:"Bug Tracking",menuItemTitle:"Add Issue",practiceEnabled:!0}),f({entityTypeName:"userStory",practiceName:"Bug Tracking",menuItemTitle:"Add Issue",practiceEnabled:!1}),f({entityTypeName:"release",practiceName:"Iterations",menuItemTitle:"Add Iteration",practiceEnabled:!0}),f({entityTypeName:"release",practiceName:"Iterations",menuItemTitle:"Add Iteration",practiceEnabled:!1})};return{run:i}})