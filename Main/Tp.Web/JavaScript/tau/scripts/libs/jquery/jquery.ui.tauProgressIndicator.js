define(["libs/jquery/jquery.ui"],function($){var DATA_KEY="progress-indicator-originalPosition";$.widget("ui.tauProgressIndicator",{_create:function(){},show:function(options){options=options||{};var $placeholder=this.element,isNoLoader=$placeholder.children(".tau-loader").size()==0;if(isNoLoader){var originalPosition=$placeholder.css("position");!$placeholder.is("body")&&originalPosition.toLowerCase()==="static"&&$placeholder.data(DATA_KEY,originalPosition).css("position","relative");var $indicator=$("<div />").addClass("tau-loader").appendTo($placeholder);if(options.hover){var hoverCss={"background-color":"white",opacity:.5,"z-index":$indicator.css("z-index")-1,position:"absolute",top:0,left:0,height:"100%",width:"100%"};$("<div />").addClass("i-role-loader-hover").css(hoverCss).appendTo($placeholder)}var parentTagName=$placeholder.get(0).tagName||"";parentTagName.toLowerCase()!=="body",$indicator.sprite({autoplay:!0,url:!0,frames:10,width:560,height:56})}},hide:function(){var $placeholder=this.element;$placeholder.children(".tau-loader,.i-role-loader-hover").remove();var originalPosition=$placeholder.data(DATA_KEY);!$placeholder.is("body")&&originalPosition&&$placeholder.css("position",originalPosition)}})})