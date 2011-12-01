(function(a){module("tp.jquery.utils.outerclick",{setup:function(){this.isClicked=!1,this.element1=a("<div />").appendTo(a("body")),this.element1child=a("<div />").appendTo(this.element1),this.element1childchild=a("<div />").appendTo(this.element1child),this.element2=a("<div />").appendTo(a("body")),this.element1.outerclick(a.proxy(function(){this.isClicked=!0},this))},teardown:function(){this.element1child.remove(),this.element1childchild.remove(),this.element1.remove(),this.element2.remove(),delete this.isClicked,delete this.element1,delete this.element2,delete this.element1child,delete this.element1childchild}}),test("click on element",function(){this.isClicked=!1,this.element1.click(),equals(this.isClicked,!1)}),test("click on body",function(){this.isClicked=!1,a("body").click(),equals(this.isClicked,!0)}),test("click on other element",function(){this.isClicked=!1,this.element2.click(),equals(this.isClicked,!0)}),test("click on first level child",function(){this.isClicked=!1,this.element1child.click(),equals(this.isClicked,!1)}),test("click on second level child",function(){this.isClicked=!1,this.element1childchild.click(),equals(this.isClicked,!1)}),module("tp.jquery.utils.minimizeTo",{setup:function(){this.tmp_hide=a.fn.hide,this.tmp_animate=a.fn.animate,this.element=a('<div style="width:25px;height:25px;">Element</div>'),this.element.appendTo("body"),this.target=a('<div style="position:absolute;left:10px;top:20px;">Target</div>'),this.target.appendTo("body")},teardown:function(){this.element.remove(),this.target.remove(),delete this.element,delete this.target,a.fn.hide=this.tmp_hide,a.fn.animate=this.tmp_animate,delete this.tmp_hide,delete this.tmp_animate}}),test("call minimizeTo",function(){var b=this,c=0;a.fn.hide=function(){c++};var d;a.fn.animate=function(a,b){d=arguments,b.complete()};var e={target:b.target,callback:function(){equal(c,1,"Hide is called when minimized")}},f=b.element.offset(),g={left:f.left,top:f.top,width:b.element.width(),height:b.element.height()};b.element.minimizeTo(e);var h=[{left:10,top:20,width:0,height:0}];same(d[0],h[0],"Animate parameters are valid"),same(b.element.data(),g,"Original layout settings are saved on element correctly")}),module("tp.jquery.utils.maximizeFrom",{setup:function(){this.tmp_hide=a.fn.hide,this.tmp_show=a.fn.show,this.tmp_animate=a.fn.animate,this.element=a('<div style="position:absolute;width:25px;height:25px;left:1px;top:2px;">Element</div>'),this.element.appendTo("body"),this.target=a('<div style="position:absolute;left:10px;top:20px;">Target</div>'),this.target.appendTo("body")},teardown:function(){this.element.remove(),this.target.remove(),delete this.element,delete this.target,a.fn.hide=this.tmp_hide,a.fn.show=this.tmp_show,a.fn.animate=this.tmp_animate,delete this.tmp_hide,delete this.tmp_show,delete this.tmp_animate}}),test("call maximizeFrom",function(){var b=this,c={hide:1,show:2},d=0,e=0;a.fn.hide=function(){d++,ok(d===1,"Hide should be called before setup");var a=this,b=a.offset();equals(b.left,1,"Left position before setup is valid"),equals(b.top,2,"Top position before setup is valid"),equals(a.width(),25,"Width before setup is valid"),equals(a.height(),25,"Height before setup is valid")};var f=0;a.fn.show=function(){d++,ok(d===2,"Show should be called after setup complete");var a=this,b=a.offset();equals(b.left,10,"Left position before setup is valid"),equals(b.top,20,"Top position before setup is valid"),equals(a.width(),0,"Width after setup is valid"),equals(a.height(),0,"Height after setup is valid")};var g;a.fn.animate=function(a,b){g=arguments,b.complete()};var h={target:b.target,callback:function(){ok(!0,"Callback is called")},settings:{left:11,top:22,width:120,height:100}};b.element.maximizeFrom(h);var i=[{left:11,top:22,width:120,height:100}];same(g[0],i[0],"Animate parameters are valid")})})(jQuery)