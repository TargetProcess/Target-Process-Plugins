Ext.ns('Tp.controls.kanbanboard');

/**
* A controller which listens to kanban board widget events,
* communicates with the remoting AJAX service, updates widgets.
*/
Tp.controls.kanbanboard.Controller = Ext.extend(Ext.util.Observable, {
	service: null,
	project: null,
	process: null,
	uxKanbanBoardPanel: null,
	swimlanes: [],
	uxbacklog: null,
	entityHashmap: null,
	uxswimlanes: [],
	enabled: true,
	renderedItemsInBacklog: 0,
	itemsCountPerRendering: 20,
	userStoryEntityPopup: null,
	refreshInterval: null,
	showTasksAsCards: false,
	refreshTask: null,
	constructor: function (clientId, project, process, preferences) {
        if (!clientId){
            throw new Error('ClientId is not specified');
        }
        if (!project) {
            throw new Error('Project not specified');
        }
        if (!process) {
            throw new Error('Process not specified');
        }

		Tp.controls.kanbanboard.Controller.superclass.constructor.call(this);
        this.clientId = clientId;
		this.service = Tp.Web.PageServices.KanbanBoard.KanbanBoardService;
		this.project = project;
		this.process = process;
		this.entityHashmap = {};
		this.uxswimlanes = [];

		this.addEvents({
			"statechanged": true,
			"showDetails": true
		});

		this.swimlanes = this.applyPreferences(this.initSwimlanes(), preferences || { reFresh: 0, order: [], limits: {} });
		this.refreshInterval = isNaN(preferences.reFresh) || preferences.reFresh < 0 ? 0 : preferences.reFresh;
		this.showTasksAsCards = !preferences.doNotShowTasksAsCards;
		this.preferences = preferences;

		this.refreshTask = new Ext.util.DelayedTask(function (task, delay) {
			this.uxKanbanBoardPanel.tryFireReloadEvent();
			task.delay(delay);
		}, this);
	},

	preferences: null,

	load: function () {
		this.clearSwimlanes();

		var span = Ext.fly("panel-task-filter");
		if (span) {
			span.setVisible(this.showTasksAsCards);
		}

		this.service.loadEntities(this.project.id, this.safe(this.project.releaseIds), this.safe(this.project.iterationIds),
				this.loadEntities_onSuccess.createDelegate(this),
				this.loadEntities_onFailure.createDelegate(this));
	},

	clearSwimlanes: function () {
		Ext.each(this.uxswimlanes, function (swimlane) {
			swimlane.removeAll();
			if (swimlane.swimlane.entities)
				swimlane.swimlane.entities = [];
			this.entityHashmap[swimlane.swimlane.name] = [];
		}, this);
		if (this.uxKanbanBoardPanel) {
			this.uxKanbanBoardPanel.resetItemsLoaded();
		}
	},

	clearBacklog: function () {
		if (this.uxbacklog) {
			this.entityHashmap[this.uxbacklog.swimlane.name] = [];
			this.uxbacklog.removeAll();
			this.uxbacklog.resetItemsLoaded();
		}
	},

	/**
	* Enable drag and drop operations.
	*/
	enable: function () {
		this.enabled = true;
		this.startAutoRefresh();
	},

	/**
	* Disable drag and drop operations.
	*/
	disable: function () {
		this.stopAutoRefresh();
		this.enabled = false;
	},

	/**
	* Save board preferences to the server.
	*/
	savePreferences: function () {
		var preferences = {
			reFresh: 0,
			order: [],
			limits: {},
			isCollapsed: null,
			showTasksAsCards: false
		};
		var plannedSwimlane = this.swimlanes.plannedSwimlane;
		if (plannedSwimlane.limit > 0) {
			preferences.limits[plannedSwimlane.id] = plannedSwimlane.limit;
		}
		for (var n = 0; n < this.swimlanes.length; n++) {
			var swimlane = this.swimlanes[n];
			preferences.order.push(swimlane.id);
			if (swimlane.limit > 0) {
				preferences.limits[swimlane.id] = swimlane.limit;
			}
		}
		preferences.reFresh = this.refreshInterval;
		preferences.doNotshowTasksAsCards = !this.showTasksAsCards;
		this.service.savePreferences(this.project.id, preferences, this.load.createDelegate(this));
	},

	setBacklog: function (backlog) {
		this.uxbacklog = backlog;
		backlog.on('enterdrag', this.onEnterDrag, this);
		backlog.on('begindrag', this.onBeginDrag, this);
		backlog.on('enddrag', this.onEndDrag, this);
		backlog.on('positionchanged', this.onPositionchanged, this);
		backlog.on('dropaccept', this.onDropAccept, this);
		backlog.on('reload', this.loadBacklogEntities, this);
		backlog.on('destroy', function (cmp) {
			this.uxbacklog = null;
		}, this);
		backlog.on('filterShown', this.recalculateHeight, this);
		backlog.on('filterHidden', this.recalculateHeight, this);
	},

	setFilter: function (filter) {
		this.filter = filter;
	},

	setKanbanBoardPanel: function (uxKanbanBoardPanel) {
		this.uxKanbanBoardPanel = uxKanbanBoardPanel;
		this.uxKanbanBoardPanel.on('reload', this.load, this);
		this.startAutoRefresh();
	},

	startAutoRefresh: function () {
		if (!isNaN(this.refreshInterval) && this.refreshInterval > 0) {
			this.refreshTask.delay(this.refreshInterval * 60000, null, null, [this.refreshTask, this.refreshInterval * 60000]);
		}
	},

	stopAutoRefresh: function () {
		if (this.refreshTask) {
			this.refreshTask.cancel();
		}
	},

	recalculateHeight: function () {
		this.uxKanbanBoardPanel.recalculateHeight();
	},

	addSwimlane: function (uxswimlane) {
		uxswimlane.on('enterdrag', this.onEnterDrag, this);
		uxswimlane.on('begindrag', this.onBeginDrag, this);
		uxswimlane.on('enddrag', this.onEndDrag, this);
		uxswimlane.on('dropaccept', this.onDropAccept, this);
		uxswimlane.on('destroy', function (cmp) {
			this.uxswimlanes.remove(cmp);
		}, this);

		this.uxswimlanes.push(uxswimlane);
		this.fillItemContainer(uxswimlane, this.entityHashmap[uxswimlane.swimlane.name] || [], true);
	},

	onEnterDrag: function (itemContaier, item, entity) {
		this.uxbacklog.highlighDisabled(item, entity);
		for (var n = 0; n < this.uxswimlanes.length; n++) {
			this.uxswimlanes[n].highlighDisabled(item, entity);
		}
	},

	onBeginDrag: function (itemContaier, item, entity) {
		this.uxbacklog.enableSorting(entity.entityType.isEditValid);
		for (var n = 0; n < this.uxswimlanes.length; n++) {
			this.uxswimlanes[n].enableSorting(entity.entityType.isEditValid);
		}
		this.isInDrag = true;
	},

	onEndDrag: function (itemContaier, item, entity) {
		this.isInDrag = false;
		this.uxbacklog.highlighDisabled(item, null);
		this.uxbacklog.enableSorting(true);
		for (var n = 0; n < this.uxswimlanes.length; n++) {
			this.uxswimlanes[n].highlighDisabled(item, null);
			this.uxswimlanes[n].enableSorting(true);
		}
	},

	onPositionchanged: function (itemContaier, item, entity, proxyEl) {
	},

	/**
	* Handles entity drop onto a swimlane event.
	*
	* @param {Tp.controls.kanbanboard.ItemContainer} swimlane A swimlane onto which the entity has been dropped.
	* @param {Tp.data.Entity} entity An entity that has been dropped.
	* @param {Tp.data.EntityState} entityState A new entity for the dropped state.
	*/
	//private
	onDropAccept: function (dd, e, data, comment, swimlane, prev, next) {
		var entityState = swimlane.swimlane.findEntityState(data.entity);
		// Call remote service to change entity state on the server.
		var ctx = { dd: dd, e: e, data: data, swimlane: swimlane, entityState: entityState, before: next, after: prev };
		this.service.changeEntityState(this.clientId, data.entity.id, entityState.id, comment, next != null ? next.entity.id : -1, prev != null ? prev.entity.id : -1,
			this.changeEntityState_onSuccess.createDelegate(this),
			this.changeEntityState_onFailure.createDelegate(this),
			ctx);
	},

	getSwimlaneByName: function (swimlaneName) {
		for (var i = 0; i < this.uxswimlanes.length; i++) {
			if (this.uxswimlanes[i].swimlane.name == swimlaneName) {
				return this.uxswimlanes[i];
			}
		}
		if (this.uxbacklog.swimlane.name == swimlaneName)
			return this.uxbacklog;

		return undefined;
	},

	//private
	initSwimlanes: function () {
		var id = 0;

		/**
		* Swimlane configuration class.
		*
		* @param name Swimlane internal name.
		*/
		var Swimlane = function (name, title, process) {
			this.id = 'x-' + id++; // id by default, updated when entity types are collected
			this.name = name;
			this.title = title || '<untitled>'; // title by default, updated when entity types are collected
			this.limit = 0; // zero means no limit
			this.entityStates = [];
			this.process = process;
			/**
			* Adds new entity state to this swimlane.
			* @param {Tp.data.EntityState} entityState A state.
			*/
			this.addEntityState = function (entityState, process) {
				this.entityStates.push(entityState);

				var ids = [];
				var labels = [];
				labels.add = function (label) {
					if (this.indexOf(label) == -1) {
						this.push(label);
					}
				};
				for (var n = 0; n < this.entityStates.length; n++) {
					if (!(!process.bugTrackingPractice && this.entityStates[n].entityType == process.bugEntityType)) {
						ids.push(this.entityStates[n].id);
						labels.add(this.entityStates[n].name);
					}
				}
				this.id = 's-' + ids.sort().join('-');
				this.title = labels.join(', ');
			};
			/**
			* Verifies whether the specified entity in a state from this swimlane.
			* @param {Tp.data.Entity} entity An entity.
			*/
			this.matchEntity = function (entity) {
				for (var n = 0; n < this.entityStates.length; n++) {
					if (entity.entityState == this.entityStates[n]) {
						return true;
					}
				}
				return false;
			};
			/**
			* Verifies whether the specified entity in a state that has transition
			* to any of the states on this swimlane.
			* @param {Tp.data.Entity} entity An entity.
			*/
			this.acceptEntity = function (entity) {
				for (var n = 0; n < this.entityStates.length; n++) {
					if (entity.entityState.hasTransitionTo(this.entityStates[n])) {
						return true;
					}
				}
				return false;
			};
			/**
			* Find appropriate entity state from this swimlane for the specified entity.
			* @param entity {Tp.data.Entity} entity An entity.
			*/
			this.findEntityState = function (entity) {
				for (var n = 0; n < this.entityStates.length; n++) {
					if (entity.entityType == this.entityStates[n].entityType) {
						return this.entityStates[n];
					}
				}
				throw new Error('Appropriate entity state not found');
			};
		};

		var swimlanes = [];

		/**
		* Array of swimlane configurations.
		*/
		Ext.apply(swimlanes, {
			/**
			* Swimlane for initial states.
			*/
			initialSwimlane: new Swimlane('__initial__', 'Initial', this.process),
			/**
			* Swimlane for final states.
			*/
			finalSwimlane: new Swimlane('__final__', 'Final', this.process),
			/**
			* Swimlane for planned states.
			*/
			plannedSwimlane: new Swimlane('__planned__', 'Planned', this.process),

			projectId: this.project.id,

			process: this.process,
			/**
			* Get swimlane by id. It also searches in initial, final and planned swimlanes.
			* @param id A swimlane id.
			*/
			item: function (id) {
				if (this.initialSwimlane.id == id) {
					return this.initialSwimlane;
				}
				if (this.finalSwimlane.id == id) {
					return this.finalSwimlane;
				}
				if (this.plannedSwimlane.id == id) {
					return this.plannedSwimlane;
				}
				for (var n = 0; n < this.length; n++) {
					if (this[n].id == id) {
						return this[n];
					}
				}
				throw new Error('Swimlane not found');
			},
			/**
			* Get appropriate swimlane for a specified state. Create new swimlane if needed.
			* @param entityState A state.
			*/
			getSwimlaneForEntityState: function (entityState) {
				if (entityState['initial']) {
					return this.initialSwimlane;
				}
				if (entityState['final']) {
					return this.finalSwimlane;
				}
				if (entityState['planned']) {
					return this.plannedSwimlane;
				}
				for (var n = 0; n < this.length; n++) {
					if (this[n].name == entityState.name) {
						return this[n];
					}
				}

				var swimlane = new Swimlane(entityState.name, null, this.process);
				this.push(swimlane);
				return swimlane;
			},
			/**
			* Add the specified state to an already existing or newly created swimlane.
			* @param entityState A state.
			*/
			addEntityState: function (entityState, process) {
				this.getSwimlaneForEntityState(entityState).addEntityState(entityState, process);
			},

			hasPlannedSwimlane: function () {
				return this.plannedSwimlane.entityStates.length > 0;
			}
		});

		var wipStatesOnBoard = false;

		for (var n = 0; n < this.process.entityStates.length; n++) {
			var entityState = this.process.entityStates[n];
			if (this.isWIP_OR_PlannedState(entityState) && this.process.bugTrackingPractice && entityState.entityType == this.process.bugEntityType) {
				swimlanes.addEntityState(entityState, this.process);
				wipStatesOnBoard = true;
			}
			else if (this.isWIP_OR_PlannedState(entityState) && (entityState.entityType == this.process.userStoryEntityType ||
			entityState.entityType == this.process.taskEntityType)) {
				swimlanes.addEntityState(entityState, this.process);
				wipStatesOnBoard = true;
			}
			else if (!this.isWIP_OR_PlannedState(entityState)) {
				if (entityState.entityType == this.process.userStoryEntityType
						|| entityState.entityType == this.process.bugEntityType
						|| entityState.entityType == this.process.taskEntityType) {
					swimlanes.addEntityState(entityState, this.process);
				}
			}
		}
		if (!wipStatesOnBoard)
			swimlanes.push(new Swimlane("Error" + this.project.id, "<span style='color:red;'>There are no WIP entity states. You may add them in Admin <span style='font-size:14px;'>&rarr;</span> Process <span style='font-size:14px;'>&rarr;</span> Workflow</span>"));

		// Make it a map: 'swimlane.Id' -> 'swimlane'.
		for (var n = 0; n < swimlanes.length; n++) {
			swimlanes[swimlanes[n].id] = swimlanes[n];
		}

		return swimlanes;
	},

	isWIP_OR_PlannedState: function (entityState) {
		return !(entityState['initial'] || entityState['final']);
	},

	//private
	applyPreferences: function (swimlanes, preferences) {
		// Assign limits from preferences.
		if (swimlanes.plannedSwimlane.id in preferences.limits) {
			swimlanes.plannedSwimlane.limit = parseInt(preferences.limits[swimlanes.plannedSwimlane.id]);
		}
		for (var n = 0; n < swimlanes.length; n++) {
			if (swimlanes[n].id in preferences.limits) {
				swimlanes[n].limit = parseInt(preferences.limits[swimlanes[n].id]);
			}
		}

		/**
		* Move the specified item to the specified index in the specified array.
		* @param array An array.
		* @param item An item to move to the specified index.
		* @param index New item index.
		*/
		function moveTo(array, item, index) {
			var n = array.indexOf(item);
			array.splice(n, 1);
			array.splice(index, 0, item);
		}

		function comparePreferences(first, second){			
			var firstIds = first.split('-');		
			var secondIds = second.split('-');
			var counter = 0;
			
			for (var i=0; i< firstIds.length; i++){
				if(secondIds.indexOf(firstIds[i]) >= 0){
					counter++;
				}
			}
			
			return counter == firstIds.length || counter == secondIds.length;
		}				
		
		// Reorder swimlanes according to preferences.
		var offset = 0;
		for (var n = 0; n < preferences.order.length; n++) {
			for(var m = 0; m < swimlanes.length; m++){
				if (comparePreferences(preferences.order[n], swimlanes[m].id)) {
					var swimlane = swimlanes[m];
					moveTo(swimlanes, swimlane, offset);
					offset++;
				}
			}
		}
		return swimlanes;
	},

    createItem: function(entity){
        var item = new Tp.controls.kanbanboard.Item({
            entity: entity,
            canShowTooltip: function () { return !self.isInDrag; },
            listeners: {
                showDetails: {
                    fn: this.showDetails,
                    scope: this
                }
            }
        });
        item.on('onBugsPopupShow', this.bugsPopupShowHandler.createDelegate(this));
        item.on('onTasksPopupShow', this.tasksPopupShowHandler.createDelegate(this));
        item.on('onImpedimentsPopupShow', this.impedimentsPopupShowHandler.createDelegate(this));
        return item;
    },

	fillItemContainer: function (itemContainer, entities) {
		var maxItemsPerSwimlane = 100;
		var self = this;
		entities.sort(itemContainer.comparator.createDelegate(itemContainer));
		for (var i = 0; i < entities.length; i++) {
			if ((itemContainer.swimlane.name == '__final__') && i >= 10)
				break;
			if (this.isWIP_OR_PlannedState(entities[i].entityState) && i >= maxItemsPerSwimlane)
				break;
            var item = this.createItem(entities[i]);
			itemContainer.add(item);
			item.on('onBugsPopupShow', this.bugsPopupShowHandler.createDelegate(this));
			item.on('onTasksPopupShow', this.tasksPopupShowHandler.createDelegate(this));
			item.on('onImpedimentsPopupShow', this.impedimentsPopupShowHandler.createDelegate(this));
		}
		itemContainer.doLayout();
	},

	showDetails: function (data) {
		this.isInDrag = false;
		this.fireEvent('showDetails', data);
	},

	lastItem: null,

	tasksPopupShowHandler: function (item) {
		this.showUserStoryEntityPopup(item, new Tp.controls.kanbanboard.board.TaskPopupStrategy({ service: this.service }), this.process.taskEntityType.titlePlural);
	},

	bugsPopupShowHandler: function (item) {
		this.showUserStoryEntityPopup(item, new Tp.controls.kanbanboard.board.BugPopupStrategy({ service: this.service }), this.process.bugEntityType.titlePlural);
	},

	impedimentsPopupShowHandler: function (item) {
		this.showUserStoryEntityPopup(item, new Tp.controls.kanbanboard.board.ImpedimentPopupStrategy({ service: this.service }), this.process.impedimentEntityType.titlePlural);
	},

	showUserStoryEntityPopup: function (item, strategy, entityTitle) {
		this.lastItem = item;
		this.uxKanbanBoardPanel.disable();

		if (this.userStoryEntityPopup == null) {
			this.userStoryEntityPopup = new Tp.controls.kanbanboard.board.UserStoryEntityPopup({ service: this.service });
			this.userStoryEntityPopup.on('hide', this.popupHideHandler.createDelegate(this));
		}

		this.userStoryEntityPopup.setStrategy(strategy);
		this.userStoryEntityPopup.setTitle(entityTitle + ' for ' + this.process.userStoryEntityType.title + ' ' + item.getEntityName());
		this.userStoryEntityPopup.loadData(item.entity);
	},

	popupHideHandler: function (popup) {
		var c = 0;
		Array.forEach(popup.store.data.items,
			function (i) {
				Array.forEach(this.process.entityStates,
					function (s) {
						if (s.id == i.data.entityStateId && !s.final)
							c++;
					}, this);
			}, this);
		popup.getStrategy().updateItem(this.lastItem, c);
		this.uxKanbanBoardPanel.enable();
	},

	placeItemOnItemContainer: function (item, doLayout) {
		if (this.uxbacklog.swimlane.matchEntity(item.entity)) {
			this.uxbacklog.add(item);
		}
		else {
			for (var n = 0; n < this.uxswimlanes.length; n++) {
				if (this.uxswimlanes[n].swimlane.matchEntity(item.entity)) {
					this.uxswimlanes[n].add(item);
				}
			}
		}
		if (doLayout) {
			this.uxbacklog.doLayout();
			for (var n = 0; n < this.uxswimlanes.length; n++) {
				this.uxswimlanes[n].doLayout();
			}
		}
	},

	fixEntities: function (entities) {
		for (var n = 0; n < entities.length; n++) {
			var entity = entities[n];

			entity.entityType = this.process.getEntityType(entity.entityTypeId);
			delete entity.entityTypeId;

			entity.entityState = this.process.getEntityState(entity.entityStateId);
			delete entity.entityStateId;
		}
		return entities;
	},

	collectEntitiesByState: function (entities) {
		entities = this.fixEntities(entities);
		this.entityHashmap[this.uxbacklog.swimlane.name] = this.entityHashmap[this.uxbacklog.swimlane.name] || [];
		for (var m = 0; m < this.uxswimlanes.length; m++) {
			this.entityHashmap[this.uxswimlanes[m].swimlane.name] = this.entityHashmap[this.uxswimlanes[m].swimlane.name] || [];
		}

		for (var i = 0; i < entities.length; i++) {
			if (this.uxbacklog.swimlane.matchEntity(entities[i])) {
				this.entityHashmap[this.uxbacklog.swimlane.name].push(entities[i]);
			}

			for (var m = 0; m < this.uxswimlanes.length; m++) {
				if (this.uxswimlanes[m].swimlane.matchEntity(entities[i])) {
					this.entityHashmap[this.uxswimlanes[m].swimlane.name].push(entities[i]);
				}
			}
		}
	},

	loadBacklogEntities: function () {
		this.clearBacklog();
		this.service.loadBacklogEntities(this.project.id, this.safe(this.project.releaseIds), this.safe(this.project.iterationIds),
				this.loadBacklogEntities_onSuccess.createDelegate(this),
				this.loadBacklogEntities_onFailure.createDelegate(this));
	},

	loadEntities_onSuccess: function (entities) {
		this.uxbacklog.allowFireReloadEvent();

		this.reload();

		this.collectEntitiesByState(entities);

		for (var m = 0; m < this.uxswimlanes.length; m++) {
			this.fillItemContainer(this.uxswimlanes[m], this.entityHashmap[this.uxswimlanes[m].swimlane.name], true);
		}

		this.uxKanbanBoardPanel.fireEvent("loadItemsSuccess");
	},

	reload: function () {
		if (!this.uxbacklog.getIsCollapsed()) {
			this.uxbacklog.fireReloadEvent();
		}
	},

	loadEntities_onFailure: function (x) {
		Tp.util.Notifier.error('Error', 'Please, reload this page.\n<hr/>\n' + x.get_message());
		this.uxKanbanBoardPanel.fireEvent("loadItemsFailed");
	},

	loadBacklogEntities_onSuccess: function (entities) {
		this.collectEntitiesByState(entities);
		this.fillItemContainer(this.uxbacklog, this.entityHashmap[this.uxbacklog.swimlane.name], true);
		this.uxbacklog.fireEvent("loadItemsSuccess");
	},

	loadBacklogEntities_onFailure: function (x) {
		Tp.util.Notifier.error('Error', 'Please, reload this page.\n<hr/>\n' + x.get_message());
		this.uxbacklog.fireEvent("loadItemsFailed");
	},

	changeEntityState_onSuccess: function (x, ctx) {
		ctx.dd.onValidDrop(ctx.swimlane.dropTarget, ctx.e, ctx.swimlane.dropTarget.id);
		ctx.swimlane.el.removeClass(ctx.swimlane.dropTarget.overClass);
		ctx.data.entity.endDate = x.EndDate;
		if ((ctx.swimlane.swimlane.name == '__final__') && ctx.swimlane.items.length >= 10) {
			ctx.swimlane.remove(ctx.swimlane.items.items[0]);
		}
		if (x.Priority != null && x.Priority !== -1) {
			ctx.data.entity.numericPriority = x.Priority;
		}
		ctx.data.entity.entityState = ctx.entityState;
		ctx.dd.proxy.repair(ctx.dd.getRepairXY(ctx.e, ctx.dd.dragData), function () {
			ctx.dd.afterRepair();
			if (ctx.swimlane.canSort()) {
				ctx.swimlane.insert(ctx.swimlane.currentProxyPos, ctx.data.item);
			} else {
				ctx.swimlane.add(ctx.data.item);
				ctx.swimlane.doLayout(); // repaint this container to display newly added items
			}
			//Retrieve entities in correct sort order and put it for further usage
			for (var swimlaneName in this.entityHashmap) {
				var foundSwimlaneUi = this.getSwimlaneByName(swimlaneName);
				if (typeof (foundSwimlaneUi) == undefined) continue;

				if (foundSwimlaneUi.items == undefined || foundSwimlaneUi.items.items == undefined)
					continue;
				this.entityHashmap[foundSwimlaneUi.swimlane.name] = [];

				for (var i = 0; i < foundSwimlaneUi.items.items.length; i++) {
					var item = foundSwimlaneUi.items.items[i];
					this.entityHashmap[foundSwimlaneUi.swimlane.name].push(item.entity);
				}
			}
			this.fireEvent('statechanged', this, ctx.data.entity);
			ctx.data.item.onRenderHandler();
			Tp.util.Notifier.notify('OK', 'State changed to "' + ctx.entityState.name + '"');
		}, this);
	},

	changeEntityState_onFailure: function (x, ctx) {
		ctx.dd.onInvalidDrop(ctx.swimlane.dropTarget, ctx.e, ctx.swimlane.dropTarget.id);
		ctx.swimlane.el.removeClass(ctx.swimlane.dropTarget.overClass);
		this.placeItemOnItemContainer(ctx.data.item, true);
		Tp.util.Notifier.error('Error', 'Please, reload this page.\n<hr/>\n' + x.get_message());
	},

	safe: function (array) {
		return !array ? [] : array;
	},

	getFilterTypes: function () {
		var types = [];
		types.push({
			id: 'x-kanban-show-userstories' + this.project.id,
			value: 'userstory',
			title: this.process.userStoryEntityType.titlePlural
		});

		if (this.process.bugTrackingPractice) {
			types.push({
				id: 'x-kanban-show-bugs' + this.project.id,
				value: 'bug',
				title: this.process.bugEntityType.titlePlural
			});
		};

		types.push({
			id: 'x-kanban-show-tasks' + this.project.id,
			value: 'task',
			title: this.process.taskEntityType.titlePlural,
			display: this.showTasksAsCards ? "" : "none"
		});

		return types;
	},

	getSortOptions: function () {
		var items = new Array();
		var nameItem = {
			id: 'x-kanban-sort-name' + this.project.id,
			value: 'name',
			title: 'Name'
		};
		var rankItem = {
			id: 'x-kanban-sort-rank' + this.project.id,
			value: 'rank',
			title: 'Rank'
		};
		var typeItem = {
			id: 'x-kanban-sort-type' + this.project.id,
			value: 'type',
			title: 'Type'
		};
		var effortItem = {
			id: 'x-kanban-sort-effort' + this.project.id,
			value: 'effort',
			title: 'Effort'
		};
		items.push(nameItem);
		items.push(rankItem);

		if (this.getFilterTypes().length > 1) {
			items.push(typeItem);
		}

		items.push(effortItem);
		return items;
	}
});
