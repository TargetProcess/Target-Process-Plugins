tau.mashups
.addCSS('style.css')
.addDependency("libs/jquery/jquery")
.addDependency("libs/jquery/jquery.tmpl")
.addDependency("tau/mashups/TPSearch/SearchResults")
.addMashup(function (jQuery, jQueryTmpl, searchResults) {
	function tpSearch() { }
	tpSearch.prototype = {
		txtGeneralSearchId: 'ctl00_hdr_generalLookup_generalSearch_TpTextBoxSearchPattern',
		txtButtonGeneralSearchId: 'ctl00_hdr_generalLookup_generalSearch_btnSearch',
		btnGeneralSearchId: 'ctl00_hdr_generalLookup_generalSearch_btnUpdate',
		generalSearchBox: null,
		resultsDialog: null,
		lstEntityTypes: null,
		lstEntityStates: null,
		searchBox: null,
		timer: null,
		timeOut: 500,

		render: function () {
			var searchProxy = $.proxy(this.onSearchTextChange, this);

			this.searchBox = $('#ctl00_hdr_txtSearch');
			this.lstEntityTypes = $('#ctl00_hdr_generalLookup_generalSearch_lstTypes');
			this.lstEntityStates = $('#ctl00_hdr_generalLookup_generalSearch_lstStates');
			this.chkIgnoreProjectRelation = $('#ctl00_hdr_generalLookup_generalSearch_uxIgnoreProjectRelation');

			$('#ctl00_hdr_Panel1').attr('onkeypress', null).keypress(searchProxy);
			$('#ctl00_hdr_btnSearch').attr('onclick', null).click(function (e) {
				return e.originalEvent.detail == 0 ? false : searchProxy(e);
			});
			this.generalSearchBox = $('#' + this.txtGeneralSearchId).attr('onkeydown', null).keydown(searchProxy);
			$('#' + this.txtButtonGeneralSearchId).attr('onclick', null).click(searchProxy);
			$('#' + this.btnGeneralSearchId).attr('onclick', null).click(searchProxy);
		},

		onSearchTextChange: function (e) {
			if ((e.type === 'keypress' && e.which != 13) || (e.type === 'keydown' && e.which != 13) || (e.type === 'click' && e.which != 1)) return true;

			var origTarget = e.delegateTarget, searchData = { Command: 'Search', Plugin: 'Search', SearchString: '', EntityTypeId: null, EntityStateIds: [], ProjectIds: [] };
			var el;
			if (origTarget.id === this.txtGeneralSearchId || origTarget.id === this.txtButtonGeneralSearchId || origTarget.id === this.btnGeneralSearchId) {
				var advancedSearchString = this.generalSearchBox.val();
				if (Math.round(advancedSearchString) == advancedSearchString) return true;
				searchData.SearchString = advancedSearchString;
				var entityTypeId = this.lstEntityTypes.val();
				searchData.EntityTypeId = entityTypeId && entityTypeId.length > 0 ? entityTypeId : null;
				var entityStateName = this.lstEntityStates.val();
				if (entityStateName && entityStateName.length > 0) {
					if (entityTypeId && entityTypeId.length > 0) {
						searchData.EntityStateIds = window._entityStateCollection.getStateIdsForEntityType(entityStateName, entityTypeId);
					} else {
						searchData.EntityStateIds = window._entityStateCollection.getAllStateIdsForEntityState(entityStateName);
					}
				}
				if (!this.chkIgnoreProjectRelation.attr('checked')) {
					for (var i = 0; i < window._projectContext.length; i++) {
						searchData.ProjectIds.push(window._projectContext[i].id);
					}
				} else {
					for (var j = 0; j < window.projects.length; j++) {
						if (window.projects[j].type === "Project") {
							searchData.ProjectIds.push(window.projects[j].id);
						}
					}
				}
				el = '#ctl00_hdr_generalLookup_pnlFilter';
			} else {
				var simpleSearchString = this.searchBox.val();
				if (Math.round(simpleSearchString) == simpleSearchString) return true;
				searchData.SearchString = simpleSearchString;
				for (var k = 0; k < window._projectContext.length; k++) {
					searchData.ProjectIds.push(window._projectContext[k].id);
				}
				el = '#ctl00_hdr_Panel1';
			}
			if (searchData.SearchString && searchData.SearchString.length > 0) {
				this.resultsDialog = this.resultsDialog || new searchResults();
				this.resultsDialog.setPageSize(10);
				this.resultsDialog.search(searchData, el);
			    /*require(['main.search'], function (main) {
			        main.showSearchPopup(searchData);
			    });*/
			}
			return false;
		}
	};

	new tpSearch().render();
});