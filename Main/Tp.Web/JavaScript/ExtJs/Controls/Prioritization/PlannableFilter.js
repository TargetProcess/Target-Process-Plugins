Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.PlannableFilter = Ext.extend(Ext.Panel, {
	constructor: function (config) {
		this.treeArray = [];
		for (var i = 0; i < config.data.length; i++) {
			var releaseTreePanel = new Tp.controls.Prioritization.ReleaseTreePanel({ data: config.data[i] });
			this.treeArray.push(releaseTreePanel);
		}

		config = Ext.apply({
			style: { borderBottom: '1px solid #EEEEEE' },
			autoHeight: true,
			cls: 'planable-filter',
			treeHeight: 'auto',
			items: []
		}, config);

		config.items.push({
			xtype: 'panel',
			height: config.treeHeight,
			items: this.treeArray,
			autoScroll: true,
			ref: 'treePanel'
		});
		Tp.controls.Prioritization.PlannableFilter.superclass.constructor.call(this, config);
	},

	getFilterValues: function () {
		return Tp.controls.Prioritization.PlannableFilterData.GetFilterDataFromTree(this.treeArray);
	},

	restoreFilterValues: function (filterArray, skipRestoringSavedFilter) {
		Array.forEach(this.treeArray, function (tree) {
			var treeFilter = Array.findOne(filterArray, function (filter) {
				return filter.projectId == tree.projectId;
			});
			skipRestoringSavedFilter ? tree.restoreValuesFrom(treeFilter) : tree.restoreFilterValues(treeFilter);
		});
	},

	setFilterDefaultValues: function () {
		Array.forEach(this.treeArray, function (tree) {
			tree.setFilterDefaultValues();
		});
	}
});