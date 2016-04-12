Ext.ns('Tp.controls');

Tp.controls.TpMultiSelect = Ext.extend(Ext.ux.ItemSelector, {

	rightPanelHidden: null,
	containerElement: null,

	constructor: function (config) {
		this.containerElement = Ext.get(config.containerElementId);
		if (Ext.isEmpty(this.containerElement))
			return;

		this.rightPanelHidden = Ext.get(config.rightPanelHiddenId);

		var storeFields = ['Id', 'Name'];
		var leftStore = new Ext.data.JsonStore({
			fields: storeFields,
			data: config.leftPanelData
		});

		var rightStore = new Ext.data.JsonStore({
			fields: storeFields,
			data: config.rightPanelData
		});

		config = Ext.apply({
			fromStore: leftStore,
			toStore: rightStore,
			displayField: 'Name',
			valueField: 'Id',
			drawUpIcon: false,
			drawDownIcon: false,
			drawTopIcon: false,
			drawBotIcon: false,
			msWidth: 337,
			msHeight: 204,
			toHeader: 'Projects in this program',
			toHeaderCls: 'defaultLabel',
			fromHeader: 'Available projects',
			fromHeaderCls: 'defaultLabel',
			applyTo: config.containerElementId
		}, config);

		Tp.controls.TpMultiSelect.superclass.constructor.call(this, config);

		this.on('change', Function.createDelegate(this, this.onStateChange))
		this.containerElement.setWidth(this.containerElement.getWidth() + 80);
	},

	onStateChange: function (sender, value) {
		this.rightPanelHidden.dom.value = value;
	}
});