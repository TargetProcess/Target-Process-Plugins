Ext.ns('Tp.controls.grid');
Tp.controls.grid.CurrentEditableItem = Ext.extend(Object,
{
    columnName: null,
    record: null,
    associativeRecord: null,
    constructor: function(context) {
        this.validateArguments(context);
        Ext.apply(this, context);
    },
    validateArguments: function(arg) {
        if (arg == null)
            throw "arg";
        if (arg.columnName == null)
            throw "columnName";
        if (arg.record == null)
            throw "record";
    }
});

Tp.controls.grid.CurrentEditableItem.instance = null;
Tp.controls.grid.CurrentEditableItem.getInstance = function() {
    return Tp.controls.grid.CurrentEditableItem.instance;
};

Tp.controls.grid.CurrentEditableItem.setInstance = function(context) {
 Tp.controls.grid.CurrentEditableItem.instance = new Tp.controls.grid.CurrentEditableItem(context);
};

Tp.controls.grid.CurrentEditableItem.reset = function() {
     delete Tp.controls.grid.CurrentEditableItem.instance;
};
