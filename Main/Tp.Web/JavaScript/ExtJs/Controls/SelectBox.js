Ext.ns('Tp.controls');

Tp.controls.SelectBox = Ext.extend(Ext.form.ComboBox, {

	constructor: function(config) {
		Tp.controls.SelectBox.superclass.constructor.call(this, config);
		if (this.insertEmptyOption)
			this.createEmptyOption();
	},

	createEmptyOption: function() {
		if (this.emptyText == null)
			return;
		if (this.emptyText.length == 0)
			return;
		var data = new Object();
		data[this.displayField] = this.emptyText;
		data[this.valueField] = null;
		var record = new Ext.data.Record(data, Tp.controls.SelectBox.EmptyID);
		this.store.insert(0, record);
	}
}
);

Tp.controls.SelectBox.EmptyID = 0;

Ext.reg('tpselectbox', Tp.controls.SelectBox);

 