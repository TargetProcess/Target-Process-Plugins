Ext.ns('Tp.controls.kanbanboard');
/**
* Top level kanban board widget, it contains child widgets such as
* {@link Tp.controls.kanbanboard.sections.Backlog backlog},
* {@link Tp.controls.kanbanboard.board.Board board}, etc.
*/
Tp.controls.kanbanboard.KanbanBoardPanel = Ext.extend(Ext.Panel, {
	board: null,
	backlogFilter: null, // backlog filter control
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
			this._isReloadPossible = false;
			this.fireEvent("reload");
		}
	},

	allowFireReloadEvent: function () {
		this._isReloadPossible = true;
	},

	controller: null,
	customizing: false,
	collapsible: true,
	headerStyle: 'font-weight: bold; background: #fff;',
	defaultHeight: 700,
	_maximized: false,
	backlog: null,

	getMaximized: function () {
		return this._maximized;
	},

	constructor: function (config) {
		this.addEvents({ "reload": true, "maximized": true, "restored": true, "loadItemsSuccess": true, "loadItemsFailed": true });
		this._initControls(config);
		this._applyConfig(config);

		this.title = config.controller.project.name;

		Tp.controls.kanbanboard.KanbanBoardPanel.superclass.constructor.call(this, config);

		this.backlog.setRegion(this.layout.west);

		if (config.collapsed != null && !config.collapsed && this.collapsed) {
			this.expand();
		}

		this.controller.setKanbanBoardPanel(this);

		this.on('expand', this.onExpanded, this);
		this.on('loadItemsSuccess', this.onloadItemsSuccess, this);
		this.on('loadItemsFailed', this.onloadItemsFailed, this);
	},

	collapseExpandHandler: function () {
		this.toggleCollapse();
		this.doLayout();
	},

	disableToggle: function () {
		this.tools.toggle.hide();
		this.getToggleButton().un('click', this.collapseExpandHandler, this);
	},

	getToggleButton: function () {
		var a = Ext.query('a.x-panel-kanban-header-link', this.el.dom)[0];
		return Ext.get(a);
	},

	enableToggle: function () {
		this.tools.toggle.show();
		this.getToggleButton().on('click', this.collapseExpandHandler, this);
	},

	doRestore: function (panel) {
		this.showAllBoards();

		panel.tools.maximize.show();
		panel.tools.restore.hide();
		this.removeClass("fullScreen");

		this._maximized = false;
		this.enableToggle();

		this.recalculateHeight();
	},

	doMaximize: function (panel) {
		this.hideOtherBoards();
		panel.tools.maximize.hide();
		panel.tools.restore.show();
		this.addClass("fullScreen");

		this._maximized = true;
		this.disableToggle();
		this.recalculateHeight();
	},

	_doRecalculateHeight: function (kanbanHeight) {
		this.setHeight(kanbanHeight);
		this.fixHeightForBacklog(kanbanHeight);
		this.board.recalculateHeight(kanbanHeight);
		this.doLayout();
	},

	recalculateHeight: function () {
		var height = this.defaultHeight;
		if (this.getMaximized()) {
			height = Tp.util.getClientHeight();
		}
		this._doRecalculateHeight(height);
	},

	fixHeightForBacklog: function (kanbanHeight) {
		var backlogHeight = kanbanHeight - 50 - this.backlogFilter.getHeight();
		this.backlog.setHeight(backlogHeight);
	},

	onWindowResize: function () {
		if (this.getMaximized()) {
			this.recalculateHeight();
		} else {
			this.doLayout();
		}
	},

	onloadItemsSuccess: function () {
		this._itemsLoaded = true;
		this.allowFireReloadEvent();
	},

	onloadItemsFailed: function () {
		this.allowFireReloadEvent();
	},

	onRender: function (ct, position) {
		Tp.controls.kanbanboard.KanbanBoardPanel.superclass.onRender.call(this, ct, position);

		var a = Ext.query('a.x-panel-kanban-header-link', this.el.dom)[0];
		Ext.fly(a.parentNode).insertFirst({ tag: 'div', cls: 'x-panel-kanban-header' }).appendChild(a);
		this.getToggleButton().on('click', this.collapseExpandHandler, this);
	},

	afterRender: function () {
		Tp.controls.kanbanboard.KanbanBoardPanel.superclass.afterRender.call(this);
		Ext.EventManager.onWindowResize(this.onWindowResize, this);
	},

	onDestroy: function () {
		Tp.controls.kanbanboard.KanbanBoardPanel.superclass.onDestroy.call(this);
		Ext.EventManager.removeResizeListener(this.onWindowResize, this);
	},

	onExpanded: function () {
		this.recalculateHeight();
		if (!this._itemsLoaded) {
			this.tryFireReloadEvent();
		}
	},

	_initControls: function (config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}

		var filter = this.makeDefaultFilter(config.controller.project);
		this.board = new Tp.controls.kanbanboard.board.Board({
			controller: config.controller
		});

		this.backlogFilter = new Tp.controls.kanbanboard.sections.FilterPanel({
			controller: config.controller,
			filter: filter
		});

		this.backlog = new Tp.controls.kanbanboard.sections.Backlog({
			controller: config.controller,
			filter: filter
		});
	},

	_applyConfig: function (config) {
		Ext.apply(config, {
			height: this.defaultHeight,
			layout: 'border',
			border: false,
			maximizable: true,
			headerCfg: {
				tag: 'a',
				cls: 'x-panel-kanban-header-link',
				href: 'javascript:void(0)'
			},
			stateId: 'KanbanBoard_' + config.controller.project.id,

			stateEvents: ["collapse", "expand"],
			getState: function () {
				return { collapsed: this.collapsed };
			},
			items: [
                {
                	floatable: false,
                	region: 'west',
                	title: 'Backlog',
                	stateId: 'Backlog_' + config.controller.project.id,

                	stateEvents: ["collapse", "expand"],
                	getState: function () {
                		return { collapsed: this.collapsed };
                	},

                	items: [
                        this.backlogFilter,
                        this.backlog
                    ],
                	collapsible: true,
                	collapseFirst: false,
                	animCollapse: true,
                	width: 260,
                	split: true,
                	minSize: 180,
                	maxSize: 380,
                	tools: this._backlogTools()
                },
                {
                	region: 'center',
                	title: 'Kanban board',
                	items: this.board,
                	layout: 'fit',
                	tools: this.mainBoardTools()
                }
            ]
		});
	},

	hideOtherBoards: function () {
		var manager = Tp.controls.kanbanboard.KanbanboardManager.getInstance();
		Ext.each(manager.kanbanBoards, function (kanbanBoard) {
			if (kanbanBoard.id != this.id) {
				kanbanBoard.hide();
			}
		}, this);
	},

	showAllBoards: function () {
		var manager = Tp.controls.kanbanboard.KanbanboardManager.getInstance();
		Ext.each(manager.kanbanBoards, function (kanbanBoard) {
			kanbanBoard.show();
		}, this);
	},

	mainBoardTools: function () {
		var mainBoardTools = [];
		if (Application.user.canEditProject) {
			mainBoardTools.push({
				id: 'gear',
				qtip: 'Customize...',
				handler: function (event, toolEl, panel) {
					this.beginCustomize();
				},
				scope: this
			});
		}

		mainBoardTools.push({
			id: 'refresh',
			qtip: 'Refresh...',
			handler: function (event, toolEl, panel) {
				this.tryFireReloadEvent();
			},
			scope: this
		});

		mainBoardTools.push({
			id: 'maximize',
			qtip: 'Maximize',
			handler: function (event, toolEl, panel) {
				this.doMaximize(panel);
				this.fireEvent("maximized");
			},
			scope: this
		});

		mainBoardTools.push({
			id: 'restore',
			qtip: 'Restore',
			handler: function (event, toolEl, panel) {
				this.doRestore(panel);
				this.fireEvent("restored");
			},
			hidden: true,
			scope: this
		});

		return mainBoardTools;
	},

	_backlogTools: function () {
		var backlogTools = [];
		backlogTools.push(
        {
        	id: 'search',

        	qtip: 'Filter...',

        	handler: function (event, toolEl, panel) {
        		if (this.backlogFilter.hidden) {
        			this.backlogFilter.show();
        			this.backlog.getEl().removeClass('kanban-backlog');
        			this.backlog.getEl().addClass('kanban-backlog-filter-on');
        			this.backlog.fireEvent("filterShown");
        		}
        		else {
        			this.backlogFilter.hide();
        			this.backlog.getEl().removeClass('kanban-backlog-filter-on');
        			this.backlog.getEl().addClass('kanban-backlog');
        			this.backlog.fireEvent("filterHidden");
        		}
        	},
        	scope: this
        });

		return backlogTools;
	},

	beginCustomize: function () {
		if (this.customizing === false) {
			this.customizing = true;
			this.disable();

			var customizer = new Tp.controls.kanbanboard.board.Customizer({ controller: this.controller });

			var window = new Ext.Window({
				title: 'Customize',
				layout: 'fit',
				width: 400,
				autoHeight: true,
				resizable: false,
				closeAction: 'close', // Destroy window and child controls on close, remove DOM elements.
				items: customizer,
				buttons: [
                    {
                    	text: 'Apply',
                    	handler: function () {
                    		this.customizing = false;

                    		// First, apply customization while window is open and elements still present.
                    		customizer.applyCustomization();
                    		// Only then close window.
                    		window.close();
                    		// Then save preferences silently.
                    		this.controller.savePreferences();
                    	},
                    	scope: this
                    }
                ]
			});

			window.on('close', function () {
				this.customizing = false;
				this.enable();
			}, this);

			window.show();
			window.center();
		}
	},

	disable: function () {
		this.controller.disable();
		this.backlog.disable();
		this.board.disable();
	},

	enable: function () {
		this.controller.enable();
		this.backlog.enable();
		this.board.enable();
		this.board.refresh();
		this.onWindowResize();
	},

	makeDefaultFilter: function (project) {
		var Filter = Ext.extend(Ext.util.Observable, {
			/**
			* Filter by keyword.
			*/
			keywords: [],
			/**
			* Filter by tag.
			*/
			tags: [],
			/**
			* Filter by entity type.
			*/
			type: {
				'feature': true,
				'userstory': true,
				'bug': true
			},
			/**
			* Sort by value.
			*/
			sort: 'rank',
			/**
			* Constructor.
			*/
			constructor: function (config) {
				Ext.apply(this, config);
				Filter.superclass.constructor.call(this);
				this.addEvents('changed');
			},
			/**
			* Fire changed filter event.
			*/
			notifyChanged: function () {
				this.fireEvent('changed', this);
			}
		});
		return new Filter();
	}
});
