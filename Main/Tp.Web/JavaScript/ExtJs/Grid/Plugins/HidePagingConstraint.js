 

Ext.ns('Tp.controls.grid.plugins');
Tp.controls.grid.plugins.HidePagingConstraint = Ext.extend(Object, {
    grid: null,
    init: function(grid) {
        this.grid = grid;
        this.grid.on('initializeEvents', this.onInitializeEvents, this);
        this.grid.store.on('load', this.onStoreLoad, this);
    },

    onInitializeEvents: function() {
        this.grid.store.on('load', this.onGridChange, this);
    },

    onStoreLoad: function() {
        this.fixBottomToolbar();
    },

    fixBottomToolbar: function() {
        var toolbar = this.grid.getBottomToolbar();
        Tp.util.validateForNulls([toolbar]);
        if (this.grid.store.totalLength <= toolbar.pageSize)
            toolbar.hide();
        else
            toolbar.show();
    }
});