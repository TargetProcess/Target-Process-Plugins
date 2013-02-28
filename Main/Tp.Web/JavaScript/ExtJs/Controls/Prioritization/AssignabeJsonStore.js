Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.AssignableJsonStore = Ext.extend(Ext.data.JsonStore, {
	constructor: function(config) {
		config = Ext.apply({
			fields:  [
				{
					name: 'id'
				},
				{
					name: 'entityIcon'
				},
				{
					name: 'entityName'
				},
				{
					name: 'effort'
				},
				{
					name: 'priority'
				},
				{
					name: 'entityState'
				},
                {
					name: 'projectId'
                },
                {
                	name: 'relations'
                },
                {
					name: 'tags'
				}
			],
			root: 'assignables'

		}, config);

		this.lastOptions = {nextPageFirstEntityId: 0};

		Tp.controls.Prioritization.AssignableJsonStore.superclass.constructor.call(this, config);
	}
});

Ext.reg('tpassignablejsonstore', Tp.controls.Prioritization.AssignableJsonStore);
