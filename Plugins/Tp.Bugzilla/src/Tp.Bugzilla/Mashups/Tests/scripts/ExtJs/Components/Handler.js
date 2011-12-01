Ext.ns('Tp.actions');

Tp.actions.Handler = Ext.extend(Object, {
	_onSucessCallback: null,

	handle: function(onSucessCallback) {
		this._onSucessCallback = onSucessCallback;
	},

	_onSaveCallback: function(context) {
		if (this._onSucessCallback != null) {
			this._onSucessCallback(context);
		}
	},

	_onErrorCallback: function(error) {
		SetLastWarning(error.get_message());
	}
});

