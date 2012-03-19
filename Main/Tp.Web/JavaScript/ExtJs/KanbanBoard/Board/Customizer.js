Ext.ns('Tp.controls.kanbanboard.board');

/*
* Board customizer dialog it allows reordering swimlanes.
*/
Tp.controls.kanbanboard.board.Customizer = Ext.extend(Ext.Panel, {
	controller: null,
	constructor: function (config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}

		this.order = new Tp.controls.kanbanboard.board.Order({ controller: config.controller });
		this.limits = new Tp.controls.kanbanboard.board.Limits({ controller: config.controller });
		this.reFresh = new Tp.controls.kanbanboard.board.ReFresh({ controller: config.controller });
		this.showTasks = new Tp.controls.kanbanboard.board.ShowTasks({ controller: config.controller });

		var items = [{
			autoHeight: true,
			title: 'Order',
			items: [{
				border: false,
				style: {
					'padding': '10px 10px 0 10px'
				},
				html: 'Use drag and drop to define columns order.'
			},
				this.order
			],
			listeners: {
				activate: this.resizeOwnerPanels
			}
		}, {
			autoHeight: true,
			title: 'Limits',
			items: [{
				border: false,
				style: {
					'padding': '10px 10px 0 10px'
				},
				html: 'Set limits on states. Limit shows how many items may be in a particular state.'
			},
				this.limits
			],
			listeners: {
				activate: this.resizeOwnerPanels
			}
		}, {
			autoHeight: true,
			title: 'Refresh',
			items: [{
				border: false,
				style: {
					'padding': '10px 10px 0 10px'
				},
				html: 'Set refresh interval for Kanban board.'
			},
				this.reFresh
			],
			listeners: {
				activate: this.resizeOwnerPanels
			}
		}
		];

		if (config.controller.preferences.showTasksAsCardsAvailable)
			items.push({
				autoHeight: true,
				title: 'Tasks',
				items: [
					this.showTasks
				],
				listeners: {
					activate: this.resizeOwnerPanels
				}
			});

		var configValues = {
			//width: 400,
			//height: 300,
			autoHeight: true,
			border: false,
			items: new Ext.TabPanel({
				autoTabs: true,
				activeTab: 0,
				deferredRender: false,
				border: false,
				items: items
			})
		};



		Ext.apply(config, configValues);

		Tp.controls.kanbanboard.board.Customizer.superclass.constructor.call(this, config);
		this.on('afterlayout', this.onAfterLayout, this);
	},

	onAfterLayout: function () {
		this.autoHeight = false;
		this.setHeight(this.getHeight());
	},

	/**
	* Reorder swimlanes in the controller according to the user preferences.
	*/
	applyCustomization: function () {
		this.order.applyCustomization();
		this.limits.applyCustomization();
		this.reFresh.applyCustomization();
		this.showTasks.applyCustomization();
	},

	resizeOwnerPanels: function (panel) {
		panel.ownerCt.ownerCt.setHeight(panel.ownerCt.getHeight());
		panel.ownerCt.ownerCt.ownerCt.setHeight(panel.ownerCt.getHeight());
	}
});
