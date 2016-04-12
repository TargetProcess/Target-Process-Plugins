Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.TagsFilter = Ext.extend(Ext.Panel, {
	constructor: function (config) {
		this.tagNamesTextField = new Ext.form.TextField({
			name: 'tagNames',
			id: 'tagNamesTextField',
			emptyText: 'Tags',
			width: '166px',
			listeners: {
				afterrender: function (sender) {
					this.restoreFilterValues(this.savedFilterState);
				},
				scope: this
			},
			value: ''
		});

		config = Ext.apply({
			cls: 'filter-panel-margin filter-panel-area-margin',
			autoHeight: true,
			layout: 'table',
			layoutConfig: {
				columns: 1
			},
			items: [this.tagNamesTextField]
		}, config);

		Tp.controls.Prioritization.TagsFilter.superclass.constructor.call(this, config);
	},

	getFilterValues: function () {
		var tagNames = new String(this.tagNamesTextField.getValue());

		var result = {
			filter: {
				tags: tagNames.trim()
			}
		};
		return result;
	},

	savedFilterState: null,

	restoreFilterValues: function (filter) {
		if (!filter)
			return;
		if (this.savedFilterState == null) {
			this.savedFilterState = filter;
			return
		}
		filter = this.savedFilterState;

		this.tagNamesTextField.setValue(filter.tags);
	},

	setFilterDefaultValues: function () {
		this.tagNamesTextField.setValue('');
	}
});

Ext.reg('tptagsfilter', Tp.controls.Prioritization.TagsFilter);