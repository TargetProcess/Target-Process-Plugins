Ext.ns('Tp.controls.kanbanboard.board');

Tp.controls.kanbanboard.board.UserStoryEntityPopup = Ext.extend(Ext.Window, {
    service: null,
    panel: null,
    grid: null,
    store: null,
    strategy: null,

    constructor: function (config) {
        Ext.apply(config, this._createConfig());
        Tp.controls.kanbanboard.board.UserStoryEntityPopup.superclass.constructor.call(this, config);
    },

    _createConfig: function () {
        this.store = new Ext.data.JsonStore({
            fields: ['id', 'name', 'entityStateId', 'entity_state']
        });

        var csPlugin = new Tp.controls.Prioritization.ChangeStateColumnPlugin();
        var getViewUrl = this.getViewUrl.createDelegate(this);
        this.grid = new Ext.grid.EditorGridPanel({
            autoHeight: true,
            clicksToEdit: 1,
            enableColumnMove: false,
            enableColumnResize: false,
            enableHdMenu: false,
            columns: [
                {
                    width: 50,
                    dataIndex: 'id',
                    header: 'ID',
                    renderer: function (value) { return String.format("<a href='{0}'>{1}</a>", getViewUrl(value), value); }
                },
                {
                    width: 330,
                    dataIndex: 'name',
                    header: 'Name',
                    renderer: function (value) { return String.format("<div style='white-space:normal'>{0}</div>", value); }
                },
                csPlugin
            ],
            sm: new Ext.grid.RowSelectionModel({ singleSelect: true }),
            store: this.store,
            plugins: csPlugin,
            autoHeight: true
        });

        this.panel = new Ext.Panel({ autoHeight: true, items: this.grid });
        return {
            closable: true,
            closeAction: 'hide',
            items: [
                this.panel
            ],
            width: 500,
            autoHeight: true,
            resizable: false,
            border: false
        };
    },

    getViewUrl: function (value) {
        return this.strategy.getViewUrl(value);
    },

    setStrategy: function (strategy) {
        this.strategy = strategy;
    },

    getStrategy: function () {
        return this.strategy;
    },

    loadData: function (data) {
        this.strategy.loadData(data,
                this.loadAssignables_onSuccess.createDelegate(this),
                this.loadAssignables_onFailure.createDelegate(this));
    },

    loadAssignables_onSuccess: function (bugs) {
        this.grid.store.loadData(bugs);
        this.show();
    },

    loadAssignables_onFailure: function (x) {
        Tp.util.Notifier.error('Error', 'Loading ' + this.getEntityName() + 's failed, please reload this page.\n<hr/>\n' + x.get_message());
        this.hide();
    }
});