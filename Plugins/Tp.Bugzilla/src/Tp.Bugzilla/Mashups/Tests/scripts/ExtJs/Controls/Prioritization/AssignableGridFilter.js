Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.AssignableGridFilter = Ext.extend(Ext.Panel, {
	constructor: function (config) {
		this.showEntitiesButton = new Ext.Button({
			text: 'Show'
		});

		this.cleanButton = new Ext.Button({
			text: 'Clean'
		});

		config = Ext.apply({
			stateId: 'AssignableGridFilter',
			stateEvents: ['filterChanged', 'collapse', 'expand'],
			getState: function () {
				return { filterValues: this.getFilterValues(), collapsed: this.collapsed };
			},
			applyState: function (value) {
				this.collapsed = value.collapsed;
				this.restoreFilterValues(value);
			},
			layout: 'vbox',
			layoutConfig: { padding: '0 0 0 0' },
			listeners: {
				afterrender: function (sender) {
					//this.updateItems();
				},
				scope: this
			},
			items: [
				{
					xtype: 'projectmemberfilter',
					margins: '5px 0px 0px 10px',
					emptyText: 'User',
					store: new Ext.data.JsonStore({
						data: config.staticMetadata.userTypes,
						fields: [{ name: 'UserId' }, { name: 'UserName'}]
					})
				},
				{
					xtype: 'tptagsfilter',
					border: false,
					padding: '0px 0px 10px 0px'
				},
				{
					id: 'assignablefilter',
					xtype: 'tpassignablefilter',
					store: new Tp.controls.Prioritization.AssignableTypeJsonStore({ data: config.staticMetadata.assignableTypes }),
					border: false
				},
				{
					xtype: 'tpplannablefilter',
					data: config.staticMetadata.planableFilter,
					selectTitle: config.staticMetadata.planableFilterTitle,
					border: false,
					buttons: [this.showEntitiesButton, this.cleanButton],
					buttonAlign: 'center',
					width: 200
				}
			],
			keys: [
				{
					key: [10, 13], // enter
					scope: this,
					stopEvent: true,
					fn: function () {
						this.fireEvent('filterChanged', this.getFilterValues());
					}
				}
			],
			title: 'Filter',
			plugins: new Tp.controls.Prioritization.AssignableFilterPlugin()
		}, config);

		Tp.controls.Prioritization.AssignableGridFilter.superclass.constructor.call(this, config);
	},

	updateItems: function () {
		this.fireEvent('filterChanged', this.getFilterValues());
	},

	onLayout: function () {
		Tp.controls.Prioritization.AssignableGridFilter.superclass.onLayout.apply(this, arguments);
		if (this.hasLayout) {
			var filter = this.findByType('tpplannablefilter')[0];
			if (filter && filter.treePanel) {
				var tb = filter.toolbars[0], tp = filter.treePanel;
				tp.setHeight((this.container.getHeight() + this.container.getY() - tp.getPosition()[1] - (tb != null ? tb.getHeight() : 0) - 10));
			}
		}
	},

	restoreFilterValues: function (filterData) {
		if (!filterData)
			return;
		this.findByType('projectmemberfilter')[0].restoreFilterValues(filterData.filterValues.peopleFilter);
		this.findByType('tpassignablefilter')[0].restoreFilterValues(filterData.filterValues.assignableFilter);
		this.findByType('tpplannablefilter')[0].restoreFilterValues(filterData.filterValues.plannableFilter, false);
		this.findByType('tptagsfilter')[0].restoreFilterValues(filterData.filterValues.tagsFilter);
	},

	getFilterValues: function () {
		return {
			peopleFilter: this.findByType('projectmemberfilter')[0].getValue(),
			assignableFilter: this.findByType('tpassignablefilter')[0].getFilterValues().filter,
			plannableFilter: this.findByType('tpplannablefilter')[0].getFilterValues().filter,
			tagsFilter: this.findByType('tptagsfilter')[0].getFilterValues().filter
		};
	},

	setFilterDefaultValues: function () {
		this.findByType('projectmemberfilter')[0].setFilterDefaultValues();
		this.findByType('tpassignablefilter')[0].setFilterDefaultValues();
		this.findByType('tpplannablefilter')[0].setFilterDefaultValues();
		this.findByType('tptagsfilter')[0].setFilterDefaultValues();
		this.updateItems();
	}
});

Ext.reg('tpassignablegridfilter', Tp.controls.Prioritization.AssignableGridFilter);
