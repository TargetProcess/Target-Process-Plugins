Ext.ns('Tp.custom.plugins');

Tp.custom.plugins.PassFailColumn = Ext.extend(Ext.Component, {
	enableCaching: true,
	menuDisabled: true,
	lazyRender: true,
	sortable: false,
	fixed: true,
	id: 'expander',
	header: 'Run',
	dataIndex: '',
	width: 100,
	_grid: null,

	init: function(grid) {
		this._grid = grid;
		this._grid.on('afterrender', function() {
			grid.getView().mainBody.on('click', this.onClick, this);
		}, this);
	},

	_isPassed: function(eventObj) {
		return eventObj.getTarget('.success') == null ? false : true;
	},

	getRecord: function(eventObj) {
		var row = eventObj.getTarget('.x-grid3-row');
		return this._grid.store.getAt(row.rowIndex);
	},

	onClick: function(eventObj) {
		if (!eventObj.getTarget('.button'))
			return;

		if (this._isPassed(eventObj)) {
			this.fireEvent('pass', this._grid, this.getRecord(eventObj));
			return;
		}
		this.fireEvent('fail', this._grid, this.getRecord(eventObj));
	},

	renderer: function() {
		return "<div class='button-group'><button class='button success' type='button'>Pass</button>" +
			"<button class='button danger' type='button'>Fail</button></div>";
	}
});
