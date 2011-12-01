Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.MoveRankUpColumnPlugin = Ext.extend(Object, {
    header: "",
    width: 50,
    sortable: false,
    fixed:true,
    menuDisabled:true,
    dataIndex: '',
    id: 'moveRankUp',
    moveable:false,
    grid:null,
    mouseEntered: false,

    init:function(grid) {
        this.grid = grid;
        this.grid.on("mouseover", this.onMouseOver, this);
        this.grid.on("mouseout", this.onMouseOut, this);
        this.grid.on("cellclick", this.onCellClick, this);
        this._handleMouseHovering = this._handleMouseHoveringWhenMouseOut;
    },

    renderer : function(v, p, record) {
        return "<a class='x-row-move-up' style='visibility:hidden; text-decoration: none;' href='javascript:void(0)'><img style='vertical-align: middle;' class='x-row-move-up' src='" + Application.baseUrl + "/JavaScript/tau/css/images/top.gif' /></a><a class='x-row-move-up' style='visibility:hidden; vertical-align: middle;'  href='javascript:void(0)'>Top</a>";
    },

    onMouseOver: function(eventObj) {
        var row = eventObj.getTarget('.x-grid3-row');
        this._handleMouseHovering(row);
        this._handleMouseHovering = this._handleMouseHoveringWhenMouseOut;
    },

    onMouseOut: function(eventObj) {
        var row = eventObj.getTarget('.x-grid3-row');
        this._handleMouseHovering(row);
        this._handleMouseHovering = this._handleMouseHoveringWhenMouseEntered;
    },

    _handleMouseHoveringWhenMouseOut: function(row) {
        if (row == null) {
            return;
        }
        var anchors = Ext.fly(row).select('a[class=x-row-move-up]');
        anchors.each(function(anchor) {
            anchor.setVisible(false);
        });
    },

    _handleMouseHoveringWhenMouseEntered: function(row) {
        if (row == null) {
            return;
        }
        var anchors = Ext.fly(row).select('a[class=x-row-move-up]');
        anchors.each(function(anchor) {
            anchor.setVisible(true);
        });
    },

    onCellClick:function(sender, rowIndex, columnIndex, e) {
        if(!Ext.fly(e.target).hasClass('x-row-move-up'))
            return;
        this.grid.fireEvent("beforerowmove", {}, rowIndex, 0, {});
        this.replaceRecord(rowIndex, 0);
    },

    replaceRecord:function(index, targetIndex) {
        var store = this.grid.getStore();
        var record = store.getAt(index);
        store.removeAt(index);
        store.insert(targetIndex, record);
    }
});


