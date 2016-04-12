Ext.ns('Tp.controls.grid');

Tp.controls.grid.Handlers = new Object();

Tp.controls.grid.Handlers.getPrintReportHandler = function(componentId) {
	return function() {
		redirectToHandler(componentId, 'print.ashx');
	};
};

Tp.controls.grid.Handlers.getReportExportToExcelHandler = function(componentId) {
	return function() {
		redirectToHandler(componentId, 'csv.ashx');
	};
};

function redirectToHandler(componentId, handler) {
	var reportGrid = Ext.ComponentMgr.get(componentId);
	var url = document.location.href.toLowerCase().replace('viewcustomreport.aspx', handler);
	if (reportGrid && reportGrid.gridDataRequest.GridID != temporaryGridId) {
		var customReportIDParam = "CustomReportID=" + reportGrid.gridDataRequest.GridID;
		if (url.contains("customreportid")) {
			url = url.substring(0, url.indexOf("customreportid")) + customReportIDParam;
		}
		else if (url.contains(handler + '?')) {
			url += "&" + customReportIDParam;
		}
		else {
			url += "?" + customReportIDParam;
		}
	}
	document.location.href = url;
}

Tp.controls.grid.Handlers.getReportEditHandler = function(componentId) {
	return function() {
		var reportGrid = Ext.ComponentMgr.get(componentId);
		var reportGridEditor = new Tp.Ajax.ReportGridEditor(reportGrid);
		reportGridEditor.show();
	};
};

Tp.controls.grid.Handlers.refreshGrid = function(gridId) {
	return function() {
		var grid = Ext.ComponentMgr.get(gridId);
		grid.getStore().reload();   
	}
};

Tp.controls.grid.Handlers.getReorderHandler = function(componentId, prioritizationService) {
	return function(gridDropTarget, index, targetIndex, selections) {
		if (gridDropTarget.target) {
			gridDropTarget.target.lock();
			if (targetIndex > index && gridDropTarget.target.rowPosition < 0) {
				targetIndex--;
			}
			if (targetIndex < index && gridDropTarget.target.rowPosition > 0) {
				targetIndex++;
			}
		}

		var grid = Ext.ComponentMgr.get(componentId);
		var store = grid.getStore();

		var targetId;

		if (targetIndex == store.data.items.length - 1) {
			targetId = 0;
		}
		else {
			if (targetIndex > index) {
				targetIndex = targetIndex + 1;
			}
			targetId = store.getAt(targetIndex).id;
		}

		var id = store.getAt(index).id;
		var nextPageFirstItemId = store.lastOptions.nextPageFirstEntityId == null ? 0 : store.lastOptions.nextPageFirstEntityId;

		var onChangePriorityCompleted = function(retValue, callbackParams) {
			callbackParams.newPriorityName = retValue;
			callbackParams.grid.fireEvent('onPriorityChanged', callbackParams);
		};
		var onChangePriorityFailed = function(error, callbackParams) {
			if (callbackParams.target) {
				callbackParams.target.unlock();
			}
			Tp.util.Notifier.error('Error', 'Changing priority failed, please reload this page.\n<hr/>\n' + error.get_message());
		};

		prioritizationService.PrioritizeBefore(
			id,
			targetId,
			nextPageFirstItemId,
			onChangePriorityCompleted,
			onChangePriorityFailed,
			{ grid: grid, recordId: id, target: gridDropTarget.target });
	}
}
