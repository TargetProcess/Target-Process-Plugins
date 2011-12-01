Ext.ns('Tp.controls.grid.custom');

Tp.controls.grid.custom.TestCasePassFailController = Ext.extend(Object, {
	changeState: function (grid, record, value) {
		grid.fireEvent('entityStateChanging', record.id);
		Ext.Ajax.request({
			url: new Tp.WebServiceURL('/PageServices/TestCaseListService.asmx/ChangeRunState').toString(),
			headers: { 'Content-Type': 'application/json' },
			success: this._onChangeStateComplete.createDelegate(this, [{ grid: grid, recordId: record.id}], true),
			failure: this._onChangeStateFailed.createDelegate(this, [{ grid: grid, recordId: record.id}], true),
			jsonData: { 'testCaseID': record.id, 'status': value }
		});
	},

	_onChangeStateComplete: function (response, context, params) {
		var d = Ext.decode(response.responseText).d;
		params.newLastRunDate = eval('new ' + d.LastRunDate.replace(/\//g, ''));
		params.newLastStatus = d.LastStatus;
		params.grid.fireEvent('entityStateChanged', params);
	},

	_onChangeStateFailed: function (fatalError) {
		DisplayError(fatalError.get_message());
	}
});