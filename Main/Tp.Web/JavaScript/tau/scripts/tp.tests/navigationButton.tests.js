(function(){function a(){}a.prototype={navigated:!1,onNavigate:function(){this.navigated=!0},isEnabled:function(){return!1},getData:function(){return{content:"data"}}},module("navigationButton",{setup:function(){this.controller=new a,this.target=$("<a><span>template ${content}</span></a>"),this.target.navigationButton({controller:this.controller}),this.widget=this.target.data("navigationButton")},tearDown:function(){}}),test("creation",function(){equals(this.widget.controller==null,!1,"controller wasn`t set"),equals(this.widget.template.toLowerCase(),"<span>template ${content}</span>","template wasn`t set")}),test("click",function(){this.target.click(),equals(this.controller.navigated,!0)}),test("render",function(){this.target.navigationButton("render"),equals(this.target.hasClass("disabled"),!1,"button should be disabled"),equals(this.target.html().toLowerCase(),"<span>template data</span>"),this.controller.navigated=!1,this.target.click(),equals(this.controller.navigated,!1,"navigation event shouldn`t occur when paging link is disabled")}),test("onclick",function(){var a=!1;this.target.navigationButton("click",function(){a=!0}),this.target.click(),equals(a,!0,"handler wasn`t called")})})()