Ext.ns('Tp.controls.TestCases');

Tp.controls.TestCaseEditorWindow = Ext.extend(Ext.Window, {
	panel: null,
	customFieldsEditors: [],

	constructor: function (config) {
		this.testCaseId = config.testCaseId;
		this.processId = config.processId;
		Ext.apply(config, this._createConfig());
		Tp.controls.TestCaseEditorWindow.superclass.constructor.call(this, config);
		this.addEvents("saved");
	},

	_createConfig: function () {
		this.panel = new Tp.controls.TestCaseEditorFormPanel({});
		var items = [this.panel];
		var c = this.getProcessConfiguration(this.processId);

		if (c && c.CustomFields && c.CustomFields.length > 0) {
			this.form = new Ext.FormPanel({
				labelWidth: 120,
				frame: false,
				bodyStyle: 'padding: 10px 10px 10px 10px; background-color: #D6D6D6;',
				border: true,
				defaultType: 'textfield',
				items: [],
				isValid: function () {
					var isValid = true;
					this.items.each(function (item) {
						isValid = item.isValid() && isValid;
					});
					return isValid;
				}
			});
			this.addCustomFields(c.CustomFields, this.form);
			items.push(this.form);
		}
		return {
			closeAction: 'hide',
			plain: true,
			buttonAlign: 'center',
			resizable: false,
			items: items,
			width: 725,    //IE7 fix: it won't be resized properly in IE7
			modal: true,
			buttons: [
                {
                	text: 'Save',
                	name: 'saveButton',
                	scope: this,
                	handler: this.onSaveClick
                },
                {
                	text: 'Cancel',
                	name: 'cancelButton',
                	scope: this,
                	handler: this.onCancelClick
                }
            ]
		};
	},

	show: function () {
		this.title = 'Edit Test Case';
		Tp.controls.TestCaseEditorWindow.superclass.show.apply(this, arguments);
		Ext.Ajax.request({
			url: new Tp.WebServiceURL('/PageServices/TestCaseListService.asmx/GetChanges').toString(),
			headers: { 'Content-Type': 'application/json' },
			success: Function.createDelegate(this, this.onCompleteLoadingDetails),
			jsonData: { 'testCaseID': this.testCaseId }
		});
	},

	addCustomFields: function (customFields, form) {
		this.removeCustomFieldEditors();
		var fieldEditorFactory = new TS.CustomFieldEditorFactory();
		for (var i = 0; i < customFields.length; i++) {
			var customFieldDto = customFields[i];
			var fieldEditor = fieldEditorFactory.createEditor(customFieldDto);
			if (!fieldEditor)
				continue;

			form.add(fieldEditor);
			this.customFieldsEditors.push(fieldEditor);
		}
	},

	removeCustomFieldEditors: function () {
		for (var i = 0; i < this.customFieldsEditors.length; i++) {
			this.form.remove(this.customFieldsEditors[i]);
			this.customFieldsEditors[i].destroy();
		}
		this.customFieldsEditors = [];
	},

	getProcessConfiguration: function (processId) {
		for (var i = 0; i < processConfigurations.length; i++) {
			if (processConfigurations[i].ID == processId) {
				return processConfigurations[i];
			}
		}
		return null;
	},

	isFormValid: function () {
		var changes = this.panel.getForm().getValues();
		if (changes.name == '' || changes.steps == '' || changes.success == '') {
			Ext.Msg.show(
			{
				title: 'Validation',
				msg: 'Please fill all fields in',
				buttons: Ext.Msg.OK,
				icon: Ext.MessageBox.ERROR
			});
			return false;
		}
		if (this.form && !this.form.isValid())
			return false;
		return true;
	},

	onSaveClick: function () {
		if (!this.isFormValid())
			return;
		var changes = this.panel.getForm().getValues();
		var cf = this.getCustomFieldValues();
		if (cf) {
			changes['customFields'] = Ext.encode(cf);
		}
		Ext.Ajax.request({
			url: new Tp.WebServiceURL('/PageServices/TestCaseListService.asmx/SetChanges').toString(),
			headers: { 'Content-Type': 'application/json' },
			success: Function.createDelegate(this, this.onSaved),
			jsonData: { 'changes': changes }
		});
	},

	onCancelClick: function () {
		this.hide();
		this.panel.getForm().reset();
	},

	onSaved: function () {
		this.fireEvent("saved");
		this.hide();
		this.panel.getForm().reset();
	},

	onCompleteLoadingDetails: function (changes) {
		var d = Ext.decode(changes.responseText).d;
		var c = Ext.decode(d.customFields);
		this.panel.getForm().setValues(d);
		this.fillCustomFields(this.customFieldsEditors, c);
	},

	fillCustomFields: function (editors, values) {
		if (!values) return;
		for (var i = 0; i < editors.length; i++) {
			var fieldEditor = editors[i];
			if (!fieldEditor)
				continue;

			var value = values[fieldEditor.DTO.Name];

			if (value)
				fieldEditor.setValue(value);
		}
	},

	getCustomFieldValues: function () {
		var customFields = {};
		for (var i = 0; i < this.customFieldsEditors.length; i++) {
			var customFieldEditor = this.customFieldsEditors[i];

			var value = customFieldEditor.getValue();

			if (value.dateFormat)
				value = value.format(extJsDateFormat);

			customFields[customFieldEditor.DTO.Name] = value;
		}
		return customFields;
	}
});
