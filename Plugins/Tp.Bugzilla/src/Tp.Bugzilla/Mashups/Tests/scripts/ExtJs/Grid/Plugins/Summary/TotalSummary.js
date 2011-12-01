
Ext.ns('Tp.controls.grid.plugins');

Tp.controls.grid.plugins.TotalSummary = Ext.extend(Tp.controls.grid.plugins.Summary, {
    summary: null,

    bindEvents: function() {
        Tp.controls.grid.plugins.TotalSummary.superclass.bindEvents.call(this);
        this.view.doRenderEnd = this.doRenderEnd.createDelegate(this);
    },

   

    doRenderEnd: function(cs, rs, ds, startRow, colCount, stripe) {
        if (this.isGroupingOn()) {
            return;
        }

        if (rs.length < 2) {
            return;
        }

        if (this.summary == null) {
            return;

        }
        return this.renderSummary({ data: this.summary }, cs);
    },

    onStoreLoad: function(sender, records, options) {
        if (this.isGroupingOn()) {
            return;
        }
        
        this.summary = options.summary;
        this.view.refresh();
    }
});



