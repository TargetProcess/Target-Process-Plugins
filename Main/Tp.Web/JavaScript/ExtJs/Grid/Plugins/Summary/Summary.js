Ext.ns('Tp.controls.grid.plugins');


Tp.controls.grid.plugins.Summary = Ext.extend(Object, {
    init: function(grid) {
        this.grid = grid;
        this.cm = grid.getColumnModel();
        this.view = grid.getView();
        this.bindEvents();

        if (!this.rowTpl) {
            this.rowTpl = new Ext.Template(
                '<div class="x-grid3-summary-row" style="{tstyle}">',
                '<table class="x-grid3-summary-table" border="0" cellspacing="0" cellpadding="0" style="{tstyle}">',
                    '<tbody><tr>{cells}</tr></tbody>',
                '</table></div>'
            );
            this.rowTpl.disableFormats = true;
        }
        this.rowTpl.compile();

        if (!this.cellTpl) {
            this.cellTpl = new Ext.Template(
                '<td class="x-grid3-col x-grid3-cell x-grid3-td-{id} {css}" style="{style}">',
                '<div class="x-grid3-cell-inner x-grid3-summary-cell x-grid3-col-{id}" unselectable="on">{value}</div>',
                "</td>"
            );
            this.cellTpl.disableFormats = true;
        }
        this.cellTpl.compile();
    },

    bindEvents: function() {
        this.grid.store.on("load", this.onStoreLoad, this);
        this.grid.on("initializeEvents", this.bindEvents, this);
    },

    renderSummary: function(o, cs) {
        cs = cs || this.view.getColumnData();
        var cfg = this.cm.config;

        var buf = [], c, p = {}, cf, last = cs.length - 1;
        for (var i = 0, len = cs.length; i < len; i++) {
            c = cs[i];
            cf = cfg[i];
            p.id = c.id;
            p.style = c.style;
            p.css = i == 0 ? 'x-grid3-cell-first ' : (i == last ? 'x-grid3-cell-last ' : '');
            p.value = o.data[c.name];
            if (p.value == undefined || p.value === "") p.value = "&#160;";
            buf[buf.length] = this.cellTpl.apply(p);
        }

        return this.rowTpl.apply({
            tstyle: 'width:' + this.view.getTotalWidth() + ';',
            cells: buf.join('')
        });
    },

    isGroupingOn: function() {
        //TO-DO: to find the robust way to detected if the grouping is on
        var groups = this.view.getGroups();
        if (groups == null)
            return false;
        if (groups.length == 0)
            return false;
        return groups[0].childNodes.length > 1;
    }

});


