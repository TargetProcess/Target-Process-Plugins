Ext.ns('Tp.controls.grid');

Tp.controls.grid.ConfigurationFactory = Ext.extend(Object, {

	initialConfig: null,

	constructor: function(config) {
		this.initialConfig = config;
	},

	createColumns: function(gridHeader, gridDataRequest) {
		Tp.util.validateForNulls([gridHeader, gridDataRequest]);
		var columns = new Array();
		for (var position = 0; position < gridHeader.Columns.length; position++) {
			var columnConfig = gridHeader.Columns[position];
			if (!columnConfig.IsSynthetic) {
				var gridColumn = {
					id: columnConfig.Id,
					header: columnConfig.Title,
					tooltip: columnConfig.Title,
					resizable: true,
					sortable: true,
					dataIndex: columnConfig.Id,
					editable: columnConfig.IsUpdateable
				};
				if (columnConfig.Width) {
					gridColumn.width = columnConfig.Width;
				}

				var fieldFactory = Tp.controls.grid.Factory.getInstance().getFieldFactory(gridDataRequest);
				gridColumn.editor = fieldFactory.createEditor(columnConfig);
				var renderer = fieldFactory.createRenderer(columnConfig, this.initialConfig);
				if (renderer != null) {
					gridColumn.renderer = renderer;
				}
				columns.push(gridColumn);
			}
		}
		return columns;
	},

	createColumnModel: function(columns) {
		return new Ext.grid.ColumnModel(columns);
	},

	createPlugins: function(pluginTypes) {
		if (pluginTypes == null) {
			return [];
		}

		if (pluginTypes.length == 0) {
			return [];
		}

		var strArr = pluginTypes.split(',');
		var result = [];
		for (i = 0; i < strArr.length; i++) {
			var plugin = eval("new " + strArr[i]);
			result.push(plugin);
		}
		return result;
	},

	getPageSize: function() {
		return this.initialConfig.gridDataRequest.Limit;
	},

	createPagingToolBar: function(store) {
		var pageSizeCount = this.getPageSize();
		var pagingToolbar = new Ext.PagingToolbar({
			pageSize: pageSizeCount,
			store: store,
			cls: 'x-pager',
			displayInfo: true,
			displayMsg: 'Displaying records {0} - {1} of {2}',
			emptyMsg: "No records to display"
		});
		return pagingToolbar;
	},

	createView: function() {
		return new Ext.grid.GridView({ forceFit: true, emptyText: 'No items found.' });
	},

	// We re-initialize options here, not sure why they lost...
	addAdditionalParameters: function(store, records, options) {
		store.lastOptions = options;
	},

	createConfig: function() {
		var config = Tp.util.clone(this.initialConfig);
		config.gridDataRequest.Limit = this.getPageSize();
		var store = Tp.controls.grid.Factory.getInstance().getStoreFactory().createStore(config.gridDataRequest, config.gridHeader);
		store.on("load", this.addAdditionalParameters, this);
		config.store = store;
		var columns = this.createColumns(this.initialConfig.gridHeader, config.gridDataRequest);
		config.columns = columns;
		config.sm = new Ext.grid.RowSelectionModel({ singleSelect: false });
		config.cm = this.createColumnModel(columns);
		config.plugins = this.createPlugins(config.pluginTypes);
		config.stripeRows = true;
		config.loadMask = true;
		config.enableColumnHide = false;
		config.view = this.createView();
		config.title = Ext.util.Format.htmlEncode(config.gridHeader.Title);
		config.autoHeight = true;
		config.enableHdMenu = false;
		config.frame = false;
		return config;
	}
});
