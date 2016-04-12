Ext.ns('Tp.controls.kanbanboard.sections');
/**
* Backlog contains items such as <em>Tasks</em> and <em>Bugs</em> that are in a planned state.
* Backlog allows dragging and dropping its items onto swimlanes to change item states.
* See {@link Tp.controls.kanbanboard.sections.Swimlane} for more details.
*/
Tp.controls.kanbanboard.sections.Backlog = Ext.extend(Tp.controls.kanbanboard.Container, {
	_itemsLoaded: false,

	resetItemsLoaded: function () {
		_itemsLoaded = false;
	},

	_isReloadPossible: true,

	canReload: function () {
		return this._isReloadPossible;
	},

	tryFireReloadEvent: function () {
		if (this._isReloadPossible) {
			this.fireReloadEvent();
		}
	},

	fireReloadEvent: function () {
		this._isReloadPossible = false;
		this.fireEvent("reload");
	},

	allowFireReloadEvent: function () {
		this._isReloadPossible = true;
	},

	/**
	* Specify visual oppearance of the backlog element with a particular CSS class.
	*/
	cls: 'kanban-backlog',

	/**
	* Backlog filter object.
	*/
	// private
	filter: null,

	//private
	controller: null,

	//private
	swimlane: null,
	collapsedCls: 'backlog-collapsed',

	loadMoreOption: null,

	constructor: function (config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}
		if (!config.filter) {
			throw new Error('Filter not specified');
		}

		config.layout = new Tp.controls.kanbanboard.ItemContainerLayout({ finalCallback: this.finalLayoutHandler.createDelegate(this) });

		Tp.controls.kanbanboard.sections.Backlog.superclass.constructor.call(this, config);

		this.swimlane = this.controller.swimlanes.initialSwimlane;

		this.filter.on('changed', function () {
			this.sortable = this.filter.sort === 'rank';
			this.applyFilter();
		}, this);

		this.controller.setBacklog(this);

		this.addEvents({ "reload": true, "loadItemsSuccess": true, "loadItemsFailed": true, "filterShown": true, "filterHidden": true });
		this.on('loadItemsSuccess', this.onloadItemsSuccess, this);
		this.on('loadItemsFailed', this.onloadItemsFailed, this);
	},

	onloadItemsSuccess: function () {
		this._itemsLoaded = true;
	},

	onloadItemsFailed: function () {
		this.allowFireReloadEvent();
	},

	finalLayoutHandler: function () {
		this.sort();
		//call repaint instead of doLayout due to performance issues
		this.repaint();
		this.applyFilter();
	},

	initComponent: function () {
		Tp.controls.kanbanboard.sections.Backlog.superclass.initComponent.call(this);
		this.addEvents("dropaccept");
	},

	highlighDisabled: function (item, entity) {
		if (entity == null) {
			this.removeClass('kanban-backlog-disabled');
		}
		else {
			if (!this.allowDrop(item, entity)) {
				this.addClass('kanban-backlog-disabled');
			}
		}
	},

	//private
	allowDrop: function (item, entity) {
		return this.swimlane.acceptEntity(entity);
	},

	//private
	onDrop: function (dd, e, data, comment, prev, next) {
		// change state of the dropped entity to the state of this swimlane
		this.fireEvent('dropaccept', dd, e, data, comment, this, prev, next);
	},

	comparator: function (a, b) {
		switch (this.filter.sort) {
			case 'name': return this.nameComparator(a, b);
			case 'rank': return this.rankComparator(a, b);
			case 'priority': return this.priorityComparator(a, b);
			case 'type': return this.typeComparator(a, b);
			case 'effort': return this.effortComparator(a, b);
			default: return 0;
		}
	},

	nameComparator: function (a, b) {
		if (a.name.toLowerCase() < b.name.toLowerCase()) {
			return -1;
		}
		if (a.name.toLowerCase() > b.name.toLowerCase()) {
			return +1;
		}
		return 0;
	},

	rankComparator: function (a, b) {
		return a.numericPriority - b.numericPriority;
	},

	priorityComparator: function (a, b) {
		return a.priorityImportance - b.priorityImportance;
	},

	typeComparator: function (a, b) {
		return a.entityType.id - b.entityType.id;
	},

	effortComparator: function (a, b) {
		return b.effort - a.effort;
	},

	/**
	* Update items displayed on the backlog according to the current filter.
	*/
	applyFilter: function () {
		// order items
		this.sort();
		// show/hide items based on entity type visibility
		this.items.each(function (item) {
			var visible = this.filterByType(item)
                    && this.filterByKeyword(item)
                    && this.filterByProject(item)
                    && this.filterByTag(item);
			if (visible) {
				item.show();
			}
			else {
				item.hide();
			}
		}, this);
		// redraw backlog
		this.repaint();
	},

	/**
	* Show specified entity types.
	*
	* @param item An item with entity.
	*/
	filterByType: function (item) {
		if (item.entity == null)
			return true;

		if (item.entity.entityType == this.controller.process.featureEntityType) {
			return this.filter.type['feature'];
		}
		if (item.entity.entityType == this.controller.process.userStoryEntityType) {
			return this.filter.type['userstory'];
		}
		if (item.entity.entityType == this.controller.process.bugEntityType) {
			return this.filter.type['bug'];
		}
		if (item.entity.entityType == this.controller.process.taskEntityType) {
			return this.filter.type['task'];
		}
		return true; // unknown type is visible anyway
	},

	/**
	* Combine keywords with OR operator.
	*
	* @param item An item with entity.
	*/
	filterByKeyword: function (item) {
		if (item.entity == null) {

			return true;
		}
		if (this.filter.keywords.length) {
			for (var n = 0; n < this.filter.keywords.length; n++) {
				var keyword = this.filter.keywords[n];
				//search by id
				var match = keyword.match(/^#?(\d+)$/);
				if (match) {
					keyword = match[1];
				}
				if (!isNaN(keyword) && keyword > 0) {
					if (item.entity.id == keyword) {
						return true;
					}
				}
				//search by keywords
				else if (item.entity.name.toLowerCase().indexOf(keyword) != -1) {
					return true;
				}
			}
			return false;
		}
		return true;
	},

	filterByProject: function (item) {
		if (item.entity == null) {
			return true;
		}

		if (this.filter.projectId > 0) {
			if (item.entity.project.id == this.filter.projectId)
				return true;
			return false;
		}
		return true;
	},

	/**
	* Combine tags with AND operator.
	*
	* @param item An item with entity.
	*/
	filterByTag: function (item) {
		if (item.entity == null) {
			return true;
		}

		if (this.filter.tags.length) {
			function containsTag(entity, tag) {
				for (var n = 0; n < entity.tags.length; n++) {
					if (entity.tags[n].toLowerCase().indexOf(tag) != -1) {
						return true;
					}
				}
				return false;
			}
			for (var n = 0; n < this.filter.tags.length; n++) {
				if (!containsTag(item.entity, this.filter.tags[n].toLowerCase())) {
					return false;
				}
			}
		}
		return true;
	},

	setRegion: function (region) {
		this.region = region;
		this.region.panel.on('beforeexpand', this.setExpandListener, this);
	},

	getIsCollapsed: function () {
		return this.region.isCollapsed;
	},

	setExpandListener: function () {
		this.tryFireReloadEvent();
	},

	isCommentRequired: function (data) {
		return this.swimlane.findEntityState(data.entity).requiredComment;
	}
});