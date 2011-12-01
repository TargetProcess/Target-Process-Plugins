Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.JsonProxy = Ext.extend(Ext.data.HttpProxy, {
	constructor: function (config) {
		var config = Ext.apply({
			listeners: {
				exception: {
					fn: function (proxy, type, action, options, response, arg) {
						var json = response.responseText;
						var o = Ext.decode(json);
						Ext.Msg.show({
							title: o.ExceptionType || 'Exception',
							msg: o.Message || 'Please reload the page.',
							minWidth: 350,
							buttons: Ext.Msg.OK,
							icon: Ext.MessageBox.ERROR
						});
					}
				}
			}
		}, config);

		Tp.controls.Lookup.JsonProxy.superclass.constructor.call(this, config);
	},

	doRequest: function (action, rs, params, reader, cb, scope, arg) {
		var jsonParams = { jsonData: {} };

		for (var paramName in params || {}) {
			jsonParams.jsonData[paramName] = params[paramName];
		}
		Tp.controls.Lookup.JsonProxy.superclass.doRequest.call(this, action, rs, jsonParams, reader, cb, scope, arg);
	}
});
