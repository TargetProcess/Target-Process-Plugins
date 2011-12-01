Ext.ns('Tp.custom.plugins');

Tp.custom.plugins.PassFailColumn = Ext.extend(Ext.Component, {
	header: "Run",
	width: 100,
	sortable: false,
	fixed: true,
	menuDisabled: true,
	dataIndex: '',
	id: 'expander',
	lazyRender: true,
	enableCaching: true,
	_grid: null,

	init: function(grid) {
		this._grid = grid;
		this._grid.on('afterrender', function() {
			this._grid.getView().mainBody.on("click", this.onClick, this);
		}, this);
	},

	_isPassed: function(eventObj) {
		return eventObj.getTarget(".x-btn-passed") == null ? false : true;
	},

	getRecord: function(eventObj) {
		var row = eventObj.getTarget('.x-grid3-row');
		return this._grid.store.getAt(row.rowIndex);
	},

	onClick: function(eventObj, target) {
		if (!eventObj.getTarget('.x-btn-passed-failed'))
			return;

		if (this._isPassed(eventObj)) {
			this.fireEvent("pass", this._grid, this.getRecord(eventObj));
			return;
		}
		this.fireEvent("fail", this._grid, this.getRecord(eventObj));
	},

	renderer: function(v, p, record) {
		return "<button  style='width:45px'  class='x-btn-passed-failed x-btn-passed' type='button'>Pass</button>" +
               "&nbsp; <button style='width:45px' class='x-btn-passed-failed x-btn-failed' type='button'>Fail</button>"
	}
});
