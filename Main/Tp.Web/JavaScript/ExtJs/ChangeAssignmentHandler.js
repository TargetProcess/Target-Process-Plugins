Ext.ns('Tp.actions');

Tp.actions.ChangeAssignmentHandler = Ext.extend(Tp.actions.Handler, {
	handle: function (item, context, onSucessCallback) {
		Tp.actions.ChangeAssignmentHandler.superclass.handle.call(this, onSucessCallback);
		WebMethods.ChangeAssignment(item, context, this._onSaveCallback.createDelegate(this), this._onErrorCallback.createDelegate(this));
	}
});