Ext.ns('Tp.controls.menu');

Tp.controls.menu.BasePopup = Ext.extend(Ext.util.Observable, {
	container: null,
	triggerElement: null,

	_checkTriggerElement: function (config) {
		if (!config.triggerElement)
			throw "Tp.controls.menu.BasePopup triggerElement was not configure";

		this.triggerElement = Ext.get(config.triggerElement);
		if (!this.triggerElement)
			throw String.format("Tp.controls.menu.BasePopup triggerElement was not found by {0}", config.triggerElement);
	},

	_createContainer: function (config) {
		this.container = new Ext.Element(document.createElement('div'));
		var brElement = new Ext.Element(document.createElement('br'));
		this.container.insertAfter(this.triggerElement);
		brElement.insertAfter(this.triggerElement);

		if (config.className)
			this.container.addClass(config.className);
	},

	constructor: function (config) {
		Ext.apply(this, config);

		this._checkTriggerElement(config);
		this._createContainer(config);

		this.addEvents({
			"beforeshow": true,
			"aftershow": true,
			"beforehide": true,
			"afterhide": true
		});

		Tp.controls.menu.BasePopup.superclass.constructor.call(this, config);

		Application.getMenuBootsrap().registerPopup(this);
	},

	_triggerElementClickHandler: function () {
		this.toggle();
	},

	getContainerElement: function () {
		return this.container;
	},

	getTriggerElement: function () {
		return this.triggerElement;
	},

	show: function () {
		this.fireEvent("beforeshow");
		this.getContainerElement().show();
		this.fireEvent("aftershow");
	},

	toggle: function () {
		if (!this.isVisible()) {
			this.show();
		}
		else {
			this.hide();
		}
	},

	isVisible: function () {
		return this.getContainerElement().isVisible();
	},

	hide: function () {
		this.fireEvent("beforehide");
		this.getContainerElement().hide();
		this.fireEvent("afterhide");
	},

	addClass: function (className) {
		this.getContainerElement().addClass(className);
	}
});
