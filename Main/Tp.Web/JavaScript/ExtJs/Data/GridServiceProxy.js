Ext.namespace('Ext.Tp');

Ext.Tp.GridServiceProxy = function (connection) {
	var proxyConnection = null;

	if (connection) {
		proxyConnection = connection;
	} else {
		proxyConnection = {
			webServiceProxy: Tp.Web.Extensions.ExtJs.Web.GridService.GridService,
			webServiceProxyMethod: Tp.Web.Extensions.ExtJs.Web.GridService.GetData
		};
	}

	Ext.Tp.GridServiceProxy.superclass.constructor.call(this);
	Ext.apply(this, proxyConnection);
	this.gridDataRequest = null;
};

Ext.extend(Ext.Tp.GridServiceProxy, Ext.data.DataProxy, {
	load: function (params, reader, callback, scope, arg) {
		var userContext = {
			params: params || {},
			request: {
				callback: callback,
				scope: scope,
				arg: arg
			},
			reader: reader,
			callback: this.loadResponse,
			scope: this
		};

		if (arg && arg.Cnt) {
			if (!this.gridDataRequest.Cnt)
				this.gridDataRequest.Cnt = {};

			Ext.apply(this.gridDataRequest.Cnt, arg.Cnt);
		}

		var proxyWrapper = this;

		var webServiceCallback = function (response, context, methodName) {
			proxyWrapper.loadResponse(response, userContext, methodName);
		};

		if (!params.groupBy && scope.groupField) {
			params.groupBy = scope.groupField;
		}

		if (params.groupBy) {
			this.gridDataRequest.GroupBy = params.groupBy;
		} else {
			this.gridDataRequest.GroupBy = null;
		}

		if (params.sort) {
			this.gridDataRequest.OrderBy = params.sort;
		} else {
			this.gridDataRequest.OrderBy = null;
		}

		if (params.dir) {
			this.gridDataRequest.OrderDir = params.dir == "ASC";
		}

		if (params.start) {
			this.gridDataRequest.Start = params.start;
		} else {
			this.gridDataRequest.Start = 0;
		}

		if (params.limit) {
			this.gridDataRequest.Limit = params.limit;
		}

		var serviceParams = [this.gridDataRequest];

		serviceParams.push(webServiceCallback);
		serviceParams.push(this.handleErrorResponse);

		this.webServiceProxyMethod.apply(this.webServiceProxy, serviceParams);
	},

	handleErrorResponse: function (response, userContext, methodName) {
		alert("Error while calling method: " + methodName + "\n" + response.get_message());
	},

	loadResponse: function (response, userContext, methodName) {
		var result = userContext.reader.readRecords(response);
		userContext.request.arg.summary = response.Summary;
		userContext.request.arg.nextPageFirstEntityId = response.NextPageFirstEntityId;
		userContext.request.arg.groupSummaries = Tp.data.RecordHelper.getInstance().normalizeGroupSummaryDates(response.GroupSummaries);
		userContext.request.callback.call(userContext.request.scope || window, result, userContext.request.arg, true);
	}
});