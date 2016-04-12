Ext.ns('Tp.controls.grid');

Tp.controls.grid.AuditHistoryFieldFactory = Ext.extend(Object, {

	createEditor: function (columnConfig) {
		return null;
	},

	createRenderer: function (columnConfig) {
		columnConfig.movable = false;

		if (columnConfig.Type == "date") {
			return Ext.util.Format.dateRenderer('d-M-Y ' + extJsTimeFormat);
		}

		if (columnConfig.Type == "HTMLText") {
			return Ext.util.Format.htmlEncode;
		}
		return null;
	}
});
