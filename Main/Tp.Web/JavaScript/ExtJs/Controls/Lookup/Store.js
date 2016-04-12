Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.Store = Ext.extend(Ext.data.Store, {
	constructor: function (config) {
		var idProperty = 'GeneralID';
		var config = Ext.apply({
			root: 'Rows',
			idProperty: idProperty,
			totalProperty: 'Results',
			successProperty: 'Success',
			remoteSort: true,
			sortInfo: {
				field: idProperty,
				direction: 'ASC'
			}
		}, config);
		Tp.controls.Lookup.Store.superclass.constructor.call(this, Ext.apply(config, {
			reader: new Tp.controls.Lookup.WebServiceReader(config)
		}));
	}
});