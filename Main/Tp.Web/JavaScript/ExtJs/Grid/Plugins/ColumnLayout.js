 
 

Ext.ns('Tp.controls.grid.plugins');
Tp.controls.grid.plugins.ColumnLayout = Ext.extend(Object, {
    grid: null,

    _onColumnLayoutChangeErrorDelegate: null,

    init: function(grid) {
        this.grid = grid;
        this.initializeEvents();
        this._onErrorColumnLayoutChangeDelegate = Function.createDelegate(this, this.onErrorColumnLayoutChange);
        this.grid.on("initializeEvents", this.initializeEvents, this);
    },

    initializeEvents: function() {
        this.grid.colModel.on("columnmoved", this.onColumnMoved, this);
        this.grid.on("columnresize", this.onColumnResize, this);
    },

    onErrorColumnLayoutChange: function(error) {
        SetLastWarning(error.get_message());
    },

    onColumnResize: function(index, size) {
        this.grid.stopEditing();
        if (size == 0) {
            return;
        }
        var columnId = this.grid.colModel.getColumnId(index);
        Tp.Web.Extensions.ExtJs.Web.GridService.ChangeFieldWidth(this.grid.gridDataRequest, columnId, size, null, this._onErrorColumnLayoutChangeDelegate);
    },

    onColumnMoved: function(columnModel, oldIndex, newIndex) {
        var columnId = this.grid.colModel.getColumnId(newIndex);
        var afterColumnId = null;
        if (newIndex > 0) {
            afterColumnId = this.grid.colModel.getColumnId(newIndex - 1);
            // Expander is the first column in the grid when inner list presents.
            // It is very unlikely there will be a report column named 'expander'.
            if (afterColumnId == 'expander') {
                afterColumnId = null;
            }
        }
        Tp.Web.Extensions.ExtJs.Web.GridService.ChangeFieldPosition(this.grid.gridDataRequest, columnId, afterColumnId, null, this._onErrorColumnLayoutChangeDelegate);
    }
});