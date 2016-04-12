Ext.ns('Tp.controls.grid.plugins');

Tp.controls.grid.plugins.ChangeStatePlugin = Ext.extend(Ext.util.Observable, {
	constructor: function (config) {
		this.testCaseService = config.testCaseService;
		Tp.controls.grid.plugins.ChangeStatePlugin.superclass.constructor.apply(this, arguments);
	},

	init: function (component) {
		component.on('entityStateChanged', this._onEntityStateChanged);
	},
	_onEntityStateChanged: function (callbackParams) {
		var grid = callbackParams.grid;
		var store = grid.getStore();
		var record = store.getById(callbackParams.recordId);
		record.beginEdit();
		record.set('LastRunDate', callbackParams.newLastRunDate);
		record.set('LastStatus', callbackParams.newLastStatus);
		record.commit();
		record.endEdit();
	}
});