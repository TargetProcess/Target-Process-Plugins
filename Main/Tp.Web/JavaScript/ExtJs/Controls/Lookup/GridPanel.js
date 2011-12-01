Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.GridPanel = Ext.extend(Ext.Panel, {
	grid: null,
	constructor: function (config) {
		this.grid = new Tp.controls.Lookup.Grid(config.gridConfig);
		var config = Ext.apply({
			border: false,
			layout: 'border',
			height: 450,
			bodyCssClass: 'x-panel-body-white',
			items: [
				this.grid
			]
		}, config);
		this.grid.on('lookupactionlinkclick', this.gridActionLinkClick, this);
		Tp.controls.Lookup.GridPanel.superclass.constructor.call(this, config);
	},

	gridActionLinkClick: function (e, el, grid, record, row, cell) {
		this.fireEvent('lookupactionlinkclick', e, el, grid, record, row, cell);
	}
});

Ext.reg('lookupgridpanel', Tp.controls.Lookup.GridPanel);
