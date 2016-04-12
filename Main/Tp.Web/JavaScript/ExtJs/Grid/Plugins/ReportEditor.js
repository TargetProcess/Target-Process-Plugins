Ext.ns('Tp.controls.grid.plugins');

Tp.controls.grid.plugins.ReportEditor = Ext.extend(Object, {
	grid: null,
	INNER_GRID_PREFIX: "INNER:",

	init: function (grid) {
		this.grid = grid;
		this.grid.on("reconfiguring", this.onReconfigure, this);
		this.grid.on("render", this.onRender, this);
	},

	onRender: function () {
		// empty column set indicates grid has not been configured yet, and report is empty,
		// so show up report editor for this grid
		if (this.grid.colModel.getColumnCount() == 0) {
			var reportGridEditor = new Tp.Ajax.ReportGridEditor(this.grid);
			reportGridEditor.show();
		} else {
			// else enable basic report commands
			this.grid.topToolbar.items.get("btnGridPrint").enable();
			this.grid.topToolbar.items.get("btnGridExport").enable();
		}
	},

	onReconfigure: function (gridHeader, limitOfRecords, arg) {
		var isGroupFieldFound = false;
		if (this.grid.gridDataRequest.GroupBy != "" && this.grid.gridDataRequest.GroupBy != null) {
			for (var i = 0; i < gridHeader.Columns.length; i++) {
				if (gridHeader.Columns[i].Id == this.grid.gridDataRequest.GroupBy) {
					isGroupFieldFound = true;
					break;
				}
			}
			if (!isGroupFieldFound) {
				this.grid.gridDataRequest.GroupBy = null;
			}
		}
		if (limitOfRecords) {
			this.grid.gridDataRequest.Limit = limitOfRecords;
		}

		var configFactory = Tp.controls.grid.Factory.getInstance().getConfigurationFactory();
		var columns = configFactory.createColumns(gridHeader, this.grid.gridDataRequest);
		this.reconfigureInnerGrids(arg, columns);
		var colModel = configFactory.createColumnModel(columns);
		var storeFactory = Tp.controls.grid.Factory.getInstance().getStoreFactory();
		var store = storeFactory.createStore(this.grid.gridDataRequest, gridHeader);
		var gridWidth = this.grid.getWidth();
		this.grid.reconfigure(store, colModel);
		this.grid.setWidth(gridWidth);
		this.grid.store.on("beforeload", this.grid.onStoreBeforeLoad, this.grid);
		this.grid.getBottomToolbar().bind(store);
		this.grid.getBottomToolbar().pageSize = limitOfRecords;
		this.grid.setTitle(gridHeader.Title);
		this.grid.getView().fitColumns();
		this.grid.fireEvent("initializeEvents");

		// else enable basic report commands
		this.grid.topToolbar.items.get("btnGridPrint").enable();
		this.grid.topToolbar.items.get("btnGridExport").enable();
	},

	reconfigureInnerGrids: function (arg, columns) {
		if (arg.InnerReports.length == 0)
			return;
		if (arg.InnerReports.length != this.grid.innerGridConfig.length) {
			this.refresh();
			return;
		}
		for (var i = 0; i < this.grid.innerGridConfig.length; i++) {
			var customReportID = this.getInnerReportId(this.grid.innerGridConfig[i].gridDataRequest.GridID);
			var isFound = false;
			for (var j = 0; j < arg.InnerReports.length; j++) {
				if (arg.InnerReports[j].ID == customReportID)
					isFound = true;
			}
			if (!isFound) {
				this.refresh();
				return;
			}
		}
		var expander = new Tp.controls.grid.plugins.GridExpander()
		columns.unshift(expander);
	},

	getInnerReportId: function (innerCustomReportId) {
		if (innerCustomReportId.indexOf(this.INNER_GRID_PREFIX) == -1)
			throw "Inner report prefix is missed";
		return innerCustomReportId.substring(this.INNER_GRID_PREFIX.length);
	},

	refresh: function () {
		var appendParam = "reconfigure=true";
		var url = document.location.href;

		if (url.contains("?"))
			appendParam = "&" + appendParam;
		else
			appendParam = "?" + appendParam;

		if (url.contains("reconfigure=true"))
			document.location.href = url;
		else
			document.location.href = url + appendParam
	}
});