Ext.ns('Tp.Menu');

var latestDrugMenuItem = null;

Tp.Menu.LinkButton= Ext.extend(Ext.Button, {
    href: null,
    template: new Ext.Template(
        ['<div id="{4}" class="button x-small">',
        '<div></div></div>'], {compiled: true}),
    buttonSelector : 'div:first-child',
    getTemplateArgs : function(){
        return [this.href, 'x-btn-' + this.scale + ' x-btn-icon-' + this.scale + '-' + this.iconAlign, this.getMenuClass(), this.cls, this.id];
    },
    handler: function(b, e) {
        if (this.href) {
            e.stopEvent();
            window.location.href = this.href;
        }
    }
});

Tp.Menu.Customizer = Ext.extend(Object, {

	constructor: function (config) {
		Ext.apply(this, config);
	},

	showLinks: function (tab) {
		var extEl = Ext.get(tab);

		var links = extEl.select('a');
		links.setVisibilityMode(2);
		links.show();

		if (tab.isSelectedTab)
			extEl.addClass("selectedTab");

		if (tab.isDisabledTab) {
			Ext.each(links.elements, function (link) {
				Ext.get(link).addClass("disabledAction");
				link.setAttribute("disabled", "disabled");
				link.removeAttribute("href");
			});
		}

		var spans = extEl.select('span');
		spans.setVisibilityMode(2);
		spans.hide();
	},

	replaceLinks: function (tab) {
		var links = Ext.get(tab).select('a');
		links.setVisibilityMode(2);
		links.hide();

		var spans = Ext.get(tab).select('span');
		spans.setVisibilityMode(2);
		spans.show();

		if (spans.elements.length == 0) {
			Ext.each(links.elements, function (link) {
				var label = document.createElement("span");
				label.innerHTML = link.innerHTML;
				tab.appendChild(label);
			});
		}
	},

	getMenuTabs: function () {
		return Ext.get("menuTabs").select(".tab").elements;
	},

	getMenuAdditionalTabs: function () {
		return this.getTabsBacklog().select(".tabToAdd").elements;
	},

	getTabElementForSpan: function (e) {
		if (e.target == null)
			return null;

		var parent = Ext.get(e.target).parent();

		if (parent == null)
			return null;

		var element = parent.dom;

		return element;
	},

	onMouseOutAdditionalMenu: function (e) {
		var element = this.getTabElementForSpan(e);

		if (element == null)
			return;

		if (element.dd && !element.isInMenu) {
			Ext.get(e.target).removeClass("additionalMenuTabCustomize");
		}
	},

	onMouseOverAdditionalMenu: function (e) {

		var element = this.getTabElementForSpan(e);

		if (element == null)
			return;

		if (element.dd && !element.isInMenu) {
			Ext.get(e.target).addClass("additionalMenuTabCustomize");
		}
	},

	getPlaceHolders: function () {
		return Ext.get("menuTable").select(".tabPlace");
	},

	initializeItems: function (items, isInMenu) {
		Ext.each(items, function (item) {
			var customizer = new Tp.Menu.Customizer();

			item.isInMenu = isInMenu;

			var tab = Ext.get(item);

			customizer.replaceLinks(tab);

			tab.select("span").on("mouseover", customizer.onMouseOverAdditionalMenu, customizer);
			tab.select("span").on("mouseout", customizer.onMouseOutAdditionalMenu, customizer);

			if (!item.dd) {
				item.dd = new Ext.dd.DD(item, 'tabsDDGroup', {
					isTarget: false
				});
			}
			else {
				item.dd.unlock();
			}

			Ext.apply(item.dd, customizer);

			if (tab.getAttribute("disabled") == 'disabled') {
				item.isDisabledTab = true;
			}

			if (tab.hasClass("selectedTab")) {
				item.isSelectedTab = true;
				tab.removeClass("selectedTab");
			}
		});
	},

	getTabsBacklog: function () {
		var backLog = Ext.get("morePanel");
		backLog.isBackLog = true;
		return backLog;
	},

	initialize: function () {
		if (!Ext.get("btnStartMenuCustomization"))
			return;

		this.customizeButton = new Tp.Menu.LinkButton({ text: "Customize Menu",  renderTo: "btnStartMenuCustomizationHolder" });
		this.customizeButton.on('click', this.onStartCustomization, this);

		this.doneButton = new Tp.Menu.LinkButton({ text: "Done", renderTo: "btnDoneCustomizeMenu", hidden: true });
		this.doneButton.on('click', this.onFinishCustomization, this);
	},

	onStartCustomization: function () {
		stopDropDownPanelHiding = true;
		var menuTabs = Ext.get("menuTabs");
		menuTabs.setHeight(menuTabs.getHeight() + 5);
		menuTabs.select(".addMenuItems").hide();
		menuTabs.select(".addItemsMenu").hide();
		Ext.get("btnStartMenuCustomization").setVisibilityMode(2);
		Ext.get("btnStartMenuCustomization").hide();
		this.doneButton.show();
		this.initializeDragAndDrop();
		menuTabs.addClass("paneledit");
		var dragLabel = Ext.get("dragyour");
		dragLabel.setVisibilityMode(1);
		dragLabel.show();
		Ext.get("morePanel").on("mouseup", this.onMouseUpInBackLog, this);
		Ext.get("morePanel").setTop('');
		this.applyOverlay();
		Ext.EventManager.onWindowResize(this.applyOverlay, this);
		SetLastAction(null);
	},

	applyOverlay: function () {
		Tp.controls.getOverlay().show();
	},

	onFinishCustomization: function () {
		this.saveMenu();
		Ext.get("btnStartMenuCustomization").setVisibilityMode(2);
		Ext.get("btnStartMenuCustomization").show();
		this.doneButton.hide();
		this.removeDDForTabs(this.getMenuTabs());
		this.removeDDForTabs(this.getMenuAdditionalTabs());
		stopDropDownPanelHiding = false;
		var menuTabs = Ext.get("menuTabs");
		menuTabs.setHeight(menuTabs.getHeight() - 5);
		menuTabs.removeClass("paneledit");
		menuTabs.select(".addMenuItems").removeClass("popupTab");
		var dragLabel = Ext.get("dragyour");
		dragLabel.setVisibilityMode(2);
		dragLabel.hide();
		var morePanel = Ext.get("morePanel");
		morePanel.removeListener("mouseup", this.onMouseUpInBackLog, this);
		morePanel.hide();
		Tp.controls.getOverlay().hide();
		Ext.get("actionGroupsViewMenu").removeClass("popupTab");
		Ext.get("menuTabs").select(".addMenuItems").show();
	},

	saveMenu: function () {
		var tabs = this.getMenuTabs();
		var menuItems = [];

		Ext.each(tabs, function (tab) {
			var menu = tab.id.split('-')[1];
			menuItems.push(menu);
		});

		Ext.Ajax.request({
			url: appHostAndPath + '/PageServices/WebMethods.asmx/SaveMenu',
			headers: { 'Content-Type': 'application/json' },
			success: this.onSaveMenu,
			failure: this.onSaveFailed,
			jsonData: { 'menuItems': menuItems }
		});
	},

	onSaveMenu: function () {
		SetLastAction("Menu saved");
	},

	onSaveFailed: function () {
		SetLastWarning("Menu is not saved due errors. Please try to save it again");
	},

	removeDDForTabs: function (tabs) {
		var customizer = this;
		Ext.each(tabs, function (tab) {
			customizer.showLinks(tab);
			tab.dd.lock();
		});
	},

	onMouseUpInBackLog: function (e) {
		var element = this.getTabElementForSpan(e);

		if (element.dd != null)
			latestDrugMenuItem = element;

		if (!latestDrugMenuItem)
			return;

		if (latestDrugMenuItem.isInMenu)
			return;

		latestDrugMenuItem.dd.DDM.handleMouseUp(e);

		latestDrugMenuItem = null;
	},

	initializeDragAndDrop: function () {
		this.initializeItems(this.getMenuTabs(), true);
		var additional = this.getMenuAdditionalTabs();
		this.initializeItems(additional, false);

		var places = this.getPlaceHolders();

		var dd = new Ext.dd.DDTarget(this.getTabsBacklog(), 'tabsDDGroup');

		Ext.each(places.elements, function (place) {
			place.isBackLog = false;
			var dd = new Ext.dd.DDTarget(place, 'tabsDDGroup');
		});
	},

	startDrag: function () {
		var el = this.getEl();
		latestDrugMenuItem = el;
		var tab = Ext.get(el);
		tab.addClass("dragTab");
		this.markAsPlaceHolder(tab.parent());
		var menu = Ext.get("menuTabs");
		menu.removeClass("paneledit");
		menu.addClass("paneledithover");
	},

	onDragEnter: function (evtObj, targetElId) {
		var element = this.getEl();
		var tab = Ext.get(element);
		var originalPlace = tab.parent();

		var targetDropZone = Ext.get(targetElId);

		if (targetDropZone.isBackLog) {
			if (element.isInMenu)
				targetDropZone.addClass("menuBackLogHighlighted");
			return;
		}

		if (!originalPlace || originalPlace.id == targetElId)
			return;

		var originalX = originalPlace.getX();
		var left = tab.dom.style.left;
		var top = tab.dom.style.top;
		var leftNumber = parseInt(left.replace('px', ''));
		var x = tab.getX();
		var y = tab.getY();

		var innerTab = targetDropZone.select(".tab");
		this.shiftTabs(tab, innerTab.elements[0]);
		targetDropZone.appendChild(tab);

		var currentX = targetDropZone.getX();
		var delta = originalX - currentX;
		leftNumber = leftNumber + delta;
		tab.applyStyles({ position: 'relative', left: leftNumber + "px", top: top });

		this.deltaSetXY = null;

		this.markAsPlaceHolder(targetDropZone);
		this.unmarkAsTabPlaceHolder(originalPlace);
	},

	shiftTabs: function (originalTab, targetTab) {
		var tabs = this.getMenuTabs();

		var originIndex = this.getTabPostionIndex(tabs, originalTab);
		var targetIndex = this.getTabPostionIndex(tabs, targetTab);

		var place = null;

		for (var i = originIndex; i < targetIndex; i++) {
			place = place == null ? Ext.get(tabs[i]).parent() : place;
			var tabToMove = tabs[i + 1];
			var newPlace = Ext.get(tabToMove).parent();
			place.appendChild(tabToMove);
			place = newPlace;
		}

		for (var i = originIndex; i > targetIndex; i--) {
			place = place == null ? Ext.get(tabs[i]).parent() : place;
			var tabToMove = tabs[i - 1];
			var newPlace = Ext.get(tabToMove).parent();
			place.appendChild(tabToMove);
			place = newPlace;
		}
	},

	getTabPostionIndex: function (tabs, tab) {
		for (var i = 0; i < tabs.length; i++) {
			if (Ext.get(tabs[i]).dom.id == Ext.get(tab).dom.id)
				return i;
		}
	},

	onDragOut: function (evtObj, targetElId) {
		var dropZone = Ext.get(targetElId);

		var element = this.getEl();

		if (dropZone.isBackLog && element.isInMenu) {
			dropZone.removeClass("menuBackLogHighlighted");
			return;
		}

		if (dropZone.isBackLog && !element.isInMenu) {

			element.isInMenu = true;
			var place = this.createPlaceHolderInMenu();
			place.isBackLog = false;
			var cell = Ext.get(place);
			var tab = Ext.get(element);
			tab.removeClass("tabToAdd");
			tab.addClass("tab");

			this.changeParentOfDragItem(tab, cell);
			this.markAsPlaceHolder(cell);
			place.isBackLog = false;
			var dd = new Ext.dd.DD(place, 'tabsDDGroup');
			var placeHolderInBackLog = Ext.get(this.getBacklogPlaceholder(element.id));
			placeHolderInBackLog.setVisibilityMode(2);
			placeHolderInBackLog.hide();
		}
	},

	changeParentOfDragItem: function (tab, targetParent) {
		var originalPlace = tab.parent();
		var originalX = originalPlace.getX();
		var originalY = originalPlace.getY();
		var left = tab.dom.style.left;
		var top = tab.dom.style.top;
		var leftNumber = parseInt(left.replace('px', ''));
		var topNumber = parseInt(top.replace('px', ''));

		targetParent.appendChild(tab);

		var currentX = targetParent.getX();
		var currentY = targetParent.getY();
		var deltaX = currentX - originalX;
		var deltaY = currentY - originalY;
		leftNumber = leftNumber - deltaX;
		topNumber = topNumber - deltaY;
		tab.applyStyles({ position: 'relative', left: leftNumber + "px", top: topNumber + "px" });
		this.deltaSetXY = null;
	},

	createPlaceHolderInMenu: function () {
		var table = Ext.get("menuTable");
		var td = document.createElement("td");
		td.className = "tabPlace";
		Ext.get(td).insertBefore(Ext.get("moreMenuHolder"));
		return td;
	},

	getBacklogPlaceholder: function (id) {
		var placeId = id + " Place";
		var place = Ext.get(placeId);

		if (place != null) {
			place.setVisibilityMode(2);
			place.show();
			return placeId;
		}

		var parts = id.split("-");

		var group = parts[0] + " Action Group";

		var tableGroup = Ext.get(group);

		if (tableGroup == null)
			throw "Group for menu item is not found: " + group;

		var tableBody = tableGroup.select("tbody").elements[0];
		var tr = document.createElement("tr");
		var td = document.createElement("td");
		td.id = placeId;
		tr.appendChild(td);
		tableBody.appendChild(tr);

		tableGroup.setVisibilityMode(Ext.Element.DISPLAY);
		tableGroup.show();

		return placeId;
	},

	onDragDrop: function (ev, targetElementID) {
		var element = this.getEl();
		var backLog = this.getTabsBacklog();
		latestDrugMenuItem = null;

		if (backLog.id == targetElementID && element.isInMenu) {

			var placeId = this.getBacklogPlaceholder(element.id);
			var placeHolder = Ext.get(placeId);

			if (placeHolder == null)
				throw "Place holder for menu is not found: " + placeId;

			var dragItem = Ext.get(element);
			var originalPlaceHolder = dragItem.parent();
			element.isInMenu = false;
			dragItem.removeClass("tab");

			dragItem.addClass("tabToAdd");
			placeHolder.appendChild(dragItem);
			this.removePlaceHolderFromMenu(originalPlaceHolder);
			backLog.removeClass("menuBackLogHighlighted");
		}
	},

	removePlaceHolderFromMenu: function (placeHolder) {
		var parent = placeHolder.parent();
		parent.dom.removeChild(placeHolder.dom);
	},

	endDrag: function () {

		var element = this.getEl();
		var el = Ext.get(element);
		el.clearPositioning();
		this.unmarkAsTabPlaceHolder(el.parent());
		el.removeClass("dragTab");

		if (element.isInMenu) {
			el.select("span").removeClass("additionalMenuTabCustomize");
		}

		var menu = Ext.get("menuTabs");
		menu.addClass("paneledit");
		menu.removeClass("paneledithover");
	},

	markAsPlaceHolder: function (placeHolder) {
		if (this.getEl().isInMenu)
			placeHolder.addClass("tabPlaceHolder");
	},

	unmarkAsTabPlaceHolder: function (placeHolder) {
		placeHolder.removeClass("tabPlaceHolder");
	}
});

