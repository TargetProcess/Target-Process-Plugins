define(["jQuery"],function(a){a.notifyBar=function(b){(function(a){var c=notifyBarNS={};notifyBarNS.shown=!1,b||(b={}),notifyBarNS.html=b.html||"Your message here",notifyBarNS.delay=b.delay||2e3,notifyBarNS.animationSpeed=b.animationSpeed||200,notifyBarNS.jqObject=b.jqObject,notifyBarNS.cls=b.cls||"",notifyBarNS.close=b.close||!1,notifyBarNS.jqObject?(c=notifyBarNS.jqObject,notifyBarNS.html=c.html()):c=jQuery("<div></div>").addClass("jquery-notify-bar").addClass(notifyBarNS.cls).attr("id","__notifyBar"),c.html(notifyBarNS.html).hide();var d=c.attr("id");switch(notifyBarNS.animationSpeed){case"slow":asTime=600;break;case"normal":asTime=400;break;case"fast":asTime=200;break;default:asTime=notifyBarNS.animationSpeed}c=="object",a("body").prepend(c),notifyBarNS.close&&(c.append(a("<a href='#' class='notify-bar-close'>Close [X]</a>")),a(".notify-bar-close").click(function(){c.attr("id")=="__notifyBar"?a("#"+d).slideUp(asTime,function(){a("#"+d).remove()}):a("#"+d).slideUp(asTime);return!1})),a(".jquery-notify-bar:visible").length>0?a(".jquery-notify-bar:visible").stop().slideUp(asTime,function(){c.stop().slideDown(asTime)}):c.slideDown(asTime),c.click(function(){a(this).slideUp(asTime)}),c.attr("id")=="__notifyBar"?setTimeout("jQuery('#"+d+"').stop().slideUp("+asTime+", function() {jQuery('#"+d+"').remove()});",notifyBarNS.delay+asTime):setTimeout("jQuery('#"+d+"').stop().slideUp("+asTime+", function() {jQuery('#"+d+"')});",notifyBarNS.delay+asTime)})(a)};return a.notifyBar})