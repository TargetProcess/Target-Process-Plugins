Ext.ns('Tp.data');

Tp.data.AssociationProxy = Ext.extend(Ext.data.DataProxy, {
    gridDataRequest: null,
    constructor: function(gridDataRequest) {
        Tp.util.validateForNulls([gridDataRequest]);
        Tp.data.AssociationProxy.superclass.constructor.apply(this);
        this.gridDataRequest = gridDataRequest;
    },

    load: function(params, reader, callback, scope, arg) {
        var item = Tp.controls.grid.CurrentEditableItem.getInstance();
        Tp.util.validateForNulls([item]);
        var callbackDelegate = callback.createDelegate(scope, [arg], true);
        Tp.Web.Extensions.ExtJs.Web.GridService.GetReplacementRows(this.gridDataRequest, item.columnName, item.record.data, this.onSuccessLoadResponse.createDelegate(this), this.onErrorLoadRequest.createDelegate(this), callbackDelegate);
    },

    onSuccessLoadResponse: function(recordSet, callback) {
        var records = Tp.data.RecordHelper.getInstance().createRecords(recordSet);
        callback(records);
    },

    onErrorLoadRequest: function() {
        throw "onErrorLoadRequest";
    }
});

