Ext.ns('Tp.controls.kanbanboard.board');

Tp.controls.kanbanboard.board.TaskPopupStrategy = Ext.extend(Object, {

	constructor: function (config) {
		Ext.apply(this, config);
	},

	loadData: function (entity, successDelegate, failureDelegate) {
		this.service.loadTasks(entity.id,
                successDelegate,
                failureDelegate);
	},

	getViewUrl: function (value) {
		return Application.getViewUrl(value, 'Task');
	},

	getEntityName: function () {
		return "task";
	},

	updateItem: function (item, count) {
		item.updateTasks(count);
	}
});