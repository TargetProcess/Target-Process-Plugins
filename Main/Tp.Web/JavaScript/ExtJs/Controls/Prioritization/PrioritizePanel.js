Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.PrioritizePanel = Ext.extend(Ext.Panel, {
	constructor: function (config) {
		var proxy = Ext.isEmpty(config.proxy) ?
                new Ext.data.HttpProxy({ url: new Tp.WebServiceURL('/Project/Planning/Iteration/ExtJsPrioritize.aspx').toString() })
                : config.proxy;

		var assignableGridFilter = new Tp.controls.Prioritization.AssignableGridFilter({
			floatable: false,
			region: "west",
			id: "assignableGridFilter",
			staticMetadata: config.staticMetadata,
			proxy: proxy,
			width: 200,
			collapsible: true
		});

		config = Ext.apply(
        {
        	border: false,
        	layout: 'border',
        	autoHeight: true,
        	bodyCssClass: "x-panel-body-white",
        	items: [
                assignableGridFilter,
                new Tp.controls.Prioritization.AssignableGridPanel({
                	id: "assignableGridPanel",
                	region: "center",
                	filter: Ext.getCmp("assignableGridFilter"),
                	width: 600,
                	margins: '0 0 0 5'
                })]
        }, config);

		Tp.controls.Prioritization.PrioritizePanel.superclass.constructor.call(this, config);
		assignableGridFilter.updateItems();
	},

	onLayout: function () {
		Tp.controls.Prioritization.PrioritizePanel.superclass.onLayout.apply(this, arguments);
		if (this.autoHeight === true) {
			this.autoHeight = false;
			this.setHeight(this.container.parent().getHeight());
		}
	},

	getAssignableGridFilter: function () {
		return this.findByType('tpassignablegridfilter')[0];
	},

	getAssignableGrid: function () {
		return this.findByType('tpassignablegridpanel')[0];
	}
});
