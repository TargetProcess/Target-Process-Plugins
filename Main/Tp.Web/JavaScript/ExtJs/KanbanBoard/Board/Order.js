Ext.ns('Tp.controls.kanbanboard.board');

/*
* Renders as a list of columns and allow reordering them with help of drag and drop.
*/
Tp.controls.kanbanboard.board.Order = Ext.extend(Ext.BoxComponent, {

	/*
	* Specify visual oppearance of the customizer element with a particular CSS class.
	*/
	cls: 'kanban-board-customizer-order',
	controller: null,
	sortable: null,

	constructor: function (config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}
		Tp.controls.kanbanboard.board.Order.superclass.constructor.call(this, config);
	},

	/*
	* Get keys assigned to items reordered after drag and drop.
	*/
	getOrderedKeys: function () {
		var keys = [];
		if (this.sortable != null) {
			var items = this.sortable.serialize();
			for (var n = 0; n < items.length; n++) {
				keys.push(items[n].xkey);
			}
		}
		return keys;
	},

	//private
	onRender: function (ct, position) {
		Tp.controls.kanbanboard.board.Order.superclass.onRender.call(this, ct, position);
		var tpl;
		if (this.controller.swimlanes.length == 0) {
			tpl = new Ext.XTemplate(
				'<table width="100%" height="200"><tr><td style="text-align: center;">No states available. You should add states in Admin - Process - Workflow.</td></tr></table>');
			tpl.overwrite(this.el, {});
		}
		else {
			tpl = new Ext.XTemplate(
				'<ul class="kanban-swimlanes-sorter" id="kanban-swimlanes-sorter-{id}">',
				'<tpl for="swimlanes">',
				'<li id="kanban-swimlanes-sorter-item-{parent.id}-{id}" class="kanban-swimlanes-sorter-item" xkey="{id}">{title}</li>',
				'</tpl>',
				'</ul>');
			tpl.overwrite(this.el, { id: this.id, swimlanes: this.controller.swimlanes });

			this.sortable = new Ext.ux.Sortable({
				container: 'kanban-swimlanes-sorter-' + this.id,
				selector: 'li.kanban-swimlanes-sorter-item'
			});

			// Assign keys to draggable items. Keys are unique identifiers of swimlanes.
			var items = this.sortable.serialize();
			for (var n = 0; n < this.controller.swimlanes.length; n++) {
				items[n].xkey = this.controller.swimlanes[n].id;
			}

			this.sortable.on("sorted", function () {
				//alert('keys = ' + this.getOrderedKeys().join(', '));
			}, this);
		}
	},

	//private
	afterRender: function () {
		Tp.controls.kanbanboard.board.Order.superclass.afterRender.call(this);

		this.el.unselectable();
	},

	/*
	* Reorder swimlanes in the controller according to the user preferences.
	*/
	applyCustomization: function () {
		// get reordered keys
		var keys = this.getOrderedKeys();
		// remove swimlanes from array
		while (this.controller.swimlanes.length > 0) {
			this.controller.swimlanes.pop();
		}
		// add swimlanes to array according to keys order
		for (var n = 0; n < keys.length; n++) {
			this.controller.swimlanes.push(this.controller.swimlanes[keys[n]]);
		}
	}
});