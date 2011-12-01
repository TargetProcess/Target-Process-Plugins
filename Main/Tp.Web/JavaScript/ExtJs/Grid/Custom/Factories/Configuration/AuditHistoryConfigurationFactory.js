Ext.ns('Tp.controls.grid');

Tp.controls.grid.AuditHistoryConfigurationFactory = Ext.extend(Tp.controls.grid.ConfigurationFactory,
{
    pageSize: 20,
    createConfig: function(sourceConfig) {
        var config = Tp.controls.grid.AuditHistoryConfigurationFactory.superclass.createConfig.call(this, sourceConfig);
        config.header = false;
        config.sm = null;
        config.disableSelection = true;
        var expander = new Tp.custom.plugins.AuditDetailsExpander();
        config.columns.unshift(expander);
        config.cm = this.createColumnModel(config.columns);
        config.plugins.push(expander);
        config.bbar = this.createPagingToolBar(config.store);
        return config;
    },

    getPageSize: function() {
        return this.pageSize;
    }
});