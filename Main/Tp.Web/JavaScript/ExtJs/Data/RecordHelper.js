Ext.ns('Tp.data');

Tp.data.RecordHelper = Ext.extend(Object, {
    createDictionaryArray: function(recordSet) {
        var dictionary = [];
        for (var i = 0; i < recordSet.Records.length; i++) {
            var item = new Object();
            for (var j = 0; j < recordSet.Header.length; j++) {
                item[recordSet.Header[j]] = recordSet.Records[i][j];
            }
            dictionary.push(item);
        }
        return dictionary;
    },

    getHeaderArray: function(headerArr) {
        var arr = [];
        for (var i = 0; i < headerArr.length; i++) {
            arr.push({ name: headerArr[i] });
        }
        return arr;
    },

    createRecords: function(recordSet) {
        Tp.util.validateForNulls([recordSet]);
        var jsonData = new Object();
        jsonData.data = this.createDictionaryArray(recordSet);
        jsonData.results = recordSet.Records.length;
        var reader = new Ext.data.JsonReader({ totalProperty: "results", root: "data" }, Ext.data.Record.create(this.getHeaderArray(recordSet.Header)));
        return reader.readRecords(jsonData);
    },

    // revert effect of time zone conversion in script service proxy 
    // to get exactly the same time which is stored in database on the server
    normalizeDate: function(obj) {
        if (!(obj instanceof Date)) {
            return obj;
        }
        return obj.toUniversalDate();
    },

    normalizeGroupSummaryDates: function(groupSummary) {
        for (entry in groupSummary) {
            groupSummary[entry].GroupValue = this.normalizeDate(groupSummary[entry].GroupValue);
        }
        return groupSummary;
    }
});

Tp.data.RecordHelper.getInstance = function() {
    if (Tp.data.RecordHelper.instance == null) {
        Tp.data.RecordHelper.instance = new Tp.data.RecordHelper();
    }
    return Tp.data.RecordHelper.instance;
};