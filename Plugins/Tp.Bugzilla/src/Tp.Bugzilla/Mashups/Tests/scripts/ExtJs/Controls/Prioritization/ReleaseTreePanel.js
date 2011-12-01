Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.ReleaseTreePanel = Ext.extend(Ext.tree.TreePanel, {
	savedFilterState: null,
	projectId: null,
	projectName: null,

	constructor: function (config) {
		this.projectId = config.data.ProjectId;
		this.projectName = config.data.ProjectName;
		Ext.apply(config,
		{
			forecolor: '#FF0000',
			//height: 245,
			//width: '196px',
			autoScroll: false,
			animate: false,
			enableDD: false,
			containerScroll: false,
			rootVisible: false,
			border: false,
			lines: false,
			selModel: new Ext.tree.MultiSelectionModel({}),
			collapsible: false,
			title: config.data.ProjectName,
			headerStyle: { color: '#333' },
			header: true,
			listeners: {
				afterrender: function (treepanel) {
					this.expandAll();
					this.restoreFilterValues(this.savedFilterState);
					var sm = this.getSelectionModel();

					Array.forEach(this.getChecked(), function (item) {
						item.getUI().toggleCheck(true);
					}, this);
					this.fireEvent('releaseTreeLoaded', null);
				},
				click: function (node, ev) {
					node.getUI().toggleCheck(!node.attributes.checked);
					return false;
				},
				checkchange: function (node, checked) {
					var sm = this.getSelectionModel();
					checked ? sm.select(node, null, true) : sm.unselect(node);
				},
				beforecollapsenode: function (node, deep, anim) {
					return false;
				},
				beforedblclick: function (node, ev) {
					return false;
				},
				beforechildrenrendered: function (node) {
					node.setText(Ext.util.Format.htmlEncode(node.text));
				},
				scope: this
			},
			root: new Ext.tree.AsyncTreeNode({
				draggable: false,
				id: 'plannables',
				children: config.data.Releases
			})
		});
		Tp.controls.Prioritization.ReleaseTreePanel.superclass.constructor.call(this, config);
	},

	restoreFilterValues: function (filter) {
		if (!filter)
			return;

		if (this.savedFilterState == null) {
			this.savedFilterState = filter;
			return
		}
		filter = this.savedFilterState;

		this.restoreValuesFrom(filter);
	},

	setFilterDefaultValues: function () {
		Array.forEach(this.getChecked(), function (item) {
			item.getUI().toggleCheck(false);
		}, this);

		this.checkDefaultItems();
	},

	checkDefaultItems: function () {
		var sm = this.getSelectionModel();
		var backLogNode = this.root.findChild('Backlog');
		backLogNode.getUI().toggleCheck(true);
		sm.select(backLogNode, null, true);
	},

	restoreValuesFrom: function (filter) {
		Array.forEach(this.getChecked(), function (item) {
			item.getUI().toggleCheck(false);
		}, this);
		var sm = this.getSelectionModel();

		var backLogNode = this.root.findChild('Backlog');
		if (backLogNode != null) {
			backLogNode.getUI().toggleCheck(false);
			sm.unselect(backLogNode, null, true);
		}

		Array.forEach(filter.releaseIds, function (releaseId) {
			var node = this.getRootNode().findChild('id', releaseId);
			if (!Ext.isEmpty(node) && !node.getUI().isChecked()) {
				node.getUI().toggleCheck(true);
				sm.select(node, null, true);
			}
		}, this);

		Array.forEach(filter.iterationIds, function (iterationId) {
			var node = this.findIterationNode(this.getRootNode(), iterationId);
			if (!Ext.isEmpty(node) && !node.getUI().isChecked()) {
				node.getUI().toggleCheck(true);
				sm.select(node, null, true);
			}
		}, this);
	},

	findIterationNode: function (rootNode, iterationId) {
		var foundIterationNode;
		rootNode.eachChild(function (node) {
			var iterationNode = node.findChild('id', iterationId);
			if (iterationNode != null)
				foundIterationNode = iterationNode;
		}, this);

		return foundIterationNode;
	}
});