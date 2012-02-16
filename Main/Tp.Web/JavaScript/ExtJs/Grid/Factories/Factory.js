Ext.ns('Tp.controls.grid');

Tp.controls.grid.Factory = new Object();

Tp.controls.grid.Factory.getInstance = function() {
    if (Tp.controls.grid.Factory.instance == null) {
        Tp.controls.grid.Factory.instance = eval("new " + factoryType);
    }
    return Tp.controls.grid.Factory.instance;
};

var factoryType = "Tp.controls.grid.ConfigurableFactory";

Tp.controls.grid.ConfigurableFactory = Ext.extend(Object, {
    getStoreFactory: function() {
        return eval("new " + storeFactory);
    },

    _getConfigurationFactoryEvaluated : function(configurationFactory, initialConfig) {
        return eval("new " + configurationFactory + "(initialConfig)");
    },

    getConfigurationFactory: function(initialConfig) {
        return this._getConfigurationFactoryEvaluated(configurationFactory, initialConfig);
    },

    getInnerListConfigurationFactory: function(initialConfig) {
        return this._getConfigurationFactoryEvaluated(innerListConfigurationFactory, initialConfig);
    },

    getFieldFactory: function(gridDataRequest) {
        return eval("new " + editorFieldFactory + "(gridDataRequest)");
    },

    createGridPanel: function(initialConfig) {
        var config = this.getConfigurationFactory(initialConfig).createConfig();

        return eval("new " + gridPanelType + "(config)");
    },

    createInnerGridPanel: function(initialConfig, context) {
        var config = this.getInnerListConfigurationFactory(initialConfig).createConfig(context);
        return eval("new " + innerGridPanelType + "(config)");
    },

    createProxy: function() {
        try {
            return eval("new " + gridProxy);
        }
        catch (ex) {

            throw "GridProxy is undefined";
        }
    }

});

var gridPanelType = "Tp.controls.grid.EditorGridPanel";

var innerGridPanelType = "Tp.controls.grid.EditorGridPanel";

var editorFieldFactory = "Tp.controls.grid.FieldFactory";

var storeFactory = "Tp.controls.grid.GridStoreFactory";

var configurationFactory = "Tp.controls.grid.ReportConfigurationFactory";

var innerListConfigurationFactory = "Tp.controls.grid.InnerListConfigurationFactory";

var gridProxy = "Ext.Tp.GridServiceProxy";