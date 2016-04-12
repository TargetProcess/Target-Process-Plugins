 
 

Ext.ns('Tp.controls.grid.plugins');
Tp.controls.grid.plugins.ErrorHighLighter = Ext.extend(Object, {
    grid: null,
    gridErrorRowClass: "gridErrorRow",

    init: function(grid) {
        this.grid = grid;
        this.grid.on(this.grid.successSaved, this.onSuccessSaved, this);
        this.grid.on(this.grid.fatalErrorSaved, this.onFatalErrorSaved, this);

        this.grid.store.on("load", this.onStoreLoad, this);

        // this event is fired by the ReportEditor Plugin after it reconfigures grid with new store and column model
        this.grid.on("initializeEvents", this.onInitializeEvents, this);
    },

    onInitializeEvents: function(gridHeader, limitOfRecords, arg) {
        // new store has been set to the grid by the ReportEditor
        this.grid.store.on("load", this.onStoreLoad, this);
    },

    onStoreLoad: function(sender, records, options) {
        if (options.command) {
            options.command();
            this.grid.store.lastOptions.command = null;
        }
    },

    onSuccessSaved: function(errors) {
        this.clearAllMessages();
        if (errors.length == 0) {
            this.commitChangesSave();
            SetLastAction('Changes saved successfully.');
            this.grid.store.reload();
        }
        else {
            var command = Function.createDelegate(this, function() {
                this.populateErrors(errors);
                this.grid.store.rejectChanges();
            });
            this.commitChangesSave();
            this.grid.store.reload({ command: command });
        }
    },

    onFatalErrorSaved: function(fatalError) {
        this.clearAllMessages();
        this.grid.store.rejectChanges();
        SetLastWarning(fatalError.get_message());
    },

    highLightErrors: function(errors) {
        Tp.util.validateForNulls([errors]);
        if (errors.length != 0) {
            var selectionModel = this.grid.getSelectionModel();
            for (var i = 0; i < errors.length; i++) {
                var index = this.grid.store.indexOfId(errors[i].Values["id"]);
                if (index == -1) {
                    throw "Row not found";
                }
                if (selectionModel.isSelected(index)) {
                    selectionModel.deselectRow(index);
                }
                var row = Ext.get(this.grid.getView().getRow(index));
                row.addClass(this.gridErrorRowClass);
            }
        }
    },

    createErrorMessage: function(error) {
        if (error.ColumnId) {
            return String.format("Item with ID = {0} is not saved. The problem is in the \"{1}\" column. {2}<br/>", error.Values["id"], error.ColumnId, error.Message);
        } else {
        return String.format("Item with ID = {0} is not saved. {1}<br/>", error.Values["id"], error.Message);
        }
    },

    populateErrors: function(errors) {
        Tp.util.validateForNulls([errors]);
        if (errors.length != 0) {
            this.highLightErrors(errors);
            var messageDump = "";
            for (var i = 0; i < errors.length; i++) {
                messageDump += this.createErrorMessage(errors[i]);
            }
            SetLastWarning(messageDump);
        }
    },

    rejectChangesForItems: function(errors) {
        for (var i = 0; i < errors.length; i++) {
            Tp.util.validateForNulls(errors[i].Values["id"]);
            this.grid.store.getById(errors[i].Values["id"]).reject();
        }
    },

    clearAllMessages: function() {
       SetLastAction(null);
       SetLastWarning(null);
       Tp.collections.createMixedCollection(Ext.getBody().query("." + this.gridErrorRowClass)).each(function(item){
           Ext.fly(item).removeClass(this.gridErrorRowClass)
       });
    },

    commitChangesSave: function() {
        try {
            this.grid.store.commitChanges();
        } catch (e) {
            //do nothing as there is only one way this exception can be propogated.
            //It can happen only if the item has been edited and not saved during page listing.
            //This block doesn't violate exception handling concepts because if it fails it immediately effects UI.
            //The red corners of edited cells doesn't get lost.
        }
    }
});