Ext.ns('Tp.controls.kanbanboard.board');

/*
* Board customizer dialog it allows reordering swimlanes.
*/
Tp.controls.kanbanboard.board.ShowTasks = Ext.extend(Ext.Panel, {
	controller: null,
	constructor: function (config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}

		Ext.apply(config, {
			border: false,
			defaults: {
				//bodyStyle: 'padding:10px'
			},
			items: [
				{
					xtype: 'radiogroup',
					columns: 1,
					vertical: true,
					defaultType: 'radio',
					style: {
						'padding': '20px 10px 0 20px'
					},
					items: [
						{
							name: 'show_tasks',
							inputValue: 'asCards',
							boxLabel: 'Show as cards',
							checked: !!config.controller.showTasksAsCards
						},
						{
							name: 'show_tasks',
							inputValue: 'asCount',
							boxLabel: 'No cards, just the count of tasks',
							checked: !config.controller.showTasksAsCards
						}]
				}]
		});

		Tp.controls.kanbanboard.board.ShowTasks.superclass.constructor.call(this, config);

	},

	/*
	* Reorder swimlanes in the controller according to the user preferences.
	*/
	applyCustomization: function () {
		this.controller.showTasksAsCards = this.findByType('radiogroup')[0].getValue().inputValue == 'asCards';
	}
});