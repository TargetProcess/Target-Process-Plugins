Ext.ns('Tp.controls.kanbanboard');
/**
 * KanbanboardManager contains list of kanban boards. It's a singleton class. 
 */
Tp.controls.kanbanboard.KanbanboardManager = Ext.extend(Ext.util.Observable, {

	kanbanBoards: [],

	constructor: function (config) {
		//this.addEvents("show", "hide");
		Tp.controls.kanbanboard.KanbanboardManager.superclass.constructor.call(this, config);
	},

	hasExpandedKanbanboard: function () {
		for (var i = 0; i < this.kanbanBoards.length; i++) {
			if (!this.kanbanBoards[i].collapsed) {
				return true;
			}
		}
		return false;
	},

	createKanbanboards: function (kanbanBoardDataList) {
		this.kanbanBoards = [];
		for (var i = 0; i < kanbanBoardDataList.length; i++) {
			var kanbanBoardData = kanbanBoardDataList[i];
			kanbanBoardData.kanbanBoardProcess = this.fixProcess(kanbanBoardData.kanbanBoardProcess);

			var config = {
				renderTo: 'kanban-board',
				controller: new Tp.controls.kanbanboard.Controller(this.clientId, kanbanBoardData.kanbanBoardScope,
				kanbanBoardData.kanbanBoardProcess,
				kanbanBoardData.kanbanBoardPreferences)
			};

			if (kanbanBoardDataList.length == 1) {
				config.collapsed = false;
			} else {
				config.collapsed = true;
			};

			var kanbanBoardPanel = new Tp.controls.kanbanboard.KanbanBoardPanel(config);
			this.kanbanBoards.push(kanbanBoardPanel);

			// force kanbanboard panel to recalculate layout after show/hide help panel action.
			var helpManager;
			try {
				helpManager = UseCaseHelp.HelpPanelManager.getInstance();
			} catch (e) { }
			if (helpManager) {
				helpManager.on("show", kanbanBoardPanel.doLayout, kanbanBoardPanel);
				helpManager.on("hide", kanbanBoardPanel.doLayout, kanbanBoardPanel);
			}

			if (i < kanbanBoardDataList.length - 1) {
				var footerDiv = new Ext.Element(document.createElement("div"));
				footerDiv.setHeight(0);

				Ext.get('kanban-board').appendChild(footerDiv);
			}
		}

		//set initial height for swimlanes
		for (var i = 0; i < this.kanbanBoards.length; i++) {
			this.kanbanBoards[i].recalculateHeight();
		}
	},

	/**
	* Updates process config flat object structure into interlinked object graph
	* where object identifiers are replaced with objects themselves.
	*
	* @param process Initial process rendered as a JSON object,
	* therefore does not contain methods and backreferences.
	*/
	fixProcess: function (process) {
		/**
		* Find type by its identifier.
		* @param id Type id.
		* @return {Tp.data.EntityType} Entity type with the specified id.
		*/
		process.getEntityType = function (id) {
			for (var n = 0; n < this.entityTypes.length; n++) {
				if (this.entityTypes[n].id == id) {
					return this.entityTypes[n];
				}
			}
			throw new Error('Entity type #' + id + ' not found');
		};

		/**
		* Find state by its identifier.
		* @param id State id.
		* @return {Tp.data.EntityState} Entity state with the specified id.
		*/
		process.getEntityState = function (id) {
			for (var n = 0; n < this.entityStates.length; n++) {
				if (this.entityStates[n].id == id) {
					return this.entityStates[n];
				}
			}
			throw new Error('Entity state #' + id + ' not found');
		};

		/**
		* Entity type object for releases.
		*/
		//process.releaseEntityType = process.getEntityType(2);
		/**
		* Entity type object for iterations.
		*/
		//process.iterationEntityType = process.getEntityType(3);
		/**
		* Entity type object for features.
		*/
		process.featureEntityType = process.getEntityType(9);
		/**
		* Entity type object for user stories.
		*/
		process.userStoryEntityType = process.getEntityType(4);
		/**
		* Entity type object for tasks.
		*/
		process.taskEntityType = process.getEntityType(5);
		/**
		* Entity type object for bugs.
		*/
		process.bugEntityType = process.getEntityType(8);
		/**
		* Entity type object for impediments.
		*/
		process.impedimentEntityType = process.getEntityType(16);

		// Replace identifiers with objects.
		for (var n = 0; n < process.entityStates.length; n++) {
			// step 1: replace entity type ids with entity type objects
			process.entityStates[n].entityType = process.getEntityType(process.entityStates[n].entityType);

			// step 2: replace next state ids with next state objects
			var nextStates = process.entityStates[n].nextStates || [];
			for (var m = 0; m < nextStates.length; m++) {
				nextStates[m] = process.getEntityState(nextStates[m]);
			}
			process.entityStates[n].nextStates = nextStates;

			/**
			* Verifies whether a specifies state can be reached from this state directly.
			*
			* @param nextState A state to be reached.
			*/
			process.entityStates[n].hasTransitionTo = function (nextState) {
				if (this.entityType == nextState.entityType) {
					return this == nextState || this.nextStates.indexOf(nextState) != -1;
				}
				return false;
			};
		}

		// Add entity states to each entity type.
		for (var n = 0; n < process.entityTypes.length; n++) {
			var entityType = process.entityTypes[n];
			entityType.entityStates = [];
			for (var m = 0; m < process.entityStates.length; m++) {
				var entityState = process.entityStates[m];
				if (entityState.entityType == entityType) {
					entityType.entityStates.push(entityState);
				}
			}
		}
		return process;
	},

	updateKanban: function (releaseIterationFilter) {
		Ext.each(this.kanbanBoards, function (board, index) {
			var projectFilter = Array.findOne(releaseIterationFilter, function (filterEntry) {
				return board.controller.project.id == filterEntry.projectId;
			});

			var releaseIds = !projectFilter ? [] : projectFilter.releaseIds;
			var iterationIds = !projectFilter ? [] : projectFilter.iterationIds;

			board.controller.project.releaseIds = releaseIds;
			board.controller.project.iterationIds = iterationIds;
			board.tryFireReloadEvent();
		});
	}
});

Tp.controls.kanbanboard.KanbanboardManager.getInstance = function () {
	if (Tp.controls.kanbanboard.KanbanboardManager.instance == null)
		Tp.controls.kanbanboard.KanbanboardManager.instance = new Tp.controls.kanbanboard.KanbanboardManager();
	return Tp.controls.kanbanboard.KanbanboardManager.instance;
}