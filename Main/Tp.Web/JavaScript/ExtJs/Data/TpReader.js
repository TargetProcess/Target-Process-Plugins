Ext.data.TpReader = Ext.extend(Ext.data.JsonReader, {
    readRecords: function(recordSet) {
        var primaryIndex = recordSet.Header.indexOf("id");
        var recordType = this.recordType;
        fields = recordType.prototype.fields;
        var records = [];
        var root = recordSet.Records;
        for (var i = 0; i < root.length; i++) {
            var n = root[i];
            var values = {};
            var id = ((primaryIndex || primaryIndex === 0) && n[primaryIndex] !== undefined && n[primaryIndex] !== "" ? n[primaryIndex] : null);
            for (var j = 0; j < fields.length; j++) {
                var f = fields.items[j];
                var k = recordSet.Header.indexOf(f.name);
                var v = n[k] !== undefined ? n[k] : f.defaultValue;

                // we need replace null with empty string as empty field will be marked as edited value once editor shows.
                if (v === null) {
                    v = "";
                }
                values[f.name] = Tp.data.RecordHelper.getInstance().normalizeDate(v);
            }
            var record = new recordType(values, id);
            record.json = n;
            records[records.length] = record;
        }
        return {
            records: records,
            totalRecords: recordSet.Total
        };
    }
});