Tp.controls.grid.GeneralConfigurationFactory = Ext.extend(Tp.controls.grid.ConfigurationFactory, {
    createView: function(columnModel) {
    return new Ext.grid.GroupingView({
            cm: columnModel,
            forceFit: true,
            hideGroupedColumn: false,
            groupTextTpl: '{text} ({[values.rs.length]} {[values.rs.length > 1 ? "Items" : "Item"]})'
        });
    },

    createConfig: function(sourceConfig) {
        var config = Tp.controls.grid.GeneralConfigurationFactory.superclass.createConfig.call(this, sourceConfig);
        config.enableHdMenu = true;
        config.showSaveButton = true;
        config.view = this.createView(config.cm);
        config.bbar = this.createPagingToolBar(config.store);
        return config;
    }
});