Ext.ns('Tp.controls.grid.plugins');
Tp.controls.grid.plugins.BunchRowsExpander = Ext.extend(Tp.controls.grid.plugins.StatefulRowExpander , {

    header: '<div style="float:left;width:17px;" class="x-grid3-row-collapsed" ><div class="x-grid3-row-expander">&#160;</div></div><div style="float:left;width:17px" class="x-grid3-row-expanded" ><div class="x-grid3-row-expander">&#160;</div></div>',

    width: 40,

    getExpandButtonCell: function() {
        var view = this.grid.getView();
        return Ext.fly(view.getHeaderCell(0));
    },

    init: function() {
        Tp.controls.grid.plugins.BunchRowsExpander.superclass.init.apply(this, arguments);
        this.grid.on('render', this._onRebindEventHandlers, this);
        this.grid.on('columnmove', this._onRebindEventHandlers, this);
    },

    _onRebindEventHandlers:function() {
        this.getExpandButtonCell().on('mousedown', this.onToggleAllButtonPressed, this);
        //UI fix as there is no other way, I wish could do it via CSS. May be I will back to it to fix it via CSS
        this.getExpandButtonCell(0).child('div').dom.style.padding = '0'
    },

    onToggleAllButtonPressed: function(e, t) {
        var target = Ext.fly(t).parent().dom;
        var store = this.grid.getStore();

        if (target.className == 'x-grid3-row-collapsed')
        {
            this.applyAction(this._expandRowIfPossible.createDelegate(this));
        }
        else
            if (target.className == 'x-grid3-row-expanded') {
                this.applyAction(this._collapseRowIfPossible.createDelegate(this));
            }
    },

    applyAction: function(action) {
        var count = this.grid.getStore().getCount();
        for (var i = 0; i < count; i++) {
            action(i);
        }
    },

    _expandRowIfPossible : function(rowIndex) {
        var record = this.grid.store.getAt(rowIndex);
        if (this.state[record.id] == true)
            return;
        this.expandRow.call(this, rowIndex);

    },

    _collapseRowIfPossible : function(rowIndex) {
        var record = this.grid.store.getAt(rowIndex);
        if (this.state[record.id] == false)
            return;
        this.collapseRow.call(this, rowIndex);
    }

});