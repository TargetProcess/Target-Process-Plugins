Ext.ns('Tp.controls.kanbanboard.sections');

/**
* Kanban board consists of several vertical swimlanes.
* Each swimlane displays items such as <em>Tasks</em> and <em>Bugs</em> in a particular state,
* such as <em>Open</em>, <em>In Progress</em>, etc.
* It also allows dragging items onto other swimlanes.
* See {@link Tp.controls.kanbanboard.sections.Backlog} for more details.
*/
Tp.controls.kanbanboard.sections.Swimlane = Ext.extend(Tp.controls.kanbanboard.Container, {
	/**
	* Specify visual oppearance of the swimlane element with a particular CSS class.
	*/
	cls: 'kanban-swimlane',

	//private
	controller: null,

	//private
	swimlane: null,

	initTitle: null,

	constructor: function (config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}
		if (!config.swimlane) {
			throw new Error('Swimlane not specified');
		}

		Tp.controls.kanbanboard.sections.Swimlane.superclass.constructor.call(this, config);

		this.controller.addSwimlane(this);

		this.addEvents('dropaccept');

		this.on('add', this.onAdd, this);

		this.on('remove', this.onRemove, this);
	},

	highlighDisabled: function (item, entity) {
		if (entity == null) {
			this.removeClass('kanban-swimlane-disabled');
		}
		else {
			if (!this.allowDrop(item, entity)) {
				this.addClass('kanban-swimlane-disabled');
			}
		}
	},

	/**
	* Apply appropriate visual style to this container if the number of contained items is over limit.
	*/
	highlightItemsCount: function () {
		if (this.uxHeader == null) {
			return;
		}
		var uxHeaderFrameEl = this.uxHeader.getEl().parent();

		if (this.swimlane.limit > 0 && this.items.length > this.swimlane.limit) {
			uxHeaderFrameEl.addClass('kanban-swimlane-overlimit');
			uxHeaderFrameEl.removeClass('kanban-swimlane-limitless');
		} else if (this.swimlane.limit > 0 && this.items.length < this.swimlane.limit) {
			uxHeaderFrameEl.addClass('kanban-swimlane-limitless');
			uxHeaderFrameEl.removeClass('kanban-swimlane-overlimit');
		} else {
			uxHeaderFrameEl.removeClass('kanban-swimlane-limitless');
			uxHeaderFrameEl.removeClass('kanban-swimlane-overlimit');
		};

		this.updateHeader();
	},

	updateHeader: function () {
		if (this.uxHeader == null) {
			return;
		};
		var title = this.swimlane.title;
		if (this.swimlane.limit > 0) {
			title = this.swimlane.initTitle + ' - ' + this.items.length + ' (limit ' + this.swimlane.limit + ')';
		} else if (this.items.length > 0 && this.swimlane.name != '__final__') {
			title = this.swimlane.initTitle + ' - ' + this.items.length;
		}
		this.uxHeader.el.update(title);
	},

	//private
	allowDrop: function (item, entity) {
		return this.swimlane.acceptEntity(entity);
	},

	//private
	onDrop: function (dd, e, data, comment, prev, next) {
		// change state of the dropped entity to the state of this swimlane
		this.fireEvent('dropaccept', dd, e, data, comment, this, prev, next);
		this.highlightItemsCount();
	},

	onAdd: function (ct, item) {
		//we do not need to call dolayout or repaint because sorting happening just in time
		this.sort();
		this.highlightItemsCount();
	},

	onRemove: function (ct, item) {
		this.highlightItemsCount();
	},

	isCommentRequired: function (data) {
		return this.swimlane.findEntityState(data.entity).requiredComment;
	}
});
