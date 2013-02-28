define(["Underscore","jQuery","tau/components/extensions/component.extension.base"],function(_,$,ExtensionBase){return ExtensionBase.extend({_declareDnD:function($element){var self=this;$(".draggable",$element).draggable({revert:"invalid",helper:"clone",cursor:"move",stop:function(event,ui){$(event.target).removeClass("moved-slice-card")},start:function(event,ui){$(event.target).addClass("moved-slice-card")}}),$(".droppable",$element).droppable({activeClass:"ui-state-hover",hoverClass:"ui-state-active",drop:function(event,ui){var $card=$(ui.draggable),$prevPapa=$card.parents(".slice-cell"),$сell=$(this);$сell.append($card);var parts=$сell.attr("id").split("_"),args={x:parts[0],y:parts[1],id:$card.attr("id"),card:$card,prevCell:$prevPapa};self.bus.fire("move",args)}})},"bus afterRender":function(evt){var $element=evt.data.element,data=evt.data.data,timeRender=new Date-data.renderTime,$sliceInfo=$("#null_null>.slice-info",$element);$sliceInfo.append("<br/> render: "+timeRender+"ms");var ddStartTime=new Date;this._declareDnD($element),$sliceInfo.append("<br/> d&d: "+(new Date-ddStartTime)+"ms")},"bus moveCompleted":function(args){var saga=args.data.saga,color=saga.card.css("backgroundColor");saga.card.css({backgroundColor:"#dbf383"}),saga.card.animate({backgroundColor:color},"slow")},"bus moveFailed":function(args){var saga=args.data.saga;$(saga.prevCell).append(saga.card);var color=saga.card.css("backgroundColor");saga.card.css({backgroundColor:"red"}),saga.card.animate({backgroundColor:color},"slow")}})})