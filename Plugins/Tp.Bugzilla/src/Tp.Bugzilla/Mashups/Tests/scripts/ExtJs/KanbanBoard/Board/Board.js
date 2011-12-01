Ext.ns('Tp.controls.kanbanboard.board');

/**
 * Displays swimlanes for entity states.
 */
Tp.controls.kanbanboard.board.Board = Ext.extend(Ext.BoxComponent, {

    
	/**
	 * Specify visual oppearance of the backlog element with a particular CSS class.
	 */
	cls: 'kanban-board-wrap',

	//private
	controller: null,

	//private
	uxswimlanes: [],

	uniqueID: null,

    recalculateHeight: function(newHeight){

        var finalSwimlaneID = this.controller.swimlanes.finalSwimlane ? this.controller.swimlanes.finalSwimlane.id : -1;
        
        Ext.each(this.uxswimlanes, function(swimlanePanel){
            if(swimlanePanel.swimlane.id == finalSwimlaneID){
                return;
            }
            var swimlaneHeight = newHeight - Ext.get('kanban-thead-' + this.uniqueID).getHeight() - 161;
            swimlanePanel.setHeight(swimlaneHeight);

        }, this);
    },

	constructor: function(config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}

		Tp.controls.kanbanboard.board.Board.superclass.constructor.call(this, config);
		this.uniqueID = config.controller.project.id;
	},

	_initTemplateNoPlannedState: function() {
		this.clearView();
		var tpl = new Ext.XTemplate(
				'<table class="kanban-swimlanes-table">',

                '<thead id="kanban-thead-{uniqueID}">',

                '<tr>',
				'<th colspan="{wipStatesCount}" class="kanban-swimlane-wip-header" >Work in Progress</th>',
				'</tr>',
				'<tr><tpl for="wipStates">',
				'<th id="kanban-swimlane-header-{id}-wrap{parent.uniqueID}" class="kanban-swimlane-header-wrap" width="{parent.width}"></th>',
				'</tpl></tr>',

                '</thead>',
                '<tbody>',

				'<tr><tpl for="states">',
				'<td id="kanban-swimlane-{id}-wrap{parent.uniqueID}" class="kanban-swimlane-wrap" width="{parent.width}" height="{parent.height}"></td>',
				'</tpl></tr>',
				'<tr>',
				'<th id="kanban-swimlane-header-final-wrap{uniqueID}" class="kanban-swimlane-header-wrap" colspan="{statesCount}"><span id="kanban-swimlane-header-final-wrap-states{uniqueID}"></span> (10 latest)</th>',
				'</tr>',
				'<tr>',
				'<td id="kanban-swimlane-final-wrap{uniqueID}" class="kanban-swimlane-final-wrap" colspan="{statesCount}"></td>',
				'</tr>',
                '</tbody>',
				'</table>');
		var wipStates = this.controller.swimlanes;
		var states = this.controller.swimlanes;
		this._bindTemplateData(tpl, wipStates, states);
	},


	_initTemplateAllStates: function() {
		this.clearView();
		var tpl = new Ext.XTemplate(
				'<table class="kanban-swimlanes-table">',
                '<thead id="kanban-thead-{uniqueID}">',
				'<tr>',
				'<th id="kanban-swimlane-header-{plannedId}-wrap{uniqueID}" class="kanban-swimlane-header-wrap" rowspan="2" width="{width}"></th>',
				'<th colspan="{wipStatesCount}" class="kanban-swimlane-wip-header" >Work in Progress</th>',
				'</tr>',
				'<tr><tpl for="wipStates">',
				'<th id="kanban-swimlane-header-{id}-wrap{parent.uniqueID}" class="kanban-swimlane-header-wrap" width="{parent.width}"></th>',
				'</tpl></tr>',
                '</thead>',
                '<tbody>',
				'<tr><tpl for="states">',
				'<td id="kanban-swimlane-{id}-wrap{parent.uniqueID}" class="kanban-swimlane-wrap" width="{parent.width}" height="{parent.height}"></td>',
				'</tpl></tr>',
				'<tr>',
				'<th id="kanban-swimlane-header-final-wrap{uniqueID}" class="kanban-swimlane-header-wrap" colspan="{statesCount}"><span id="kanban-swimlane-header-final-wrap-states{uniqueID}"></span> (10 latest)</th>',
				'</tr>',
				'<tr>',
				'<td id="kanban-swimlane-final-wrap{uniqueID}" class="kanban-swimlane-final-wrap" colspan="{statesCount}"></td>',
				'</tr>',
                '</tbody>',
				'</table>');
		var wipStates = this.controller.swimlanes;
		var states = [this.controller.swimlanes.plannedSwimlane].concat(this.controller.swimlanes);
		this._bindTemplateData(tpl, wipStates, states);
	},

	clearView: function() {

		//destroy previously created swimlanes.
		for (var n = 0; n < this.uxswimlanes.length; n++) {
			this.uxswimlanes[n].destroy();
		}
		this.uxswimlanes = [];

	},

	_bindTemplateData: function(tpl, wipStates, states) {
		tpl.overwrite(this.el, {
			uniqueID: this.uniqueID,
			initialId: this.controller.swimlanes.initialSwimlane.id,
			finalId: this.controller.swimlanes.finalSwimlane.id,
			plannedId: this.controller.swimlanes.plannedSwimlane.id,
			states: states,
			statesCount: states.length,
			wipStates: wipStates,
			wipStatesCount: wipStates.length,
			width: (100 / states.length) + '%',
			// IE treats height incorrect, so have to set value for swimlane manually
			height: Ext.isIE ? '70%' : '100%'
		});

		for (var n = 0; n < states.length; n++) {
			var swimlane = this.createSwimlane({
				renderTo: 'kanban-swimlane-' + states[n].id + '-wrap' + this.uniqueID,
				controller: this.controller,
				swimlane: states[n],
				uxHeader: new Tp.controls.kanbanboard.sections.SwimlaneHeader({
					renderTo: 'kanban-swimlane-header-' + states[n].id + '-wrap' + this.uniqueID,
					controller: this.controller,
					swimlane: states[n],
					title: String(states[n].title)
				})
			});
			this.uxswimlanes.push(swimlane);
			if (swimlane.items != undefined)
			{
				swimlane.highlightItemsCount();
			}
		}
		var swimlane = this.createFinalSwimlane({
			renderTo: 'kanban-swimlane-final-wrap' + this.uniqueID,
			controller: this.controller,
			swimlane: this.controller.swimlanes.finalSwimlane,
			uxHeader: new Tp.controls.kanbanboard.sections.SwimlaneHeader({
				renderTo: 'kanban-swimlane-header-final-wrap-states' + this.uniqueID,
				controller: this.controller,
				swimlane: this.controller.swimlanes.finalSwimlane,
				title:  String(this.controller.swimlanes.finalSwimlane.title)
			}),
			sortable: false
		});
		this.uxswimlanes.push(swimlane);
		if (swimlane.items != undefined) {
			swimlane.highlightItemsCount();
		}
		;
	},

	createSwimlane: function(config) {
		return new Tp.controls.kanbanboard.sections.ProjectSwimlane(config);
	},

	createFinalSwimlane: function (config) {
		return new Tp.controls.kanbanboard.sections.OrderByEndDateSwimlane(config);
	},

	refresh: function() {
		if (this.controller.swimlanes.hasPlannedSwimlane()) {
            this._initTemplateAllStates();
			return;
		}
        this._initTemplateNoPlannedState();
	},

	//private
	onRender: function(ct, position) {
		Tp.controls.kanbanboard.board.Board.superclass.onRender.call(this, ct, position);
		this.refresh();
	}

});

