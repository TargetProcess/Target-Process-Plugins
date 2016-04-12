Ext.ns('Tp.controls.grid');

Tp.controls.grid.GridStoreFactory = Ext.extend(Object, {
    createFields: function(gridHeader) {
        var fields = new Array();
        for (var i = 0; i < gridHeader.Columns.length; i++) {
            var column = gridHeader.Columns[i];
            fields.push({ name: column.Id, type: column.Type });
        }
        return fields;
    },

    createReader: function(gridHeader) {
        var fieldMap = new Ext.data.Record.create(this.createFields(gridHeader));
        return new Ext.data.TpReader({}, fieldMap);
    },
   
    createAssociationProxy: function(gridDataRequest) {
        Tp.util.validateForNulls([gridDataRequest]);
        return new Tp.data.AssociationProxy(gridDataRequest);
    },

    createAssociationStore: function(columnConfig, gridDataRequest) {
        Tp.util.validateForNulls([gridDataRequest, columnConfig]);
        return new Ext.data.SimpleStore({
            fields: [columnConfig.Id],
            proxy: this.createAssociationProxy(gridDataRequest)
        });
    },

    createStore: function(gridDataRequest, gridHeader) {
        var proxy = Tp.controls.grid.Factory.getInstance().createProxy();
        proxy.gridDataRequest = gridDataRequest;
        var reader = this.createReader(gridHeader);
        var sortInfo = { field: gridDataRequest.OrderBy, direction: gridDataRequest.OrderDir ? "ASC" : "DESC" };
        return new Ext.data.GroupingStore({
            autoLoad: true,
            proxy: proxy,
            reader: reader,
            sortInfo: sortInfo,
            remoteSort: true,
            remoteGroup: true,
            groupField: gridDataRequest.GroupBy
        });
    }
});

