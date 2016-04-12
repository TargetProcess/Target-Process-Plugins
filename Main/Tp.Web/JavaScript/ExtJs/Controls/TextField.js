Ext.ns('Tp.controls');
/*
* Trims value when loses focus.
*/
Tp.controls.TextField = Ext.extend(Ext.form.TextField, {
	constructor: function (config) {
		if (config.maxLength) {
			/**
			* Use autoCreate so the maxlength setting for form textfield and
			* numberfield components respected on onkeyup/onkeydown, rather
			* than just when the form is submitted.
			*/
			var autoCreate = Ext.apply({}, this.defaultAutoCreate || config.autoCreate);
			Ext.apply(autoCreate, { maxlength: config.maxLength });
			Ext.apply(config, { autoCreate: autoCreate });
		}
		Tp.controls.TextField.superclass.constructor.call(this, config);
	},

	beforeBlur: function () {
		var v = this.getRawValue();
		Tp.controls.TextField.superclass.setValue.call(this, v.trim());
	}
});

Ext.reg('tptextfield', Tp.controls.TextField);