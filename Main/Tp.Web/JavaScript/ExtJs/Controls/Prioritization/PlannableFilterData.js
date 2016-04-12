Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.PlannableFilterData = Ext.extend(Object, {
    releaseIds: [],
    iterationIds: [],
    projectId: null,
    projectName: null,

    constructor: function(projectFilter) {
        this.releaseIds = [];
        this.iterationIds = [];
        this.projectId = null;
        this.projectName = null;
        Ext.apply(this, projectFilter);

        Tp.controls.Prioritization.PlannableFilterData.superclass.constructor.call(this);
    },

    setProjectId: function(projectId)
    {
        this.projectId = projectId;
    },

    setProjectName: function(projectName)
    {
        this.projectName = projectName;
    },

    addIterationId: function(iterationId)
    {
        this.iterationIds.push(iterationId);
    },

    addReleaseId: function(releaseId)
    {
        this.releaseIds.push(releaseId);
    },

    getReleaseNames: function(releaseIterationFilterData)
    {
        return this.getEntityNames(this.releaseIds, Function.createDelegate(this, this.getReleaseName), releaseIterationFilterData);
    },

    getIterationNames: function(releaseIterationFilterData)
    {
        return this.getEntityNames(this.iterationIds, Function.createDelegate(this, this.getIterationName), releaseIterationFilterData);
    },

    getReleaseName: function(entityId, releaseIterationFilterData) {
        var entityName = null;
        Array.forEach(
            releaseIterationFilterData,
            function(projectFilterData) {
                var matchedRelease = Array.findOne(projectFilterData.Releases, function(release) {
                    return (release.id == entityId);
                });

                if (matchedRelease) {
                    entityName = matchedRelease.text;
                }
            }, this);
        return entityName;
    },

    getEntityNames: function(entityIds, getEntityNameFunction, releaseIterationFilterData)
    {
        var entityNames = new Array();
        Array.forEach(entityIds,
                function(entityId) {
                    var name = getEntityNameFunction(entityId, releaseIterationFilterData);
                    if (!Ext.isEmpty(name))
                        entityNames.push(name);
                },
                this);
        return entityNames;
    },

    getIterationName: function(iterationId, releaseIterationFilterData) {
        var iterationName = null;
        Array.forEach(
            releaseIterationFilterData,
            function(projectFilterData) {
                var matchedRelease = Array.findOne(projectFilterData.Releases, function(release) {
                    var matchedIteration = Array.findOne(release.children, function(iteration) {
                        return iteration.id == iterationId;
                    });
                    if (!Ext.isEmpty(matchedIteration)) {
                        iterationName = matchedIteration.text;
                        return true;
                    }
                });
                if (!Ext.isEmpty(matchedRelease))
                    return;
            }, this);
        return iterationName;
    }
});


Tp.controls.Prioritization.PlannableFilterData.GetFilterDataFromTree = function(treeArray)
{
    var filterValues = {
        filter: []
    };

    Array.forEach(treeArray, function(tree) {

        var result = new Tp.controls.Prioritization.PlannableFilterData();
        result.setProjectId(tree.projectId);
        result.setProjectName(tree.projectName);

        Array.forEach(tree.getChecked(), function(item) {
            if (!item.disabled) {
                item.attributes.isIteration ? result.addIterationId(item.id) : result.addReleaseId(item.id);
            }
        }, this);

        filterValues.filter.push(result);
    });

    return filterValues;
};

Tp.controls.Prioritization.PlannableFilterData.GetFilterDataFromHidden = function(hiddenFieldDataList)
{
    var result = [];
    Array.forEach(hiddenFieldDataList, function(hiddenFieldData) {
        var plannableFilterData = new Tp.controls.Prioritization.PlannableFilterData();

        plannableFilterData.projectId = hiddenFieldData.projectId;

        var releaseHiddenField = Ext.get(hiddenFieldData.releasesHiddenId);
        plannableFilterData.releaseIds = releaseHiddenField.dom.value == '' ? [] : releaseHiddenField.dom.value.split(',');

        var iterationHiddenField = Ext.get(hiddenFieldData.iterationsHiddenId);
        plannableFilterData.iterationIds = iterationHiddenField.dom.value == '' ? [] : iterationHiddenField.dom.value.split(',');

        result.push(plannableFilterData);
    });

    return result;
}
