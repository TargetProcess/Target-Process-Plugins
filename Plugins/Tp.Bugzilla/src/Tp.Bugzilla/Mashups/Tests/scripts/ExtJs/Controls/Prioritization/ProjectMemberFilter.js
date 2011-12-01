Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.ProjectMemberFilter = Ext.extend(Ext.form.ComboBox, {
	constructor: function (config) {
		var config = Ext.apply({
			triggerAction: 'all',
			autoSelect: false,
			lazyRender: true,
			editable: false,
			mode: 'local',
			value: 0,
			valueField: 'UserId',
			displayField: 'UserName',
			listeners: {
				beforequery: {
					fn: function (e) {
						if (e.combo.view.selected.elements.length == 0 || e.combo.view.selected.elements[0].viewIndex != e.combo.selectedIndex) {
							e.combo.store.each(function (r, i) {
								if (r.data[e.combo.valueField] == e.combo.value) {
									e.combo.select(i, false);
								}
							}, this);
						}
					}
				}
			}
		}, config);
		Tp.controls.Prioritization.ProjectMemberFilter.superclass.constructor.call(this, config);
	},

	restoreFilterValues: function (filter) {
		this.store.each(function (r, i) {
			if (r.data[this.valueField] == filter) {
				this.setValue(filter);
			}
		}, this);
	},

	setFilterDefaultValues: function () {
		this.setValue(0);
	}
});

Ext.reg('projectmemberfilter', Tp.controls.Prioritization.ProjectMemberFilter);