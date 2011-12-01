Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.EntityTypesFilterControl = Ext.extend(Ext.form.ComboBox, {
	constructor: function (config) {
		var config = Ext.apply({
			triggerAction: 'all',
			autoSelect: false,
			lazyRender: true,
			editable: false,
			entityTypeIds: [],
			mode: 'local',
			value: [],
			store: new Ext.data.Store({
				fields: ['id', 'name'],
				proxy: new Tp.controls.Lookup.JsonProxy({
					method: 'POST',
					url: new Tp.WebServiceURL('/PageServices/LookupService.asmx/GetEntityTypeData').toString(),
					api: {
						load: { url: new Tp.WebServiceURL('/PageServices/LookupService.asmx/GetEntityTypeData').toString(), method: 'POST' }
					}
				}),
				reader: new Ext.data.JsonReader({
					fields: [{ name: 'id' }, { name: 'name'}],
					idProperty: 'id',
					root: 'd'
				}),
				listeners: {
					load: {
						fn: function (store, records, o) {
							var c = this;
							if (!c.data) {
								c.data = { d: [] };
								store.each(function (r) {
									c.data.d.push(r.data);
								}, this);
							}
						}
					},
					scope: this
				}
			}),
			valueField: 'id',
			displayField: 'name',
			listeners: {
				beforequery: {
					fn: function (e) {
						var c = e.combo;
						if (!c.data) {
							var context = { params: { entityTypeIds: this.entityTypeIds} };
							Tp.util.validateForNulls([context]);
							e.combo.store.load(context);
						}
					}
				}
			}
		}, config);
		Tp.controls.Lookup.EntityTypesFilterControl.superclass.constructor.call(this, config);
	},

	getValue: function () {
		return this.value == '' ? this.entityTypeIds : this.value;
	}
});

Ext.reg('entitytypesfiltercontrol', Tp.controls.Lookup.EntityTypesFilterControl);
