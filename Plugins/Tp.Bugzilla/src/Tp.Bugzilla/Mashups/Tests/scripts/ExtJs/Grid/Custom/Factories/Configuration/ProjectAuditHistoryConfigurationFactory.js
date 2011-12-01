Ext.ns('Tp.controls.grid');

Tp.controls.grid.ProjectAuditHistoryConfigurationFactory = Ext.extend(Tp.controls.grid.AuditHistoryConfigurationFactory,
{
    createConfig: function(sourceConfig) {
        var config = Tp.controls.grid.ProjectAuditHistoryConfigurationFactory.superclass.createConfig.call(this, sourceConfig);

        config.title = "Audit History"
        config.header = false;

        this.entitySelect = new Tp.controls.SelectBox({
            store: auditTypes,
            displayField: "name",
            valueField: "entityTypeId",
            typeAhead: true,
            width: 120,
            mode: "local",
            triggerAction: "all",
            forceSelection: true,
            selectOnFocus: true,
            allowBlank: false,
            editable: false,
            insertEmptyOption: false
        });
        
        if (config.gridDataRequest.Cnt && config.gridDataRequest.Cnt.EntityTypeID)
            this.entitySelect.setValue(config.gridDataRequest.Cnt.EntityTypeID);

        this.modifierSelect = new Tp.controls.SelectBox({
            store: usersInProject,
            displayField: "name",
            valueField: "userId",
            typeAhead: true,
            width: 150,
            mode: "local",
            triggerAction: "all",
            emptyText: "- Select Modifier -",
            forceSelection: true,
            selectOnFocus: true,
            allowBlank: true,
            editable: false,
            insertEmptyOption: true
        });

        if (config.gridDataRequest.Cnt && config.gridDataRequest.Cnt.UserID)
            this.modifierSelect.setValue(config.gridDataRequest.Cnt.UserID);

        this.fromDate = new Ext.tp.form.TpDateField({ allowBlank: true, format: extJsDateFormat });

        if (config.gridDataRequest.Cnt && config.gridDataRequest.Cnt.From)
            this.fromDate.setValue(config.gridDataRequest.Cnt.From);

        this.toDate = new Ext.tp.form.TpDateField({ allowBlank: true, format: extJsDateFormat });

        if (config.gridDataRequest.Cnt && config.gridDataRequest.Cnt.To)
            this.toDate.setValue(config.gridDataRequest.Cnt.To);

        var filterHandler = Function.createDelegate(this, this.onFilter);
        var clearHandler = Function.createDelegate(this, this.onClear);

        config.tbar = [
            "Entity&nbsp;",
            this.entitySelect,
            "&nbsp;&nbsp;",
            "Modifier&nbsp;",
            this.modifierSelect,
	    "&nbsp;",
            "From&nbsp;",
            this.fromDate,
            "&nbsp;&nbsp;",
            "To&nbsp;",
            this.toDate,
	    "&nbsp;&nbsp;",
            new Ext.Button({ text: "Filter", handler: filterHandler }),
            new Ext.Button({ text: "Clear", handler: clearHandler })
            ];
        return config;
    },

    createView: function() {
        this.gridView = new Ext.grid.GridView({ forceFit: true });
        return this.gridView;
    },

    filter: function(entity, modifier, from, to) {
        var store = this.gridView.grid.store;
        var cnt = { "EntityTypeID": entity, "UserID": modifier, "From": this.formatDate(from), "To": this.formatDate(to) };
        store.load({ Cnt: cnt });
    },

    onFilter: function() {
        this.filter(this.entitySelect.getValue(), this.modifierSelect.getValue(), this.fromDate.getValue(), this.toDate.getValue());
    },

    formatDate: function(value) {
        if (value == null || value == "")
            return "";

        return value.format(extJsDateFormat);
    },

    onClear: function() {

        this.entitySelect.setValue("");
        this.modifierSelect.setValue("");
        this.fromDate.setValue("");
        this.toDate.setValue("");
        this.filter("", "", "", "");
    }

});