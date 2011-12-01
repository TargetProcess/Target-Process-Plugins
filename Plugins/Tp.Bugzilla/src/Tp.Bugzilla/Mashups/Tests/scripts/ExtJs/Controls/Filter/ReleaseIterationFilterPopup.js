Ext.ns('Tp.controls.Filter');

Tp.controls.Filter.ReleaseIterationFilterPopup = Ext.extend(Ext.Window, {
	releaseIterationFilterData: null,
	releaseIterationFilterTitle: null,
	selectButton: null,
	closeButton: null,
	plannableFilter: null,

	constructor: function (config) {
		Ext.apply(this, config);

		this.selectButton = new Ext.Button({
			text: 'Select'
		});
		this.selectButton.on("click", this.onSelectButtonClick, this);

		this.closeButton = new Ext.Button({
			text: 'Close'
		});
		this.closeButton.on("click", function () {
			this.hide();
		}, this);

		this.plannableFilter = new Tp.controls.Prioritization.PlannableFilter({
			data: this.releaseIterationFilterData,
			treeHeight: 245,
			border: false
		});

		config = Ext.apply({
			layout: 'fit',
			modal: true,
			width: 400,
			autoHeight: true,
			resizable: true,
			closeAction: 'hide', // Destroy window and child controls on close, remove DOM elements.
			items: this.plannableFilter,
			buttons: [this.selectButton, this.closeButton],
			title: this.releaseIterationFilterTitle
		}, config);

		Tp.controls.Filter.ReleaseIterationFilterPopup.superclass.constructor.call(this, config);
	},

	onSelectButtonClick: function () {
		var filterValues = this.plannableFilter.getFilterValues();
		this.fireEvent('filterSelected', filterValues.filter);
		this.hide();
	},

	restoreFilterValues: function (filterData) {
		if (!filterData)
			return;
		this.plannableFilter.restoreFilterValues(filterData, true);
	}
});
