define(["Underscore","tau/views/view.container","tau/components/component.tabsHeader"],function(a,b){var c=b.extend({"bus beforeInit":function(b){var c=this;c._selectedIndex=-1,this.additionalHeaderTabIndexList={};var d=b.data.config;this.tabsHeader={type:"tabsHeader",children:[]},this.panels=[],a.each(d.tabs,this.addTabInfo,this),a.each(d.additionalHeaders||[],this.addAdditionalActionToTab,this),d.children=[this.tabsHeader].concat(this.panels)},addTabInfo:function(a,b){var c=a.selected===!0,d={type:"container",name:a.label+" tab container",selected:c,children:a.header||[],label:a.label};this.tabsHeader.children.push(d);var e={type:"container",visible:c,children:a.items||[]};this.panels.push(e),c&&(this._selectedIndex=b)},addAdditionalActionToTab:function(a){var b=a||{};b.position={right:!0};var c=this.tabsHeader;c.children.push(b),this.additionalHeaderTabIndexList[c.children.length]=!0},isAdditionalAction:function(a){return this.additionalHeaderTabIndexList[a+1]},onBeforeSelectTab:function(a){var b=a.data.index;this.isAdditionalAction(b)&&a.cancel(),a.data.tabHeader.fire("select")},"bus childrenRendered":function(a){var b=(this.children=a.data.children)[0];b.on("beforeSelectTab",this.onBeforeSelectTab,this),b.on("tabSelected",this.onTabSelected,this)},hideAllChildren:function(){a.forEach(this.children,function(a,b){b>0&&a.fire("hide")})},showChildrenByIndex:function(a){this.children[1+a].fire("show")},setSelectedTabIndex:function(a){this.children[1+a]&&(this.hideAllChildren(),this.showChildrenByIndex(a),this._selectedIndex=a)},onTabSelected:function(a){var b=a.data.index;this.setSelectedTabIndex(b)}});return c})