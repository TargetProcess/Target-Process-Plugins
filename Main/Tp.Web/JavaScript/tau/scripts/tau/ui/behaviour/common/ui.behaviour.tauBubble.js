define(["Underscore","jQuery"],function(a,b){b.widget("ui.tauBubble",{_bubblesCache:[],_isInit:!1,$popup:null,$closeEl:null,$arrows:{top:null,bottom:null},$target:null,$alignTo:null,options:{viewport:"body",alignTo:null,collision:"fit flip",onShow:b.noop,showOnCreation:!1,closeOnEscape:!0},_create:function(){this.$target=this.element,this.options.alignTo&&(this.$alignTo=b(this.options.alignTo)),this.$alignTo=this.$alignTo.length?this.$alignTo:this.$target;if(this.options.showOnCreation==!0)this.show();else{var a=this.onTargetClickDelegate=b.proxy(this._onTargetClick,this);this.$target.bind("click",a)}},_onTargetClick:function(a){a.preventDefault(),a.stopPropagation(),this.toggle()},_initInstance:function(a){var c=this,d,e=this.$target.parents(".ui-popup-content:first");e.length?(d=e,$place=d.parent()):(d=b("body"),$place=d);var f=b('<div class="tau-bubble popup-container"><div class="tau-bubble__inner"></div></div>');f.appendTo($place),this.$popup=f,c.$arrows={top:b('<div class="tau-bubble__arrow-top"></div>'),bottom:b('<div class="tau-bubble__arrow-bottom"></div>')},c.$arrows.top.prependTo(f),c.$arrows.bottom.prependTo(f),c._bubblesCache.push(c);var g=b.proxy(this.hide,this);f.mouseenter(function(a){f.data("focus",!0)}),f.mouseleave(function(a){f.data("focus",!1)}),d.scroll(function(){g()}),this._initInstance=function(){}},show:function(){this._initInstance();var c=this;c.$popup.zIndex(c.$target.zIndex()+1),c.adjustPosition(),c.options.onShow.call(c,c.$popup),b.each(c._bubblesCache,function(){this!=c&&this.hide()}),c._signUpForCloseEvents(),c._windowResizeDelegate||(c._windowResizeDelegate=a.bind(function(){this.adjustPosition()},this)),b(window).bind("resize",c._windowResizeDelegate),c.$target.trigger("show",{})},hide:function(){var a=this,c=a.$popup;!c.is(":visible")||(c.hide(),a._documentKeyDown&&(b(document).unbind("keydown",a._documentKeyDown),delete a._documentKeyDown),a._documentClickDelegate&&(b(document).unbind("click",a._documentClickDelegate),delete a._documentClickDelegate),a._windowResizeDelegate&&(b(window).unbind("resize",a._windowResizeDelegate),delete a._windowResizeDelegate),a.$target.trigger("close",{}))},_onKeyDown:function(a){a.keyCode==b.ui.keyCode.ESCAPE&&this.hide()},_signUpForCloseEvents:function(){var a=this;a.options.closeOnEscape&&!a._documentKeyDown&&(a._documentKeyDown=function(b){a._onKeyDown(b)},b(document).keydown(a._documentKeyDown)),a._documentClickDelegate||(a._documentClickDelegate=function(b){a._onDocumentClick(b)},b(document).click(a._documentClickDelegate))},_processArrows:function(){var a=this,c=a.$alignTo,d=a.$popup;a.$arrows.top.hide(),a.$arrows.bottom.hide();var e=a.$arrows.top,f=c.position(),g=d.position();g.top<f.top&&(e=a.$arrows.bottom);if(g.left<f.left){var h=c.width(),i=f.left-g.left-(b.browser.webkit?17:24)+h/2;e.css("left",i+"px")}e.show()},toggle:function(){var a=this;!a.$popup||!a.$popup.is(":visible")?a.show():a.hide()},widget:function(){return this.$popup||b()},adjustPosition:function(){var a=this,b=a.$alignTo,c=a.$popup;c.show();var d="-20 9";b.width()>=20&&(d="0 9");var e={of:b,my:"left top",at:"left bottom",offset:d,collision:a.options.collision};c.position(e);var f=b.position(),g=c.position();g.top<0&&(e.collision="none",c.position(e),g=c.position()),a._processArrows();var h=a.element},_onDocumentClick:function(a){var c=this,d=b(a.target);if(!!c.$popup.is(":visible")){if(d.hasClass("tau-bubble-target")||d.parents(".tau-bubble-target").length>0)return;if(d.hasClass("tau-bubble")||d.parents("div.tau-bubble").length>0)return;if(c._justActivated){c._justActivated=!1;return}c.hide()}},activate:function(){this._justActivated=!0,this.show()},destroy:function(){var a=this,c=a.element;a.$popup&&(a.hide(),a.$popup.remove(),a.$arrows.top.remove(),a.$arrows.bottom.remove()),c.unbind("click",a.onTargetClickDelegate),b.Widget.prototype.destroy.apply(this,arguments)},empty:function(){var a=this;a.hide(),a.$popup.find(".tau-bubble__inner").empty()}});return b.fn.tauBubble})