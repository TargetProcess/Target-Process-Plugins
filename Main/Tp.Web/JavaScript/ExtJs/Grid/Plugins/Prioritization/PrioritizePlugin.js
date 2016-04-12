Ext.ns('Tp.controls.grid.plugins');

Tp.controls.grid.plugins.PrioritizePlugin = Ext.extend(Ext.util.Observable, {
	constructor: function(config) {
		this.prioritizationService = config.prioritizationService;
		Tp.controls.grid.plugins.PrioritizePlugin.superclass.constructor.apply(this, arguments);
	},

	init: function(component) {
		component.on('beforerowmove', Tp.controls.grid.Handlers.getReorderHandler(component.id, this.prioritizationService));
		component.on('onPriorityChanged', this._onPriorityChanged);
	},
	_onPriorityChanged: function(callbackParams) {
		var grid = callbackParams.grid;
		var store = grid.getStore();
		var record = store.getById(callbackParams.recordId);
		record.beginEdit();
		record.set('priority', callbackParams.newPriorityName);
		record.commit();
		record.endEdit();
		if (callbackParams.target) {
			callbackParams.target.unlock();
		}
	}
});
