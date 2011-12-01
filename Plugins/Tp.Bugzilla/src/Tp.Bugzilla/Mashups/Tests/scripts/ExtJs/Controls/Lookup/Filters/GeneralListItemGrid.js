Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.GeneralListItemGrid = Ext.extend(Tp.controls.Lookup.GridPanel, {
	constructor: function (config) {
		var config = Ext.apply({
			gridConfig: {
				region: 'center',
				width: 600,
				columns: [
					{ header: 'Type', align: 'center', sortable: true, dataIndex: 'EntityTypeAbbreviation', renderer: function (v, o, r) { return String.format("<img class='x-item-icon' src='{0}/img/{1}' />", Application.baseUrl, r.data.Icon); } },
					{ id: 'id', header: 'ID', width: 70, sortable: true, dataIndex: 'GeneralID' },
					{ header: 'Name', width: 320, sortable: true, dataIndex: 'Name', renderer: function (v) { return String.format("<div style='white-space:normal'><a class='lookupActionLink' href='javascript:void(0)'>{0}</a></div>", v); } },
					{ header: 'Project', width: 150, sortable: true, dataIndex: 'ProjectName' }
				],
				tbar: {
					xtype: 'generallistitemgridfilter',
					showReleaseIteration: config.showReleaseIteration,
					excludeEntityId: config.excludeEntityId,
					showEntityType: config.showEntityType,
					entityTypeIds: config.entityTypeIds,
					showProjects: config.showProjects,
					projectIds: config.projectIds,
					projectId: config.projectId,
					showState: config.showState,
					appendItems: config.appendItems
				}
			}
		}, config);
		Tp.controls.Lookup.GeneralListItemGrid.superclass.constructor.call(this, config);
	}
});

Ext.reg('lookupgenerallistitemgrid', Tp.controls.Lookup.GeneralListItemGrid);
