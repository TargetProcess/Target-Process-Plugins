Ext.ns('Tp.controls.grid.plugins');
Tp.controls.grid.plugins.GridDragDropRowOrder = Ext.extend(Ext.ux.dd.GridDragDropRowOrder, {

	constructor:function() {
		Tp.controls.grid.plugins.GridDragDropRowOrder.superclass.constructor.apply(this, arguments);
		this.on("afterrowmove", this._onAfterRowMove, this);
		this.on("beforerowmove", this._onBeforeRowMove, this);
	},

	_onBeforeRowMove:function(gridDropTarget, index, targetIndex, selections) {
		this.grid.fireEvent("beforerowmove", gridDropTarget, index, targetIndex, selections);
	},

	_onAfterRowMove:function(gridDropTarget, index, targetIndex, selections) {
		this.grid.fireEvent("roworderchanged", targetIndex);
	}
});

Ext.reg('tpgriddragdroproworder', Tp.controls.grid.plugins.GridDragDropRowOrder);