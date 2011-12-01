define(["Underscore","jQuery","tau/utils/utils.textsearch","tau/components/extensions/component.extension.base","tau/ui/behaviour/common/ui.behaviour.listSelectable"],function(a,b,c,d){var e=d.extend({"bus dataBind+afterRender":function(c){var d=this.config,e=this.element=c.afterRender.data.element,f=this.dataToBind=c.dataBind.data,g=0;a.forEach(f.states,function(a){a.items?g+=a.items.length:g++}),this.$list=e.find(".drop-down-list"),this.$options=this.$list.find(".drop-down-option"),this.$options.click(b.proxy(this.onItemSelected,this)),d.hasOwnProperty("maxHeight")&&this.$list.css("max-height",d.maxHeight).css("overflow-y","auto");var h=b("<div class='drop-down-actions' />"),i=!1;if(f.hasOwnProperty("defaultValue")){i=!0;var j=this.config.defaultValueLabel||"default";b("<span class='action-link default'>"+j+"</span>").click(b.proxy(this.defaultValue,this)).appendTo(h)}if(f.nullableValue){i=!0;var k=this.config.clearValueLabel||"reset";b("<span class='action-link clear'>"+k+"</span>").click(b.proxy(this.clearValue,this)).appendTo(h)}i&&h.insertBefore(this.$list);if(f.states.length===0&&f.completed){var l=d.emptyDataMessage||"None";this.$list.append("<span class='empty-message'>"+l+"</span>")}d.filter&&g>3&&(this.filterField=b("<input type='text' class='filter-field' />").keyup(b.proxy(this._filterDelegate,this)),b("<div class='filter-field-wrapper'></div>").append(this.filterField).insertBefore(this.$list));if(d.expandable&&!f.completed){var m=d.mode||"short";if(m!=="full"){var n=b("<div class='drop-down-footer'></div>"),o=this.config.fullModeLabel||"more...";this.$mode=b("<div class='action-link more'>"+o+"</div>").click(b.proxy(this._toggleMode,this)).appendTo(n),n.insertAfter(this.$list)}}this._resize()},"bus focus":function(a){this.element.find(".drop-down-list").listSelectable({items:".drop-down-option:visible",className:"drop-down-option_hover"}),this.element.find(".drop-down-list").listSelectable("enable")},"bus blur":function(a){this.element.find(".drop-down-list").listSelectable("disable")},_resize:function(a){if(!!this.element){var c=this.element,d=c.find(".drop-down-list");d.css("maxHeight",b(window).height()/2*.8),d.css("overflowY","auto"),d.css("overflowX","hidden")}},"bus afterRender+popupResize":function(a){this._resize(a.popupResize.data)},"bus popupResize":function(a){this._resize(a.data)},_toggleMode:function(a){a.stopPropagation();var b=this.config.mode==="full"?"short":"full";this.fire("refresh",{mode:b})},clearValue:function(a){a.stopPropagation(),this.fire("updateState",null)},defaultValue:function(a){a.stopPropagation(),this.fire("updateState",{id:this.dataToBind.defaultValue})},onItemSelected:function(a){var c=b(a.target).tmplItem().data;this.fire("updateState",{id:c.id})},_resetList:function(){this.$options.show()},_noResults:function(a){var a='"'+a+'" not found';this.$noResults?this.$noResults.text(a):this.$noResults=b("<div class='not-found' />").text(a).insertBefore(this.$list),this.$noResults.show()},_clearNoResults:function(){this.$noResults&&this.$noResults.hide()},_filterDelegate:function(d){if(d.keyCode!=jQuery.ui.keyCode.DOWN&&d.keyCode!=jQuery.ui.keyCode.UP&&d.keyCode!=jQuery.ui.keyCode.ENTER){if(!this.searchIndex){var e=[];this.searchIndex=new c,this.$options.each(function(a,c){e.push({_id:a,data:b(c).text()}),b(this).data("text",b(c).text())}),this.searchIndex.load(e)}var f=this.filterField.val(),g=this.searchIndex.search(f);this.$options.hide();var h=this.$options;a.forEach(g,function(a){var b=h.eq(a._id),c=b.data("text");c=c.substr(0,a.entry.from)+"<em>"+c.substr(a.entry.from,a.entry.len)+"</em>"+c.substr(a.entry.from+a.entry.len,c.length),b.html(c),b.show(),b.parent().show()}),h.filter(":hidden").parent().not(h.filter(":visible").parent()).hide(),f.length?g.length==0?this._noResults(f):this._clearNoResults():(this._clearNoResults(),this._resetList()),this.element.find(".drop-down-list").listSelectable("reset")}}});return e})