Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.ReleaseIterationFilterControl = Ext.extend(Ext.form.ComboBox, {
	constructor: function (config) {
		var config = Ext.apply({
			triggerAction: 'all',
			autoSelect: false,
			lazyRender: true,
			editable: false,
			isLoaded: false,
			projectId: null,
			entityTypeIds: [],
			jsonCache: [],
			mode: 'local',
			value: '',
			tpl: new Ext.XTemplate('<tpl for="."><div class="x-combo-list-item {kind}">{name}</div></tpl>'),
			store: new Ext.data.Store({
				fields: ['id', 'name', 'kind'],
				proxy: new Tp.controls.Lookup.JsonProxy({
					method: 'POST',
					url: new Tp.WebServiceURL('/PageServices/LookupService.asmx/GetReleaseIterationData').toString(),
					api: {
						load: { url: new Tp.WebServiceURL('/PageServices/LookupService.asmx/GetReleaseIterationData').toString(), method: 'POST' }
					}
				}),
				reader: new Ext.data.JsonReader({
					fields: [{ name: 'id' }, { name: 'name' }, { name: 'kind'}],
					idProperty: 'id',
					root: 'd'
				}),
				listeners: {
					load: {
						fn: function (store, records, o) {
							var c = this;
							if (c.data.d.length > 0) {
								c.isLoaded = true;
								return;
							}
							Tp.util.validateForNulls([c.jsonCache]);
							var id = c.data.id;
							store.each(function (r) {
								c.data.d.push(r.data);
							}, this);
							c.jsonCache.push(c.data);
							c.isLoaded = true;
						}
					},
					scope: this
				}
			}),
			valueField: 'id',
			displayField: 'name',
			listeners: {
				afterrender: {
					fn: function (combo) {
						if (combo.projectId) {
							combo.showDefault(combo.projectId);
						}
					}
				},
				beforequery: {
					fn: function (e) {
						var c = e.combo;
						if (!c.isLoaded) {
							Tp.util.validateForNulls([c.jsonCache]);
							var id = c.data.id;
							if (c.data.d.length > 0) {
								c.store.loadData(c.data);
							}
							else {
								var context = { params: { projectId: id, entityTypeIds: this.entityTypeIds} };
								Tp.util.validateForNulls([context]);
								e.combo.store.load(context);
							}
						}
					}
				}
			}
		}, config);

		Tp.controls.Lookup.ReleaseIterationFilterControl.superclass.constructor.call(this, config);
	},

	showDefault: function (projectId) {
		var c = this;
		c.value = '';
		c.reset();
		if (projectId) {
			Tp.util.validateForNulls([c.jsonCache]);
			var json = Array.findOne(c.jsonCache, function (i) { return i.id == projectId; }, this);
			if (!json || !c.data || c.data.id != json.id) {
				c.data = json != null ? json : { d: [], id: projectId };
				c.isLoaded = false;
			}
			if (c.disabled) {
				c.enable();
			}
		}
		else {
			if (!c.disabled) {
				c.disable();
			}
		}
	}
});

Ext.reg('releaseiterationfiltercontrol', Tp.controls.Lookup.ReleaseIterationFilterControl);