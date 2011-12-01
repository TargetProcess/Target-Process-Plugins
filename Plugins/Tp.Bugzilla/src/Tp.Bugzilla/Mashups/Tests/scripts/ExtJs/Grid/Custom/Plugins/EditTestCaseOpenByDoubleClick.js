Ext.ns('Tp.controls.grid.plugins');

Tp.controls.grid.plugins.EditTestCaseOpenByDoubleClick = Ext.extend(Object, {
	_form: null,

	constructor: function(config) {
		Ext.apply(this, config);
	},

	init: function(grid) {
		grid.on('celldblclick', this._onEditTestCase, this);
		grid.on('expandedareadblclick', this._onExpandedAreaDblClick, this);
	},

	_onExpandedAreaDblClick: function(recordId, processId) {
		this.showForm(recordId, processId);
	},

	_onEditTestCase: function(grid, rowIndex, columnIndex, eventObj) {
		var targetElement = eventObj.getTarget();
		if (targetElement.tagName.toLowerCase() == 'button') {
			return;
		}
		var record = grid.store.getAt(rowIndex);
		this.showForm(record.id, record.data['Project.ProcessID']);
	},

	showForm: function(testCaseId, processId) {
		this._form = new Tp.controls.TestCaseEditorWindow({'testCaseId': testCaseId, 'processId': processId});
		this._form.on("saved", this.onSaveCallBack);
		this._form.show();
	},

	_properCellToStartEditClicked: function(grid, columnIndex) {
		return grid.colModel.getColumnAt(columnIndex).id == 'Name';
	}
});
