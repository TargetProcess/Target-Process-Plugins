Ext.ns('Tp.controls.grid');

Tp.controls.grid.TestCaseConfigurationFactory = Ext.extend(Tp.controls.grid.ConfigurationFactory,
{
	pageSize: 20,
	createConfig: function () {
		var config = Tp.controls.grid.TestCaseConfigurationFactory.superclass.createConfig.call(this);
		Tp.collections.createMixedCollection(config.columns).each(function (item) {
			item.sortable = false;
		});

		config.header = false;
		config.sm = new Ext.grid.RowSelectionModel({ singleSelect: true });

		var expander = new Tp.custom.plugins.TestCaseDetailsExpander();
		var passFail = new Tp.custom.plugins.PassFailColumn();

		var passFailController = new Tp.controls.grid.custom.TestCasePassFailController();
		passFail.on("pass", function (grid, record) {
			passFailController.changeState(grid, record, true)
		});
		passFail.on("fail", function (grid, record) {
			passFailController.changeState(grid, record, false)
		});

		config.columns.unshift(expander);
		config.columns.push(passFail);
		config.plugins.push(expander);
		config.plugins.push(passFail);

		config.plugins.push(new Tp.controls.grid.plugins.EditTestCaseOpenByDoubleClick({ onSaveCallBack: Tp.controls.grid.Handlers.refreshGrid(this.initialConfig.id) }));

		config.plugins.push(new Tp.controls.grid.plugins.GridDragDropRowOrder({ copy: false, scrollable: true, targetCfg: {} }));
		config.plugins.push(new Tp.controls.grid.plugins.PrioritizePlugin({ prioritizationService: Tp.Web.Extensions.ExtJs.TestCaseListService }));
		config.plugins.push(new Tp.controls.grid.plugins.ChangeStatePlugin({ testCaseService: Tp.Web.Extensions.ExtJs.TestCaseListService }));
		config.cm = this.createColumnModel(config.columns);
		config.bbar = this.createPagingToolBar(config.store);

		expander.on('onCompleteLoading', function () { this.onExpandedAndLoaded() });

		return config;
	},

	onExpandedAndLoaded: function () {
	},

	getPageSize: function () {
		return this.pageSize;
	}
});
