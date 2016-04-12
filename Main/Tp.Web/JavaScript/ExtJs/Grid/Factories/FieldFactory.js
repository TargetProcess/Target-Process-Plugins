Ext.ns('Tp.controls.grid');

Tp.controls.grid.FieldFactory = Ext.extend(Object, {
	gridDataRequest: null,

	constructor: function (gridDataRequest) {
		Tp.util.validateForNulls([gridDataRequest]);
		this.gridDataRequest = gridDataRequest;
	},

	createEditor: function (columnConfig) {
		Tp.util.validateForNulls([columnConfig]);
		if (columnConfig.IsUpdateable) {

			switch (columnConfig.Type) {
				case "string":
					if (columnConfig.Facet.dictionary) {
						return this.makeComboBox(columnConfig.Facet.dictionary);
					}
					if (columnConfig.Facet.collection) {
						return new Tp.controls.grid.Editors.ComboBox({ columnConfig: columnConfig, gridDataRequest: this.gridDataRequest });
					}
					if (columnConfig.Facet.format == "richText") {
						return new Tp.controls.RichTextField({});
					}
					if (columnConfig.Facet.format == "url") {
						return new Tp.controls.UrlField({});
					}
					return new Tp.controls.TextField({});
				case "int":
					if (columnConfig.Facet.dictionary) {
						return this.makeComboBox(columnConfig.Facet.dictionary);
					}
					return new Ext.form.NumberField({ allowNegative: false });
				case "float":
					return new Ext.form.NumberField({ allowNegative: false });
				case "bool":
					return new Tp.controls.BooleanField({});
				case "date":
					return new Ext.form.DateField({ format: 'd-M-Y' });
			}
		}
	},

	createRenderer: function (columnConfig) {
		if (columnConfig.Facet.dictionary) {
			return enumRenderer(columnConfig.Facet.dictionary);
		}
		if (columnConfig.Type == "string") {

			if (columnConfig.Facet.format == "richText") {
				return richTextRenderer();
			}
			if (columnConfig.Facet.format == "url") {
				return urlRenderer();
			}
			if (columnConfig.Facet.format == "entity") {
				return entityRenderer();
			}
			return stringRenderer();
		}

		if (columnConfig.Type == "int") {
			if (columnConfig.Facet.entityKind) {
				return viewLinkRenderer(columnConfig.Facet.entityKind);
			}
			return null;
		}

		if (columnConfig.Type == "bool") {
			return boolRenderer();
		}

		if (columnConfig.Type == "date") {
			return Ext.util.Format.dateRenderer('d-M-Y');
		}

		if (columnConfig.Type == "float") {
			return floatRenderer();
		}
		return null;
	},

	makeComboBox: function (entries) {
		return new Ext.form.ComboBox({
			allQuery: null,
			triggerAction: "all",
			store: new Ext.data.ArrayStore({
				id: 0,
				fields: [
                    'id',
                    'text'
                ],
				data: this.convertDictToArray(entries)
			}),
			mode: "local",
			editable: false,
			valueField: 'id',
			displayField: 'text'
		});
	},
	convertDictToArray: function (dict) {
		var array = new Array();
		Ext.each(dict, function (item) {
			array.push([item.Key, item.Value])
		}, this);
		return array;
	}
});