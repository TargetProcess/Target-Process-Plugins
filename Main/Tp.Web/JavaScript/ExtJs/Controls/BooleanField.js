Ext.ns('Tp.controls');

Tp.controls.BooleanField = Ext.extend(Ext.form.ComboBox, {

    constructor: function(config) {
        Tp.util.validateForNulls([config]);
        config.disableKeyFilter = true;
        config.editable = false;
        config.store = [[true, "Yes"], [false, "No"]];
        config.mode = "local";
        config.allQuery = null;
        config.triggerAction = "all";
        Tp.controls.BooleanField.superclass.constructor.call(this, config);
    }

});

Ext.reg('tpbooleanfield', Tp.controls.BooleanField);