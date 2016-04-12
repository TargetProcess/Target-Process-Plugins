Ext.ns('Tp.controls.grid');
Tp.controls.grid.ParentListConfigurationFactory = Ext.extend(Tp.controls.grid.GeneralConfigurationFactory, {
	createConfig: function (sourceConfig) {
		var config = Tp.controls.grid.ParentListConfigurationFactory.superclass.createConfig.call(this, sourceConfig);
		if (config.innerGridConfig.length != 0) {
			var expander = new Tp.controls.grid.plugins.GridExpander(config);
			config.columns.unshift(expander);
			config.cm = this.createColumnModel(config.columns);
			config.plugins.push(expander);
		}

		config.view = this.createView();
		config.plugins.push(new Tp.controls.grid.plugins.ReportEditor());
		return config;
	},

	createView: function (columnModel) {
		return new Tp.controls.grid.SummaryGroupingView({
			cm: columnModel,
			forceFit: true,
			hideGroupedColumn: false,
			groupTextTpl: '{text}'
		});
	}
});