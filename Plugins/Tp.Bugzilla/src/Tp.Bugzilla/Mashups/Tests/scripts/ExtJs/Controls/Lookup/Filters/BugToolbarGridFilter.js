Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.BugToolbarGridFilter = Ext.extend(Tp.controls.Lookup.ToolbarGridFilter, {
	constructor: function (config) {
		var config = Ext.apply({
			entityTypeIds: [8],
			items: [
			{
				xtype: 'textfield',
				name: 'ID',
				emptyText: 'ID',
				cls: 'x-text-search-item',
				width: 100
			}, {
				xtype: 'textfield',
				name: 'Name',
				emptyText: 'Name',
				cls: 'x-text-search-item',
				width: 10
			}, {
				xtype: 'textfield',
				name: 'State',
				emptyText: 'State',
				cls: 'x-text-search-item',
				width: 80
			}]
		}, config);
		Tp.controls.Lookup.BugToolbarGridFilter.superclass.constructor.call(this, config);
	}
});

Ext.reg('bugtoolbargridfilter', Tp.controls.Lookup.BugToolbarGridFilter);