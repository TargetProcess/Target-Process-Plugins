 
 

//temp hack for #11313

Ext.ns('Tp.controls.grid.plugins');

/**
 * See bug #11313
 *
 * Revert effect of time zone conversion in script service proxy
 * so that users will see the same time as stored in the database
 * regardles of their local time zone settings. An vice versa,
 * the database will store users local time.
 *
 * TargetProcess does not support time zones, so the automatic
 * timezone conversion which happens in script service proxy
 * is not necessary.
 */
Ext.ns('Tp.controls.grid.plugins');
Tp.controls.grid.plugins.DateConverter = Ext.extend(Object, {

    grid: null,

    init: function(grid) {
        this.grid = grid;
        this.grid.on("saving", this.onDataSaving, this);
        this.grid.store.on("load", this.onStoreLoad, this);
    },

    onStoreLoad: function(store, rowRecords) {
        // this code is moved to TpReader.js as we can't call commit for each date - we have UI problems.
        /* for (var i = 0; i < rowRecords.length; i++) {
            var data = rowRecords[i].data;
            for (var key in data) {
                if (data[key] == null)
                    continue;
                if (data[key].toUniversalDate) {
                    rowRecords[i].set(key, data[key].toUniversalDate())
                    //we need to commit record's values to fire changes in store object.
                    rowRecords[i].commit();
                }
            }
        } */
    },

    onDataSaving: function(records) {
        for (i = 0; i < records.length; i++) {
            var data = records[i];
            for (var key in data) {
                if (data[key] instanceof Date) {
                    // we do not need to commit record's values as these prepared records are sent directly to web service. 
                    data[key] = data[key].toLocalDate();
                    
                }
            }
        }
    }
})