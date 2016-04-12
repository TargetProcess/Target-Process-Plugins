Ext.ns('Tp.controls.grid.plugins');
Tp.controls.grid.plugins.GridExpander = Ext.extend(Ext.grid.RowExpander, {
    innerGrids: null,
    movable: false,
    lazyRender: false,
    constructor: function(config) {
        if (config == null)
            return;
        this.innerGrids = [];
        this.config = config;
        var template = { tpl: new Ext.Template(this.generateTemplate()) }
        Tp.controls.grid.plugins.GridExpander.superclass.constructor.call(this, template);
    },

    init: function(grid) {
        Tp.controls.grid.plugins.GridExpander.superclass.init.call(this, grid);
        this.on("expand", this.onExpand, this);
        this.on("beforecollapse", this.onBeforeCollapse, this);
        this.grid.store.on("update", this.onStoreUpdate, this);
		this.grid.getView().on("groupChanging", this.onCollapseAllLists, this);
        this.grid.store.on("beforeload", this.onCollapseAllLists, this);
        this.grid.getView().on("rowupdated", this.onRowUpdated, this);
        this.grid.on("save", this.onSave, this);
        this.grid.on("reconfiguring", this.onCollapseAllLists, this);
        this.grid.colModel.on("columnmoved", this.onCollapseAllLists, this);
    },

    onExpand: function(sender, record, body, rowIndex) {

        if (this.isListsHidden(record.id)) {
            this.showInnerLists(record.id);
            return;
        }
        for (var i = 0; i < this.config.innerGridConfig.length; i++) {
            var elementId = this.config.renderTo + "_innerGrid_" + i + "_" + record.id;
            var context = { renderTo: elementId, Id: record.id };
            var grid = Tp.controls.grid.Factory.getInstance().createInnerGridPanel(this.config.innerGridConfig[i], context);
            this.innerGrids.push(grid);
        }
    },

    onBeforeCollapse: function(sender, record, body, rowIndex) {
        this.hideInnerLists(record.id);

    },

    onStoreUpdate: function(sender, record, operation) {
        this.hideInnerLists(record.id);
    },

    onRowUpdated: function(view, firstRow, record) {
        this.showInnerLists(record.id);
    },

    onCollapseAllLists: function() {
        if (this.grid.store.getCount() == 0)
            return;

        var recordCount = this.grid.store.getCount();
        for (var i = 0; i < recordCount; i++) {
            var record = this.grid.store.getAt(i);
            if (this.state[record.id] === true) {
                this.collapseRow(i);
            }
        }
    },

    onSave: function() {
        for (var i = 0; i < this.innerGrids.length; i++) {
            this.innerGrids[i].fireEvent("save");
        }
    },

    generateTemplate: function() {
        var innerDivs = "";
        for (var i = 0; i < this.config.innerGridConfig.length; i++)
            innerDivs += "<div id='" + this.config.renderTo + "_innerGrid_" + i + "_{id}'></div><br />";
        return "<div id='" + this.config.renderTo + "_{id}'>" + innerDivs + "</div>";
    },

    showInnerLists: function(id) {
        if (!this.state[id])
            return;
        var listsHolderId = this.config.renderTo + "_" + id;
        var listsHolder = Ext.fly(listsHolderId).dom;
        var listStorage = Ext.fly(listsHolderId + "_listStorage").dom;
        while (listsHolder.childNodes.length != 0) {
            listsHolder.removeChild(listsHolder.childNodes[0])
        }
        while (listStorage.childNodes.length != 0) {
            listsHolder.appendChild(listStorage.childNodes[0])
        }
        listStorage.parentNode.removeChild(listStorage);
    },

    hideInnerLists: function(id) {
        if (!this.state[id])
            return;
        var listsHolderId = this.config.renderTo + "_" + id;
        var listsHolder = Ext.fly(listsHolderId).dom;
        var listStorage = document.body.appendChild(document.createElement("div"));
        listStorage.id = listsHolder.id + "_listStorage";
        listStorage.style.display = "none";
        while (listsHolder.childNodes.length != 0) {
            listStorage.appendChild(listsHolder.childNodes[0])
        }
    },

    isListsHidden: function(id) {
        var listsStorageId = this.config.renderTo + "_" + id + "_listStorage";
        return Ext.fly(listsStorageId)!= null;
    }
});