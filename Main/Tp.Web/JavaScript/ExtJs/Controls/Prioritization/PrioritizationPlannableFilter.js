Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.PrioritizationPlannableFilter = Ext.extend(Tp.controls.Prioritization.PlannableFilter, {
	constructor: function (config) {
		config = Ext.apply({
			items: [{
				xtype: 'panel',
				bodyCssClass: 'x-panel-body-blue filter-panel-area-margin',
				items: [{
					xtype: 'label',
					cls: 'filter-panel-margin',
					text: config.selectTitle
				}]
			}]
		}, config);

		Tp.controls.Prioritization.PrioritizationPlannableFilter.superclass.constructor.call(this, config);
	}
});

Ext.reg('tpplannablefilter', Tp.controls.Prioritization.PrioritizationPlannableFilter);