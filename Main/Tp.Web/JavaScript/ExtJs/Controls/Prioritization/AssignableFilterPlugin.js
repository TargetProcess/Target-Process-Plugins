Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.AssignableFilterPlugin = function () {
	this.init = function(component) {
		this.component = component;
		this.component.showEntitiesButton.on('click', this.onFilterClick, this);
		this.component.cleanButton.on('click', this.onCleanClick, this);
	};

	this.onFilterClick = function () {
		this.component.fireEvent('filterChanged', this.component.getFilterValues());
	};

	this.onCleanClick = function () {
		this.component.setFilterDefaultValues();
	};
};

Ext.reg('tpassignablefilterplugin', Tp.controls.Prioritization.AssignableFilterPlugin);
