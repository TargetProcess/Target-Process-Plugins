 


Ext.ns('Tp.controls.grid.components');

Tp.controls.grid.components.SpecialCaseTable = Ext.extend(Ext.grid.EditorGridPanel, {
    getEmptyValue: function(columnName) {
        Tp.util.validateForNulls([columnName]);
        var value = Tp.controls.grid.components.SpecialCaseTable.emptyValuesMapping[columnName];
        if (value != null) {
            return value;
        }
        return Tp.controls.grid.components.SpecialCaseTable.emptyValuesMapping.defaultValue;
    },

    resetColumnsByType: function(entityType, gridHeader, record, value) {
        for (var i = 0; i < gridHeader.Columns.length; i++) {
            if (entityType == gridHeader.Columns[i].Facet.entityType) {
                record.set(gridHeader.Columns[i].Id, value);
            }
        }
    },

    findColumnHeaderById: function(columnId, gridHeader) {
        for (var i = 0; i < gridHeader.Columns.length; i++) {
            if (gridHeader.Columns[i].Id == columnId) {
                return gridHeader.Columns[i];
            }
        }
    },

    applyChangesToRecord: function(sourceColumnId, record, gridHeader) {
        Tp.util.validateForNulls([sourceColumnId, record, gridHeader]);
        var specialCase = Tp.controls.grid.components.SpecialCaseTable.cascadeChangesMapping[sourceColumnId];
        if (specialCase == null)
            return;
        var targetColumnHeader = this.findColumnHeaderById(specialCase.columnId, gridHeader);
        if (targetColumnHeader == null)
            return;
        this.resetColumnsByType(targetColumnHeader.Facet.entityType, gridHeader, record, specialCase.value);
    }
});

Tp.controls.grid.components.SpecialCaseTable.emptyValuesMapping = { "release": "- Backlog -", "iteration": "- Backlog -", "feature": "- No Feature -" };
Tp.controls.grid.components.SpecialCaseTable.emptyValuesMapping.defaultValue = "&nbsp;";
Tp.controls.grid.components.SpecialCaseTable.cascadeChangesMapping = { "release": { columnId: "iteration", value: "" } };

Tp.controls.grid.components.SpecialCaseTable.getInstance = function() {
    if (Tp.controls.grid.components.SpecialCaseTable.instance == null) {
        Tp.controls.grid.components.SpecialCaseTable.instance = new Tp.controls.grid.components.SpecialCaseTable();
    }
    return Tp.controls.grid.components.SpecialCaseTable.instance;
};

var SpecialCaseHandler = Tp.controls.grid.components.SpecialCaseTable;