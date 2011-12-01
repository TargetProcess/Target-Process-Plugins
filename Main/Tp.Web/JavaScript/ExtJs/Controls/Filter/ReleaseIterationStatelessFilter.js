Ext.ns('Tp.controls.Filter');

Tp.controls.Filter.ReleaseIterationStatelessFilter = Ext.extend(Ext.Container, {
	filterSerializer: null,
    placeHolderElement: null,
	selectElementId: null,
	releaseIterationFilterData: null,
	releaseIterationFilterTitle: null,
	releaseIterationFilterPopup: null,
	xTemplate: null,
	releasesTerm: null,
	iterationsTerm: null,

	selectedPlannableFilterData: null,
	hideIterations: null,

	constructor: function (config) {
        if (config.serializerType){
            this.filterSerializer = eval(String.format('new {0}()', config.serializerType));
            this.filterSerializer.initFilter(config);
        }
        this.selectedPlannableFilterData = null;
		this.releaseIterationFilterData = null;
		this.hideIterations = null;

		Ext.apply(this, config);

		this.selectElementId = Ext.id();
        this.initSelectedPlannableFilterData(config.selectedReleaseIterationData);

        Ext.apply(config, {
            data: this.getData(),
            tpl: this.getTemplate()
        });

        Tp.controls.Filter.ReleaseIterationStatelessFilter.superclass.constructor.call(this, config);
        this.attachToEvents();
	},

    initSelectedPlannableFilterData: function(selectedReleaseIterationData) {
        this.selectedPlannableFilterData = [];
        Array.forEach(selectedReleaseIterationData, function (projectFilter) {
            this.selectedPlannableFilterData.push(new Tp.controls.Prioritization.PlannableFilterData(projectFilter));
        }, this);
    },

    attachToEvents: function() {
        Ext.get(this.selectElementId).on('click', this.onSelectClick, this);
    },

    getTemplate: function() {
                return this.xTemplate == ''
            ? new Ext.XTemplate(
                '<tpl for="filterData">',
                    '<tpl if="releaseNames.length &gt; 0 || iterationNames.length &gt; 0">',
                        '<div style="padding-bottom: 3px;"><strong>{projectName}</strong>',

                        '<tpl if="releaseNames.length &gt; 0">',
                            '<div style="white-space: normal; padding: 2px 0px 2px 10px;">',
                            '<tpl if="' + !this.hideIterations + '">',
                                '<dd style="color:#666; font-size: 11px">' + this.releasesTerm + ': </dd>',
                            '</tpl>',
                                    '<dd><tpl for="releaseNames">',
                                        '{.}' +
                                        '<tpl if="xindex == xcount">.</tpl>' +
                                        '<tpl if="xindex < xcount">,</tpl> ',
                                    '</tpl></dd>',
                            '</div>',
                        '</tpl>',

                        '<tpl if="iterationNames.length &gt; 0">',
                            '<div style="white-space: normal; padding: 2px 0px 2px 10px;">',
                                '<dd style="color:#666; font-size: 11px">{parent.iterationsTerm}: </dd>',
                                '<dd><tpl for="iterationNames">',
                                        '{.}' +
                                        '<tpl if="xindex == xcount">.</tpl>' +
                                        '<tpl if="xindex < xcount">,</tpl> ',
                                '</tpl></dd>',
                            '</div>',
                        '</tpl>',
                    '</div></tpl>',
                '</tpl>',
         '<a href="javascript:void(0)" id="{selectElementId}">Select</a>')
         : new Ext.XTemplate(this.xTemplate);
    },

    getData: function() {
        var selectElementId = this.selectElementId;
        var projectsFilter = this.selectedPlannableFilterData;
		if (!Ext.isEmpty(projectsFilter)) {
			Array.forEach(projectsFilter, function (projectFilter) {
				projectFilter.releaseNames = projectFilter.getReleaseNames(this.releaseIterationFilterData);
				projectFilter.iterationNames = projectFilter.getIterationNames(this.releaseIterationFilterData);
			}, this);
		}

		var projectFilterSliced = [];
		Array.forEach(projectsFilter, function (projectFilter) {
			if (projectFilter.releaseNames.length > 0 || projectFilter.iterationNames.length > 0) {
				projectFilterSliced.push(projectFilter);
			}
		}, this);

		var filterItems = [];
		Array.forEach(projectFilterSliced, function (projectFilter) {
			var projectAdded = false;
			if (projectFilter.releaseNames.length > 0) {
				for (var n = 0; n < projectFilter.releaseNames.length; n++) {
					filterItems.push({ project: projectAdded ? '' : projectFilter.projectName, item: projectFilter.releaseNames[n], type: 'release', beginGroup: n == 0, endGroup: n == projectFilter.releaseNames.length - 1 });
					projectAdded = true;
				}
			}
			if (projectFilter.iterationNames.length > 0) {
				for (var n = 0; n < projectFilter.iterationNames.length; n++) {
					filterItems.push({ project: projectAdded ? '' : projectFilter.projectName, item: projectFilter.iterationNames[n], type: 'iteration', beginGroup: n == 0, endGroup: n == projectFilter.iterationNames.length - 1 });
					projectAdded = true;
				}
			}
		}, this);

        return {
			selectElementId: selectElementId,
			filterData: projectFilterSliced,
			releasesTerm: this.releasesTerm,
			iterationsTerm: this.iterationsTerm,
			filterItems: filterItems
		};
    },

	onSelectClick: function () {
		if (!Ext.isEmpty(this.releaseIterationFilterPopup) && this.releaseIterationFilterPopup.isVisible())
			return;
		this.show();
	},

	show: function () {
		if (Ext.isEmpty(this.releaseIterationFilterPopup)) {
			this.releaseIterationFilterPopup = new Tp.controls.Filter.ReleaseIterationFilterPopup(
            {
            	releaseIterationFilterData: this.releaseIterationFilterData,
            	releaseIterationFilterTitle: this.releaseIterationFilterTitle
            });
			this.releaseIterationFilterPopup.on('filterSelected', this.onFilterSelected, this);
			// This is hack for firefox. Screen is blinking when popup is visible and tree filter is restoring.
			// So we decide to restore tree filter first and then show the popup.
			this.releaseIterationFilterPopup.render(Ext.getBody());
		}

		this.releaseIterationFilterPopup.restoreFilterValues(this.load());
		this.releaseIterationFilterPopup.show();
	},

    load : function() {
        if (this.filterSerializer)
            return this.filterSerializer.load();
    },

    save : function(filter) {
        if (this.filterSerializer)
            this.filterSerializer.save(filter);
    },

	onFilterSelected: function (filter) {
        this.selectedPlannableFilterData = filter;
        
        this.update(this.getData());
        this.attachToEvents();     

        this.fireEvent("filterChanged", filter);

        this.save(filter);

		if (this.onFilterChangedScript) {
			this.onFilterChangedScript.call(filter);
		}
	}
});

var taskBoardFilter = {
    showMore: function (id) {
        var selectedFilterValue = Ext.get(id);

        var hiddenContent = selectedFilterValue.query('[rel^=hiddenContent]');
        for (var n = 0; n < hiddenContent.length; n++)
            hiddenContent[n].style.display = 'inline';
        var dots = selectedFilterValue.query('[rel^=dots]')[0];
        dots.style.display = 'none';
        var less = selectedFilterValue.query('[rel^=less]')[0];
        less.style.display = 'inline';
    },
    showLess: function (id) {
        var selectedFilterValue = Ext.get(id);

        var hiddenContent = selectedFilterValue.query('[rel^=hiddenContent]');
        for (var n = 0; n < hiddenContent.length; n++)
            hiddenContent[n].style.display = 'none';
        var dots = selectedFilterValue.query('[rel^=dots]')[0];
        dots.style.display = 'inline';
        var less = selectedFilterValue.query('[rel^=less]')[0];
        less.style.display = 'none';
    }
}
