Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.ToolbarGridFilter = Ext.extend(Ext.Toolbar, {
	entityTypeIds: [],
	excludeEntityId: null,
	projectId: null,
	constructor: function (config) {
		config.items.push('->');
		config.items.push({
			xtype: 'button',
			cls: 'x-btn-text-icon x-btn-over',
			icon: appHostAndPath + '/javascript/tau/css/images/search-icon.png',
			text: 'Search',
			ref: 'searchBtn',
			listeners: {
				click: {
					fn: function () {
						this.fireEvent('filterChanged');
					},
					scope: this
				},
				mouseout: {
					fn: function () {
						this.searchBtn.addClass('x-btn-over');
					},
					scope: this
				}
			}
		});
		var config = Ext.apply({
			cls: 'search-toolbar',
			listeners: {
				afterrender: {
					fn: function () {
						Ext.each(this.items.items, function (item) {
							if (item.initialConfig.cls == 'x-text-search-item') {
								item.addListener('specialkey', function (field, e) {
									if (e.getKey() == e.ENTER) {
										this.fireEvent('filterChanged');
									}
								} .createDelegate(this));
							}
							if (item.initialConfig.cls == 'x-combo-search-item') {
								item.addListener('select', function (combo, record) {
									this.fireEvent('filterChanged');
								} .createDelegate(this));
							}
						}, this);
					},
					scope: this
				}
			}
		}, config);
		Tp.controls.Lookup.ToolbarGridFilter.superclass.constructor.call(this, config);
	},

	getFilterValues: function () {
		var result = { EntityTypeIds: this.entityTypeIds, ProjectIds: this.projectIds ? this.projectIds : [], ExcludeEntityId: this.excludeEntityId };
		Ext.each(this.items.items, function (item) {
			if (item.initialConfig.cls == 'x-text-search-item' || item.initialConfig.cls == 'x-combo-search-item') {
				result[item.name] = item.getValue();
			}
		})
		return result;
	}
});

Ext.reg('toolbargridfilter', Tp.controls.Lookup.ToolbarGridFilter);
