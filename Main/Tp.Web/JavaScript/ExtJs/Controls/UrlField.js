/**
* Specialized text field class for editing URL custom fields in custom report grid.
*/
Tp.controls.UrlField = Ext.extend(Ext.form.TextField, {
	label: null,

	constructor: function (config) {
		Tp.util.validateForNulls([config]);
		Tp.controls.UrlField.superclass.constructor.call(this, config);
		this.validator = this.validateUrl;
	},

	validateUrl: function (value) {
		if (this.allowBlank && (!value || value.length == 0))
			return true;
		var v = new RegExp();
		v.compile("^[A-Za-z]+://[A-Za-z0-9-_.]+(/)?(/[A-Za-z0-9-_%&\?\/.=#]+)?$");
		return v.test(value);
	},

	getValue: function () {
		var v = Tp.controls.UrlField.superclass.getValue.call(this);
		if (v && v != "http://") {
			if (this.label) {
				return this.label + "\n" + v.trim();
			} else {
				return v.trim();
			}
		} else {
			return null;
		}
	},

	setValue: function (v) {
		var label = null;
		var url = null;
		if (v) {
			var s = v.split("\n");
			if (s.length == 2) {
				label = s[0];
				url = s[1];
			} else {
				url = v;
			}
		} else {
			url = "http://";
		}
		if (label == url) {
			label = null;
		}
		this.label = label;
		Tp.controls.UrlField.superclass.setValue.call(this, url);
	},

	beforeBlur: function () {
		var v = this.getRawValue();
		if (v == "http://") {
			this.label = null;
			Tp.controls.UrlField.superclass.setValue.call(this, null);
		}
	}
});

Ext.reg('tpurlfield', Tp.controls.UrlField);