Ext.ns('Tp.components.Filters');

Tp.components.Filters.FilterSerializer = Ext.extend(Object, {
    hiddenFieldDataList: null,

    constructor: function (config) {
        Ext.apply(this, config);
    },

    initFilter: function(config){
        this.hiddenFieldDataList = config.hiddenFieldDataList;
    },

    save: function(filter) {
        Array.forEach(this.hiddenFieldDataList, function (hiddenFieldData) {
			var releaseHiddenField = Ext.get(hiddenFieldData.releasesHiddenId);
			var iterationHiddenField = Ext.get(hiddenFieldData.iterationsHiddenId);

			var filterProjectDataFound = Array.findOne(filter, function (filterProjectData) {
				return filterProjectData.projectId == hiddenFieldData.projectId;
			});

			if (!Ext.isEmpty(filterProjectDataFound)) {
				releaseHiddenField.dom.value = filterProjectDataFound.releaseIds.join(',');
				iterationHiddenField.dom.value = filterProjectDataFound.iterationIds.join(',');
			}
		});
    },
    
    load : function() {
        return Tp.controls.Prioritization.PlannableFilterData.GetFilterDataFromHidden(this.hiddenFieldDataList);
    }
});


