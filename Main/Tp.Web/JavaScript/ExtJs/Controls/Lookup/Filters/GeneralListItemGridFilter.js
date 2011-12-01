Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.GeneralListItemGridFilter = Ext.extend(Tp.controls.Lookup.ToolbarGridFilter, {
	constructor: function (config) {
		config.items = config.items || [];
		config.items.push({
			xtype: 'textfield',
			name: 'ID',
			emptyText: 'ID',
			cls: 'x-text-search-item',
			maskRe: /^[\d]$/,
			width: 50
		});
		if (config.showProjects) {
			config.items.push({
				xtype: 'projectsfiltercontrol',
				name: 'ProjectIds',
				emptyText: 'Project',
				cls: 'x-combo-search-item',
				projectIds: config.projectIds,
				projectId: config.projectId,
				width: 160,
				ref: 'projects'
			});
		}
		config.items.push({
			xtype: 'textfield',
			name: 'Name',
			emptyText: 'Name',
			cls: 'x-text-search-item',
			width: config.nameWidth || 150
		});
		if (config.showState) {
			config.items.push({
				xtype: 'textfield',
				name: 'State',
				emptyText: 'State',
				cls: 'x-text-search-item',
				width: 100
			});
		}
		if (config.showReleaseIteration) {
			config.items.push({
				xtype: 'releaseiterationfiltercontrol',
				name: 'ReleaseIterationId',
				emptyText: 'Release / Iteration',
				cls: 'x-combo-search-item' || [],
				entityTypeIds: config.entityTypeIds,
				projectId: config.projectId,
				disabled: true,
				width: 140,
				ref: 'ri'
			});
		}
		if (config.showEntityType) {
			config.items.push({
				xtype: 'entitytypesfiltercontrol',
				name: 'EntityTypeIds',
				emptyText: 'EntityType',
				cls: 'x-combo-search-item',
				entityTypeIds: config.entityTypeIds || [],
				width: 110,
				ref: 'et'
			});
		}
		if (config.appendItems && config.appendItems.length > 0) {
			Ext.each(config.appendItems, function (item) {
				config.items.push(item);
			});
		}
		var config = Ext.apply({}, config);

		Tp.controls.Lookup.GeneralListItemGridFilter.superclass.constructor.call(this, config);

		if (this.projects && this.ri) {
			this.projects.on('select', function (combo, record) {
				this.ri.showDefault(record.data.id.length == 1 ? record.data.id[0] : null);
			}, this);
		}
	}
});

Ext.reg('generallistitemgridfilter', Tp.controls.Lookup.GeneralListItemGridFilter);
