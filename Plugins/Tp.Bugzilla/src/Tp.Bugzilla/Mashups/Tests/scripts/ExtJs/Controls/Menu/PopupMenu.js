Ext.ns('Tp.controls.menu');

Tp.controls.menu.PopupMenu = Ext.extend(Tp.controls.menu.BasePopup, {
	popupTabClassName: 'popupTab',

	constructor: function (config) {
		Tp.controls.menu.PopupMenu.superclass.constructor.call(this, config);

		this.on("aftershow", this.onShowHandler, this);
		this.on("afterhide", this.onHideHandler, this);
		
	},

	_createContainer: function (config) {
	},

	getContainerElement: function () {
		return Ext.get(this.containerElement);
	},

	onShowHandler: function () {
		this.getTriggerElement().parent().addClass(this.popupTabClassName);
	},

	onHideHandler: function () {
		if (!this.getTriggerElement().hasClass("selectedTab"))//This line is an IE7 Workaround to not hide selection when the more tab is selected...
			this.getTriggerElement().parent().removeClass(this.popupTabClassName);
	},

	init: function () {
	}
});