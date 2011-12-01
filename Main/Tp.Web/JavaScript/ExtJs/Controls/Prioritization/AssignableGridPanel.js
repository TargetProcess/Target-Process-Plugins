Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.ViewPlugin = Ext.extend(Object, {
	header: "",
	width: 35,
	sortable: false,
	fixed: true,
	menuDisabled: true,
	dataIndex: '',
	id: 'view',
	moveable: false,
	grid: null,
	mouseEntered: false,

	init: function (grid) {
		this.grid = grid;
		this.grid.on("mouseover", this.onMouseOver, this);
		this.grid.on("mouseout", this.onMouseOut, this);
		this.grid.on("cellclick", this.onCellClick, this);
		this.grid.on("rowdblclick", this.showEntity, this);
		this._handleMouseHovering = this._handleMouseHoveringWhenMouseOut;
	},

	showEntity: function (grid, index) {

		var reloadBoard = function() {
			grid.store.reload();
		};

		var data = grid.store.getAt(index).data;

		var entityId = data.id;
		var entityTypeName = data.entityIcon;
		require(["tau/integration"], function (integrationModule) {
			integrationModule.show(entityId, entityTypeName, reloadBoard);
		});
	},

	renderer: function (v, p, record) {
		return "<a class='tau-target-link' style='visibility:hidden; vertical-align: middle; text-decoration: none;' href='javascript:void(0)'>View</a>";
	},

	onMouseOver: function (eventObj) {
		var row = eventObj.getTarget('.x-grid3-row');
		this._handleMouseHovering(row);
		this._handleMouseHovering = this._handleMouseHoveringWhenMouseOut;
	},

	onMouseOut: function (eventObj) {
		var row = eventObj.getTarget('.x-grid3-row');
		this._handleMouseHovering(row);
		this._handleMouseHovering = this._handleMouseHoveringWhenMouseEntered;
	},

	_handleMouseHoveringWhenMouseOut: function (row) {
		if (row == null) {
			return;
		}
		var anchors = Ext.fly(row).select('a[class=tau-target-link]');
		anchors.each(function (anchor) {
			anchor.setVisible(false);
		});
	},

	onCellClick: function (sender, rowIndex, columnIndex, e) {
		if (!Ext.fly(e.target).hasClass('tau-target-link'))
			return;

		this.showEntity(this.grid, rowIndex);
	},

	_handleMouseHoveringWhenMouseEntered: function (row) {
		if (row == null) {
			return;
		}

		var anchors = Ext.fly(row).select('a[class=tau-target-link]');
		anchors.each(function (anchor) {
			anchor.setVisible(true);
		});
	}
});

Tp.controls.Prioritization.AssignableGridPanel = Ext.extend(Ext.grid.GridPanel, {
	createView: function() {
		return new Ext.ux.grid.BufferView({ forceFit: true, emptyText: 'No items found.' });
	},
	constructor: function(config) {
		var moveRankUpColumn = new Tp.controls.Prioritization.MoveRankUpColumnPlugin();
		var viewColumn = new Tp.controls.Prioritization.ViewPlugin();

		config = Ext.apply({
			view: this.createView(),
			stateful: true,
			stateId: "prioritizeGrid",
			cls: 'x-grid-dragableRow',
			store: new Tp.controls.Prioritization.AssignableJsonStore({
				proxy: this._getProxy(config.filter.proxy)
			}),
			columns: [
                {
                	dataIndex: 'entityIcon',
                	header: 'Type',
                	renderer: ExtJs.tp.util.Format.EntityType
                },
                {
                	dataIndex: 'id',
                	header: 'ID',
                	renderer: ExtJs.tp.util.Format.AssignableIDLink
                },
                {
                	dataIndex:'entityName',
                	header: 'Name',
                	renderer: ExtJs.tp.util.Format.ProjectAbbr,
                	width: 400
                },
                {
                	dataIndex: 'entityState',
                	header: 'State',
                	renderer: ExtJs.tp.util.Format.HtmlEncode
                },
                {
                	dataIndex: 'effort',
                	header: 'Effort, h'
                },
                {
                	dataIndex: 'priority',
                	header: 'BV',
                	renderer: ExtJs.tp.util.Format.HtmlEncode
                },
                {
                	dataIndex: 'tags',
                	header: 'Tags',
                	renderer: ExtJs.tp.util.Format.HtmlEncode,
                	width: 200
                },
				moveRankUpColumn
            ],
			sm: new Ext.grid.RowSelectionModel({ singleSelect: true }),
			enableHdMenu: false,
			plugins: [
                new Tp.controls.grid.plugins.GridDragDropRowOrder({ copy: false, scrollable: true, targetCfg: {} }),
                new Tp.controls.grid.plugins.PrioritizePlugin({ prioritizationService: Tp.Web.PageServices.AssignableService }),
				viewColumn,
                moveRankUpColumn
            ]
		}, config);

		if (config.filter != null) {
			config.filter.on("filterChanged", this._onFilterChanged, this);
		}

		Tp.controls.Prioritization.AssignableGridPanel.superclass.constructor.call(this, config);
	},

	_onFilterChanged: function(filterOptions) {
		this.store.load({
			params: { filterJson: Ext.encode(filterOptions) }
		});
	},

	_getProxy: function(proxyProvided) {
		if (proxyProvided != null)
			return proxyProvided;

        //for tests only
		//return new Ext.data.HttpProxy({ url: Application.baseUrl + '/Project/Planning/Iteration/ExtJsPrioritize.aspx?ProjectID=1' });
	},

	onRender: function() {
		Tp.controls.Prioritization.AssignableGridPanel.superclass.onRender.apply(this, arguments);
		this.getStore().load();
	}
});

Ext.reg('tpassignablegridpanel', Tp.controls.Prioritization.AssignableGridPanel);