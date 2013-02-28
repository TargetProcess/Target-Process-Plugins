define(["Underscore","jQuery","tau/components/extensions/component.extension.base"],function(_,$,ExtensionBase,search){return ExtensionBase.extend({"bus afterRender":function(args){var $element=args.data.element,self=this,$input=$(".filter-field",$element),showHandler=function(){$input.is(":visible")&&$input.focus()};$(".tau-bubble").bind("show",showHandler),self.fire("showHandlerSigned",{handler:showHandler});var searchHandler=function(evt){var $input=$(this),value=$input.val();if(evt.keyCode==jQuery.ui.keyCode.ENTER){self.fire("completeInput",{element:$element,value:value});return}if(evt.keyCode==jQuery.ui.keyCode.DOWN||evt.keyCode==jQuery.ui.keyCode.UP)return;self.fire("inputChange",{element:$element,value:value})};$input.keyup(searchHandler),this.disableChange=!1},"bus inputChange":function(args){var self=this;self.lastChangeDate||(self.lastChangeDate=new Date),self.lastArgs||(self.lastArgs={value:""});if(self.lastArgs.value==args.data.value)return;self.lastArgs=args.data,setTimeout(function(){if(!self.lastChangeDate)return;var diff=new Date-self.lastChangeDate;diff>500&&self.lastArgs&&self.fire("completeInput",self.lastArgs)},501)},"bus partialDataRequest":function(){this.disableChange=!0},"bus dataRequestCompleted":function(){this.disableChange=!1},"bus completeInput":function(args){var self=this;self.lastRequestArgs||(self.lastRequestArgs={value:""});if(self.lastRequestArgs.value==args.data.value)return;var collection=args.data.value!==""&&args.data.value!==null?[{property:"name",value:args.data.value}]:[];self.fire("search",{element:args.data.element,collection:collection}),self.lastChangeDate=null,self.lastRequestArgs=args.data},"bus destroy+showHandlerSigned":function(args){$(".tau-bubble").unbind("show",args.showHandlerSigned.data.handler)}})})