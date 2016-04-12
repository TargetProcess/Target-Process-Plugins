Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.AssignableTypeJsonStore = Ext.extend(Ext.data.JsonStore, {
	constructor: function (config) {
		config = Ext.apply({
			fields: ['entityTypeId', 'entityTypeName', 'entityIcon']
		}, config);
		Tp.controls.Prioritization.AssignableTypeJsonStore.superclass.constructor.call(this, config);
	}
});

Ext.reg('tpassignabletypejsonstore', Tp.controls.Prioritization.AssignableTypeJsonStore);