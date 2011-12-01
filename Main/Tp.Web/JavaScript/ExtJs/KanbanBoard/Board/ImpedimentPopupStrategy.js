Ext.ns('Tp.controls.kanbanboard.board');

Tp.controls.kanbanboard.board.ImpedimentPopupStrategy = Ext.extend(Object, {
	constructor: function (config) {
		Ext.apply(this, config);
	},

	loadData: function (entity, successDelegate, failureDelegate) {
		this.service.loadImpediments(entity.id,
				successDelegate,
				failureDelegate);
	},

	getViewUrl: function (value) {
		return Application.getViewUrl(value, 'Impediment');
	},

	getEntityName: function () {
		return "impediment";
	},

	updateItem: function (item, count) {
		item.updateImpediments(count);
	}
});