define(["tau/core/tau","tau/components/component.label","tests/components/common/common.setup","tests/components/component.specs"],function(a,b,c,d){var e=function(){var a={context:{type:"story",id:15},settings:{text:"label text"}},e=c.create("[component.label] without badge",{},b),f={text:a.settings.text},g=[{name:"should render label without badge",test:function(){var a=this.$el;equal(a.children(".ui-label").text(),f.text,"Text of label is valid"),ok(!a.children(".ui-badge").length,"Badge was not added")}}];d.create(e,a).viewShouldFollowDataComponentLifeCycle().modelShouldReturnData(f,[]).viewShouldPassTests(g).done();var h={context:{type:"story",id:15},settings:{text:"label text",badgeFieldName:"textForBadge",getBadgeText:function(a){a.callback.call(a.scope,{textForBadge:"valueForTextOfBadge"})}}},i=c.create("[component.label] with badge",{},b),g=[{name:"should render label with badge",test:function(){var a=this.$el;equal(a.children(".ui-label").text(),f.text,"Text of label is valid"),equal(a.children(".ui-quantity").text(),"valueForTextOfBadge","Badge text is valid")}}];d.create(i,h).viewShouldFollowDataComponentLifeCycle().modelShouldReturnData(f,[]).viewShouldPassTests(g).done();var j={context:{type:"story",id:15},settings:{text:["Foo","Bar","Baz"]}},k={text:j.settings.text},l=c.create("[component.label] with text array",{},b);g=[{name:"should render label with correct text and css class",test:function(){var a=this.$el;equals(a.children(".ui-label").text(),"Foo, Bar & Baz","Text of label is valid"),equals(a.children(".ui-label").attr("class"),"ui-label","No unspecified css classes")}}],d.create(l,j).viewShouldFollowDataComponentLifeCycle().modelShouldReturnData(k,[]).viewShouldPassTests(g).done(),j={context:{type:"story",id:15},settings:{text:["Foo"],cssClass:"test-css-class"}},k={text:j.settings.text,cssClass:j.settings.cssClass},l=c.create("[component.label] with text array",{},b),g=[{name:"should render label with correct text and css class",test:function(){var a=this.$el;equals(a.children(".ui-label").text(),"Foo","Text of label is valid"),ok(a.children(".ui-label").hasClass("test-css-class"),"Css class is applied")}}],d.create(l,j).viewShouldFollowDataComponentLifeCycle().modelShouldReturnData(k,[]).viewShouldPassTests(g).done()};return{run:e}})