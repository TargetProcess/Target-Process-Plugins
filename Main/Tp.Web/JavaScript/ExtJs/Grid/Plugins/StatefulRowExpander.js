Ext.ns('Tp.controls.grid.plugins');
//this expander is supposed to be stateful but currently it just collapses all rows in case of moving columns regardless of state(Expanded/Collapsed)
Tp.controls.grid.plugins.StatefulRowExpander = Ext.extend(Ext.grid.RowExpander, {
	movable: false,
	init: function () {
		Tp.controls.grid.plugins.StatefulRowExpander.superclass.init.apply(this, arguments);
		this.grid.on('columnmove', this._onRestoreStates, this);
		this.grid.getStore().on('load', this._onRestoreStates, this);
		this.grid.on('afterrender', function () {
			this.grid.getView().mainBody.on("dblclick", this._onDblClick, this);
		}, this);
		this.grid.on('roworderchanged', this._onRowOrderChanged, this);
	},

	_onDblClick: function (eventObj) {
		if (eventObj.getTarget('.x-grid3-row-body-tr') == null)
			return;

		var row = eventObj.getTarget('.x-grid3-row');
		var d = this.grid.store.getAt(row.rowIndex).data;
		this.grid.fireEvent('expandedareadblclick', d['id'], d['Project.ProcessID']);
	},

	_onRowOrderChanged: function (rowIndex) {
		this.collapseRow(rowIndex);
	},

	_onRestoreStates: function () {
		var store = this.grid.getStore();
		var rows = this.grid.getView().getRows();
		for (var i = 0; i < rows.length; i++) {

			var row = rows[i];
			var recordId = store.getAt(row.rowIndex).id;
			if (this.state[recordId]) {
				this.expandRow(row);
			}
			else {
				this.collapseRow(row);
			}
		}
	}
});