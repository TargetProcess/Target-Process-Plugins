Ext.ns('Tp.controls.grid');
Tp.controls.grid.ReportConfigurationFactory = Ext.extend(Tp.controls.grid.ParentListConfigurationFactory, {
	createConfig: function() {
		var config = Tp.controls.grid.ReportConfigurationFactory.superclass.createConfig.call(this);
		config.tbar = [];
		config.tbar.push({ id: "btnGridPrint", text: 'Printable View', disabled: true, handler: Tp.controls.grid.Handlers.getPrintReportHandler(config.id) });
		config.tbar.push("-");
		config.tbar.push({ id: "btnGridExport", text: 'Export To Excel', disabled: true, handler: Tp.controls.grid.Handlers.getReportExportToExcelHandler(config.id) });
		config.tbar.push("-");
		config.tbar.push({ text: 'Edit Report', tooltip: 'Change report configuration', handler: Tp.controls.grid.Handlers.getReportEditHandler(config.id), disabled: !this.initialConfig.canEdit });
		config.stateful = false;
		return config;
	}
});