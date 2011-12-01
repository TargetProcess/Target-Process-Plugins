Ext.ns('Tp.controls.menu');

Tp.controls.menu.MenuHighlighter = Ext.extend(Object, {
	constructor: function (config) {
		this.init();
	},

	init: function () {
		var currentTabItem = this.highlightCurrentTabItem("div.tab");
		if (Ext.isEmpty(currentTabItem)) {
			this.highlightCurrentMoreItem("div.tabToAdd");
		}
	},

	highlightCurrentTabItem: function (cssClass) {
		var tabItem = this.highlightCurrentItem(cssClass);
		if (tabItem.getCount() > 0) {
			tabItem.first().child("a").addClass("selectedTabLink");
			return tabItem;
		}
		return null;
	},

	highlightCurrentMoreItem: function (cssClass) {
		var moreItem = this.highlightCurrentItem(cssClass);
		if (moreItem.getCount() > 0) {
			var item = moreItem.first(); 
			var text = item.dom.innerText + '';

			if (text == 'undefined') {
				text = item.first().dom.innerHTML;
			}

			Ext.fly("morePopupTrigger").update(text + " - " + Ext.getDom("morePopupTrigger").innerHTML);
			Ext.fly("actionGroupsViewMenu").addClass("popupTab");
		}
	},

	highlightCurrentItem: function (cssClass) {
		var tabItems = Ext.select(cssClass);
		var currentItem = tabItems.filter(this.matchCurrentUrl);
		if (currentItem.getCount() > 0) {
			currentItem.addClass("selectedTab");
		}

		return currentItem;
	},

	matchCurrentUrl: function (el, index) {
		var anchor = el.child("a");

		if (!el.parent().isVisible())
			return false;

		var url = new Tp.URL(anchor.dom.href);
		var currentUrl = new Tp.URL(location.href);

		if (url.getPath().toLowerCase() == currentUrl.getPath().toLowerCase()) {
			return true;
		}

		return false;
	}
});