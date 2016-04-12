Ext.ns('Tp.controls.TestCases');

Tp.controls.TestCaseCreateEditorWindow = Ext.extend(Tp.controls.TestCaseEditorWindow, {
	onSaveClick: function () {
		if (!this.isFormValid())
			return;
		var changes = this.panel.getForm().getValues();
		var cf = this.getCustomFieldValues();
		if (cf) {
			changes['customFields'] = Ext.encode(cf);
		}

		Ext.Ajax.request({
			url: new Tp.WebServiceURL('/PageServices/TestCaseListService.asmx/CreateTestCase').toString(),
			headers: { 'Content-Type': 'application/json' },
			success: Function.createDelegate(this, this.onSaved),
			jsonData: { 'testCaseJsonData': changes, 'userStoryId': this.initialConfig.userStoryId }
		});
	},

	show: function () {
		this.title = 'Add Test Case';
		Tp.controls.TestCaseEditorWindow.superclass.show.apply(this, arguments);
	}
});
