define(["tau/components/extensions/component.extension.base","tau/ui/behaviour/common/ui.behaviour.tauBubble","libs/underscore"],function(a,b){return a.extend({category:"edit",initBubble:function(a){var c=this,d=a.target;d.addClass("tau-bubble-target");var e=a.alignTo;if(!e||!e.length)e=d;e.addClass&&e.addClass("ui-link");var f=function(){c.initializeContainer(a.target,this.$popup,a.componentsConfig)};this.target=d,b.call(d,{alignTo:e,onShow:f})},"bus initBubble":function(a){this.initBubble(a.data)},"bus toggleBubble":function(){b.call(this.target,"toggle")},initializeContainer:function(a,b,c){var d=this,e=b.find(".tau-bubble__inner");d.$popupInner=e,d.$target=a;a.data("bubbleLoaded")!==!0&&(e.find("div.ui-loading-message").remove(),e.append("<div class='ui-loading-message'>Loading...</div>"),d.createComponents(c,function(b){var c=b[0].component;c.on("afterRender",function(b){e.find(".ui-loading-message").remove();var d=b.data.element;d.appendTo(e),a.data("bubbleLoaded",!0),b.removeListener(),a.tauBubble("adjustPosition"),a.tauBubble("adjustPositionStart"),a.tauBubble("adjustPosition"),a.bind("close",function(){c.fire("blur",{})}),a.bind("show",function(){c.fire("focus",{}),c.fire("popupResize",a.tauBubble("getMaxSize")),a.tauBubble("adjustPosition")}),c.fire("popupResize",a.tauBubble("getMaxSize")),a.tauBubble("adjustPosition"),setTimeout(function(){a.tauBubble("adjustPosition")},10),setTimeout(function(){c.fire("focus",{})},500)}),c.initialize();var d=function(){a.data("bubbleLoaded",!1),a.tauBubble("destroy")};c.on("beforeSave",d),c.on("beforeRemove",d),c.on("destroy",d);var f=function(){a.data("bubbleLoaded",!1),a.tauBubble("empty")};c.on("reset",f)}))}})})