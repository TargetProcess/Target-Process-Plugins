Ext.ns('Tp.controls.Filter');

Tp.controls.Filter.ReleaseIterationFilter = Ext.extend(Tp.controls.Filter.ReleaseIterationStatelessFilter, {
	constructor: function (config) {
		if (!config.disableCookieState) this.stateEvents = ['filterChanged'];
		Ext.apply(this, config);
		Tp.controls.Filter.ReleaseIterationFilter.superclass.constructor.call(this, config);

		var filterData = this.getData();
		this.update(filterData);
		this.attachToEvents();

		if (this.onFilterChangedScript) {
			this.onFilterChangedScript.call(filterData.filterData);
		}
	},

	getState: function () {
		return { filterData: this.selectedPlannableFilterData };
	},

	applyState: function (state) {
		this.applySavedFilterToSelectedPlannableFilterData(state.filterData);
		this.save(this.selectedPlannableFilterData);
	},

	applySavedFilterToSelectedPlannableFilterData: function (stateFilterData) {
		Ext.each(stateFilterData, function (data) {
			for (var i = 0; i < this.selectedPlannableFilterData.length; i++) {
				if (this.selectedPlannableFilterData[i].projectId == data.projectId) {
					this.selectedPlannableFilterData[i] = new Tp.controls.Prioritization.PlannableFilterData(data);
				}
			}

			this.updateFilterTree(data);
		}, this);
	},

	// Update filter tree (check selected releases/iteration) according to list of releaseIds and iterationIds from saved state
	updateFilterTree: function (data) {
		var projectSelected = Array.findOne(this.releaseIterationFilterData, function (key) {
			return key.ProjectId == data.projectId
		});

		if (!projectSelected)
			return;

		// Update releases
		Ext.each(data.releaseIds, function (releaseId) {
			var release = Array.findOne(projectSelected.Releases, function (key) {
				return key.id == releaseId
			});
			if (release)
				release.checked = true;
		}, this);

		// Update iterations
		Ext.each(data.iterationIds, function (iterationId) {
			Ext.each(projectSelected.Releases, function (release) {
				var iteration = Array.findOne(release.children, function (key) {
					return key.id == iterationId
				});
				if (iteration)
					iteration.checked = true;
			}, this);
		}, this)
	}
});
