Ext.ns('Tp.controls.grid');
Tp.controls.grid.InnerListConfigurationFactory = Ext.extend(Tp.controls.grid.ConfigurationFactory, {
    pageSize: 20,
    createConfig: function(context) {
        this.initialConfig.renderTo = context.renderTo;
        this.initialConfig.gridDataRequest.Cnt.ID = context.Id;
        this.initialConfig.gridDataRequest.GroupBy = "";
        var config = Tp.controls.grid.InnerListConfigurationFactory.superclass.createConfig.call(this);
        config.view = this.createView();
        config.bbar = this.createPagingToolBar(config.store);
        config.plugins.push(new Tp.controls.grid.plugins.HidePagingConstraint());
        return config;
    },

    createView: function(columnModel) {
        return new Tp.controls.grid.SummaryGroupingView({
            enableGrouping: false,
            cm: columnModel,
            forceFit: true
        });
    },

    getPageSize: function() {
        return this.pageSize;
    }
});