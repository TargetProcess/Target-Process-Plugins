Ext.ns('Tp.controls.grid.Editors');

Tp.controls.grid.Editors.ComboBox = Ext.extend(Ext.form.ComboBox, {

	primaryKeyPrefix: "__key",
	dataLoaded: false,

	constructor: function(config) {
		Tp.util.validateForNulls([config.columnConfig, config.gridDataRequest]);
		config.store = Tp.controls.grid.Factory.getInstance().getStoreFactory().createAssociationStore(config.columnConfig, config.gridDataRequest);
		config.displayField = config.columnConfig.Id;
		config.typeAhead = true;
		config.triggerAction = 'all';
		config.mode = "local";
		config.minChars = 1;
		config.enableKeyEvents = true;
		Tp.controls.grid.Editors.ComboBox.superclass.constructor.call(this, config);
		this.store.on("load", this.onAfterLoad, this);
		this.on("beforequery", this.onBeforeQuery, this);
		this.on("show", this.onShow, this);
		this.on("keydown", this.onKeyPressHandler, this);
	},

	// Do not use names as "onBlur", "onHide" etc. in your implementation as base class could have the same names.
	// Otherwise such names would lead to losing the functionality of the base class.
	onKeyPressHandler: function() {
		if (this.columnConfig.IsRequired)
			return;
		if (!this.getRawValue().isBlank())
			return;
		var item = Tp.controls.grid.CurrentEditableItem.getInstance();
		Tp.util.validateForNulls([item]);
		item.associativeRecord = this.createNullRecord();
	},

	createNullRecord: function() {
		var headers = this.getHeaders();
		var data = new Object();
		for (var i = 0; i < headers.length; i++) {
			data[headers[i]] = null;
		}
		return new Ext.data.Record(data, null);
	},

	createEmptyOption: function() {
		var data = new Object();
		data[this.displayField] = Tp.controls.grid.components.SpecialCaseTable.getInstance().getEmptyValue(this.columnConfig.Id);
		var record = new Ext.data.Record(data, Tp.controls.grid.Editors.ComboBox.EmptyID);
		this.store.insert(0, record);
	},

	onShow: function() {
		this.store.removeAll();
		this.dataLoaded = false;
	},

	onBeforeQuery: function() {
		if (this.dataLoaded === true) {
			return;
		}
		this.store.load();
	},

	getHeaders: function() {
		if (this.getItemCount() == 0) {
			throw "InvalidOperation";
		}
		var index = this.columnConfig.IsRequired ? 0 : 1;
		return this.store.data.items[index].fields.keys;
	},

	getItemCount: function() {
		if (this.columnConfig.IsRequired)
			return this.store.getTotalCount();
		return this.store.getTotalCount() - 1;
	},

	onSelect: function(record, index) {
		Tp.controls.grid.Editors.ComboBox.superclass.onSelect.call(this, record, index);
		if (this.getItemCount() == 0) {
			this.setValue("");
			//exit as no item can be selected
			return;
		}
		var item = Tp.controls.grid.CurrentEditableItem.getInstance();
		Tp.util.validateForNulls([item]);
		//check whether item is empty stub if so set null record then exit
		if (record.id == Tp.controls.grid.Editors.ComboBox.EmptyID) {
			this.setValue("");
			item.associativeRecord = this.createNullRecord();
			return;
		}
		//set selected item
		item.associativeRecord = record;
	},

	validateDataValues: function(value, items) {
		if (this.dataLoaded === false)
			return true;
		for (var i = 0; i < items.length; i++) {
			if (items[i].data[this.displayField].indexOf(value) > -1)
				return true;
		}
		return false;
	},

	findPrimaryKey: function() {
		if (this.store.getCount() == 0)
			return;
		var keys = this.store.data.items[0].fields.keys;
		for (var i = 0; i < keys.length; i++) {
			if (keys[i].indexOf(this.primaryKeyPrefix) == 0)
				return keys[0];
		}
	},

	replaceTemplate: function() {
		var primaryKey = this.findPrimaryKey();
		this.view.tpl = new Ext.XTemplate('<tpl for="."><div class="x-combo-list-item"> {[values.' + primaryKey + ' == null ? "" : "<b>#"]}{' + primaryKey + '}{[values.' + primaryKey + ' == null ? "" : ".</b>"]} {' + this.displayField + '}</div></tpl>');
		this.view.refresh();
	},

	onAfterLoad: function() {
		this.dataLoaded = true;
		if (!this.columnConfig.Facet.simpleEnum)
			this.replaceTemplate();
		if (!this.columnConfig.IsRequired)
			this.createEmptyOption();
		this.validator = this.validateDataValues.createDelegate(this, [this.store.data.items], 1);
	}
});

Tp.controls.grid.Editors.ComboBox.EmptyID = -1;
