define(["libs/jquery/jquery.ui"],function(){(function(a){a.fn.right=function(){return this.offset().left+this.width()},a.fn.bottom=function(){return this.offset().top+this.height()},a.fn.id=function(){return this.attr("id")},a.fn.visibility=function(a){a?this.css("visibility","visible"):this.css("visibility","hidden")},a.fn.selectClass=function(a,b,c){a?(this.removeClass(c),this.addClass(b)):(this.removeClass(b),this.addClass(c))},a.fn.stretch=function(){this.height(a("html").height()),this.width(a("html").width())},a.fn.outerclick=function(b){function d(d){c(this,a(d.target))||a.proxy(b,this).call()}function c(a,b){return a[0]==b[0]?!0:a.has(b).length>0?!0:!1}a("body").click(a.proxy(d,this))},a.widget("ui.modal",{overlay:null,resizeHandler:null,_create:function(){this.overlay=this._createOverlay(),this.resizeHandler=a.proxy(this._onResize,this)},_createOverlay:function(){var b=a('<div class = "modalBackground" style="position:absolute; left:0px; top:0px;"/>');b.attr("disabled","disabled"),b.zIndex(this.element.zIndex()-1);return b},_onResize:function(){this.overlay.stretch()},show:function(){this._onResize(),a(window).bind("resize",this.resizeHandler),this.overlay.appendTo("body"),this.element.show()},hide:function(){a(window).unbind("resize",this.resizeHandler),this.overlay.remove(),this.element.hide()}});var b=function(a,b,c){var d={complete:c,duration:300,queue:!0};a.animate(b,d)};a.fn.minimizeTo=function(a){var c=this,d=a.target,e=a.callback,f=c.offset(),g={left:f.left,top:f.top,width:c.width(),height:c.height()};c.data(g);var h=d.offset(),i={left:h.left,top:h.top,width:0,height:0},j=function(){c.hide(),e&&e()};b(c,i,j)},a.fn.maximizeFrom=function(c){var d=this,e=c.target,f=c.callback,g=c.settings||{},h=e.offset(),i=d.data(),j=["left","top","width","height"];a.each(j,function(a,b){i.hasOwnProperty(b)&&(g[b]=i[b])}),d.hide(),d.width(0),d.height(0),d.css("left",h.left),d.css("top",h.top),d.show();var k=function(){f&&f()};b(d,g,k)},a.fn.enabled=function(b){var c={getEnable:function(a){return!a.hasClass("disabled")},setEnabled:function(a,b){b?a.removeClass("disabled"):a.addClass("disabled")}},d={getEnable:function(a){return a.is(":enabled")},setEnabled:function(a,b){b?a.removeAttr("disabled"):a.attr("disabled","disabled")}},e=d;a(this).is("a")&&(e=c);if(b!=null)e.setEnabled(a(this),b);else return e.getEnable(a(this))},a.repeat=function(a,b){var c=[];for(var d=0;d<b;d++)c.push(a);return c},a.fn.serializeObject=function(){var b={},c=this.serializeArray();a.each(c,function(){b[this.name]!==undefined?(b[this.name].push||(b[this.name]=[b[this.name]]),b[this.name].push(this.value||"")):b[this.name]=this.value||""});return b}})(jQuery)})