Ext.onReady(function() {
	var customizer = new Tp.Menu.Customizer();
	customizer.initialize();
	FixUpDDM();
});

function FixUpDDM() {
	if (Ext.isIE) {
		Ext.EventManager.removeListener(document, 'mousemove', Ext.dd.DragDropMgr.handleMouseMove, Ext.dd.DragDropMgr);
		Ext.EventManager.on(document, "mousemove", handleMouseMoveForMenuOverride, Ext.dd.DragDropMgr, true);
	}
}

//Fix UP Drag And Drop Manager FOR IE
function handleMouseMoveForMenuOverride(e) {
	if (! Ext.dd.DragDropMgr.dragCurrent) {
		return true;
	}

	if (!Ext.dd.DragDropMgr.dragThreshMet) {
		var diffX = Math.abs(Ext.dd.DragDropMgr.startX - e.getPageX());
		var diffY = Math.abs(Ext.dd.DragDropMgr.startY - e.getPageY());
		if (diffX > Ext.dd.DragDropMgr.clickPixelThresh ||
                    diffY > Ext.dd.DragDropMgr.clickPixelThresh) {
            Ext.dd.DragDropMgr.startDrag(Ext.dd.DragDropMgr.startX, Ext.dd.DragDropMgr.startY);
        }
    }

    if (Ext.dd.DragDropMgr.dragThreshMet) {
        Ext.dd.DragDropMgr.dragCurrent.b4Drag(e);
        Ext.dd.DragDropMgr.dragCurrent.onDrag(e);
        if(!Ext.dd.DragDropMgr.dragCurrent.moveOnly){
            Ext.dd.DragDropMgr.fireEvents(e, false);
        }
    }

    Ext.dd.DragDropMgr.stopEvent(e);

    return true;
}
