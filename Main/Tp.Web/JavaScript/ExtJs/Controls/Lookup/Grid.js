Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.Grid = Ext.extend(Ext.grid.GridPanel, {
	constructor: function (config) {
		var storeFields = [{name: 'Icon'}];
		Ext.each(config.columns, function (column) {
			storeFields.push({ name: column.dataIndex });
		});

		var store = new Tp.controls.Lookup.Store({
			fields: storeFields,
			proxy: new Tp.controls.Lookup.JsonProxy({
				method: 'POST',
				url: new Tp.WebServiceURL('/PageServices/LookupService.asmx/GetData').toString(),
				api: {
					load: { url: new Tp.WebServiceURL('/PageServices/LookupService.asmx/GetData').toString(), method: 'POST' }
				}
			})
		});

		store.on('beforeload', function (store, options) {
			Ext.apply(options.params, {
				filter: this.getTopToolbar().getFilterValues(),
				columns: this.getColumns()
			});
		}, this);

		var config = Ext.apply({
			stateful: true,
			store: store,
			viewConfig: {
				forceFit: true,
				enableRowBody: true,
				listeners: {
					refresh: function (view) {
						var links = view.el.query('a.lookupActionLink');
						Array.forEach(links, function (link) {
							Ext.get(link).on('click', Function.createDelegate(this, function (e, el) {
								var row = this.findRow(el);
								var record = this.grid.store.getAt(row.rowIndex);
								this.grid.fireEvent('lookupactionlinkclick', e, el, this.grid, record, row, this.findCell(el));
								e.preventDefault();
							}));
						}, this);
					}
				}
			},
			listeners: {
				afterrender: {
					fn: function () {
						this.getTopToolbar().on('filterChanged', this.reload, this);
						this.reload();
					},
					scope: this
				}
			},
			bbar: {
				xtype: 'paging',
				store: store,
				displayInfo: true,
				pageSize: 20,
				style:{zIndex:10000012}
			}
		}, config);

		Tp.controls.Lookup.Grid.superclass.constructor.call(this, config);
	},

	getColumns: function () {
		var result = ['Icon'];
		var cm = this.getColumnModel();
		for (var i = 0; i < cm.getColumnCount(); i++) {
			var c = cm.getColumnAt(i);
			result.push(c.dataIndex);
		}
		return result;
	},

	reload: function () {
		this.getStore().load({ params: { start: 0, limit: 20} });
	}
});
