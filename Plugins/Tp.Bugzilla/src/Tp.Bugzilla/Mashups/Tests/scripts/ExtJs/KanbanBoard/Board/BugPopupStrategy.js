Ext.ns('Tp.controls.kanbanboard.board');

Tp.controls.kanbanboard.board.BugPopupStrategy = Ext.extend(Object, {

	constructor: function (config) {
		Ext.apply(this, config);
	},

	loadData: function (entity, successDelegate, failureDelegate) {
		this.service.loadBugs(entity.id,
                successDelegate,
                failureDelegate);
	},

	getViewUrl: function (value) {
		return Application.getViewUrl(value, 'Bug');
	},

	getEntityName: function () {
		return "bug";
	},

	updateItem: function (item, count) {
		item.updateBugs(count);
	}
});