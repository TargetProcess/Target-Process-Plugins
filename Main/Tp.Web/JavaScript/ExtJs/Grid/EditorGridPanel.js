Ext.ns('Tp.controls.grid');

Tp.controls.grid.EditorGridPanel = Ext.extend(Ext.grid.EditorGridPanel, {
    onFatalErrorSavedDelegate: null,
    onSuccessSavedDelegate: null,
    successSaved: "successSaved",
    afterSuccessSaved: "afterSuccessSaved",
    fatalErrorSaved: "fatalErrorSaved",
    initializeEvents: "initializeEvents",
    save: "save",
    saving: "saving",
    saveButton: null,

    constructor: function(config) {
        this.onPreInit(config);
        Tp.controls.grid.EditorGridPanel.superclass.constructor.call(this, config);
        this.addEvents(this.save);
        this.addEvents(this.saving);
        this.addEvents(this.successSaved);
        this.addEvents(this.afterSuccessSaved);
        this.addEvents(this.fatalErrorSaved);
        this.addEvents(this.initializeEvents);
        this.onFatalErrorSavedDelegate = Function.createDelegate(this, this.onFatalErrorSaved);
        this.onSuccessSavedDelegate = Function.createDelegate(this, this.onSuccessSaved);
        var thisRef = this;
        window.onbeforeunload = function() {
            if (thisRef.isDataModified()) {
                return "This operation discards the change(s) you made, are you sure want to leave this page?";
            } else {
                return undefined;
            }
        };
        this.store.on("beforeload", this.onStoreBeforeLoad, this);
        this.on("save", this.onSave, this);
        this.on("beforeedit", this.onBeforeEdit, this);
        this.on("afteredit", this.onAfterEdit, this);
        this.on("headermousedown", this.onHeaderMouseDown, this);

        registerViewOpeningInParent('#' + config.renderTo);
    },

    onPreInit: function(config) {
        if (config.showSaveButton) {
            config.tbar = (config.tbar == null) ? new Array() : config.tbar;
            this.saveButton = new Ext.Button({ text: 'Submit Changes', disabled: true, tooltip: 'Save changes made to the grid' });
            this.saveButton.on("click", this.onSaveClick, this);
            config.tbar.push('-');
            config.tbar.push(this.saveButton);
        }
    },

    onColumnResize: function(index, size) {
        Tp.controls.grid.EditorGridPanel.superclass.onColumnResize.call(this, index, size);
        this.stopEditing();
    },

    onSuccessSaved: function(errors) {
        this.fireEvent(this.successSaved, errors);
        this.fireEvent(this.afterSuccessSaved);
    },

    onFatalErrorSaved: function(fatalError) {
        this.fireEvent(this.fatalErrorSaved, fatalError);
    },

    isDataModified: function() {
        return (this.store.getModifiedRecords().length > 0) ? true : false;
    },

    recordsToArray: function(records) {
        var arr = [];
        for (i = 0; i < records.length; i++) {
            arr.push(records[i].data);
        }
        return arr;
    },

    save: function() {
        var records = this.recordsToArray(this.store.getModifiedRecords());
        this.fireEvent(this.saving, records);
        Tp.Web.Extensions.ExtJs.Web.GridService.SaveRecords(this.gridDataRequest, records, this.onSuccessSavedDelegate, this.onFatalErrorSavedDelegate);
    },

    safeSave: function() {
        this.stopEditing();
        if (!this.isDataModified())
            return;
        if (window.confirm("This operation discards the change(s) you made, do you want to save changes?"))
            this.fireEvent("save");
        else
            this.store.rejectChanges();
    },

    onStoreBeforeLoad: function() {
        this.safeSave();
    },

    onHeaderMouseDown: function() {
        this.safeSave();
    },

    onSave: function() {
        if (this.isDataModified()) {
            this.save();
        }
    },

    onSaveClick: function() {
        this.fireEvent("save");
    },

    onBeforeEdit: function(arg) {
        Tp.controls.grid.CurrentEditableItem.setInstance({ columnName: arg.field, record: arg.record, grid: this });
    },

    onAfterEdit: function(arg) {
        var item = Tp.controls.grid.CurrentEditableItem.getInstance();
        Tp.util.validateForNulls([item]);
        if (item.associativeRecord != null) {
            for (key in item.associativeRecord.data) {
                item.record.set(key, item.associativeRecord.data[key]);
            }
        }

        SpecialCaseHandler.getInstance().applyChangesToRecord(item.columnName, item.record, item.grid.gridHeader);
        Tp.controls.grid.CurrentEditableItem.reset();
    }
});
