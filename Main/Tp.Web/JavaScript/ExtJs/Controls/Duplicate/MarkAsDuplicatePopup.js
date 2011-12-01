Ext.ns('Tp.controls.Duplicate');

Tp.controls.Duplicate.CreatePopup = function(projectIds, assignableId, btnTrigger, config) {
	return new Tp.controls.Duplicate.MarkAsDuplicatePopup(Ext.apply({
			projectIds: projectIds,
			assignableId: assignableId,
			renderTo: Ext.getBody(),
			btnTrigger: btnTrigger
		}, config || { }));
};

Tp.controls.Duplicate.ShowMarkAsDuplicatePopup = function (projectIds, assignableId, btnTrigger, config) {
	var duplicatePopup = Tp.controls.Duplicate.CreatePopup(projectIds, assignableId, btnTrigger, config);
	duplicatePopup.show(projectIds, assignableId);
};



Tp.controls.Duplicate.MarkAsDuplicatePopup = Ext.extend(Ext.BoxComponent, {
	_projectIds: null,
	_assignableId: null,
	_btnTrigger: null,
	constructor: function (config) {
		this._projectIds = config.projectIds;
		this._assignableId = config.assignableId;
		this._btnTrigger = config.btnTrigger;
		var config = Ext.apply({
			hideMode: 'display',
			style: {
				position: 'fixed',
				top: '50%',
				left: '50%'//,
				//zIndex: Tp.controls.getOverlay().getZIndex() + 1
			},
			listeners: {
				beforeshow: {
					fn: function () { Tp.controls.getOverlay().show(); },
					scope: this
				},
				beforehide: {
					fn: function () { Tp.controls.getOverlay().hide(); },
					scope: this
				}
			}
		}, config);

		Tp.controls.Duplicate.MarkAsDuplicatePopup.superclass.constructor.call(this, config);
	},

	initComponent: function () {
		Tp.controls.Duplicate.MarkAsDuplicatePopup.superclass.initComponent.call(this);

		var ts = this.templates || {};
		if (!ts.tplPopup) {
			ts.tplPopup = new Ext.XTemplate(
                 '<div class="mark-as-duplicate">',
                    '<div class="titleBlock">',
                        '<div class="titleBlockInner">',
                            '<h1>Find Duplicate</h1>',
                        '</div>',
                    '</div>',
                    '<table class="bugsTable" cellpading="0" cellspacing="0">',
                        '<tr>',
                            '<td class="left-ct"></td>',
                            '<td class="right-ct"></td>',
                        '</tr>',
                    '</table>',
                    '<div class="x-row-editor"></div>',
                 '</div>'
			);
		}
		this.templates = ts;
	},

	show: function (projectId, bugId) {
		this['leftBugView'].show(bugId);
		Tp.controls.Duplicate.MarkAsDuplicatePopup.superclass.show.call(this);
	},

	hide: function () {
		this.btnPanel.hide();
		this.hideToolbars();
		this.rightBugView.getView().hide();
		this.searchPanel.show();
		this.searchPanel.un('lookupactionlinkclick', this.onSelectDuplicate, this);
		Tp.controls.Duplicate.MarkAsDuplicatePopup.superclass.hide.call(this);
	},

	onRender: function (ct, position) {
		Tp.controls.Duplicate.MarkAsDuplicatePopup.superclass.onRender.call(this, ct, position);

		this.el.update(this.templates.tplPopup.apply(this));

		Ext.create({
			xtype: 'box',
			renderTo: Ext.query('div.titleBlockInner', this.el.dom)[0],
			autoEl: {
				tag: 'a',
				cls: 'close-popup-btn',
				href: 'javascript:void(0)'
			}
		}).el.on('click', this.hide, this);

		this.leftBugView = Ext.create({
			xtype: 'duplicatebugview',
			applyTo: Ext.query('.left-ct', this.el.dom)[0]
		});

		this.rightBugView = Ext.create({
			xtype: 'duplicatebugview',
			applyTo: Ext.query('.right-ct', this.el.dom)[0]
		});
		this.initBugViews([this.leftBugView, this.rightBugView]);

		this.goBackSearch = Ext.create({
			xtype: 'button',
			//hidden: true,
			hideMode: 'display',
			cls: 'back-to-search',
			renderTo: Ext.query('td[class=right-ct] div[class*=bug-view]', this.el.dom)[0],
			text: '',
			icon: appHostAndPath + '/javascript/tau/css/images/search-icon.png',
			tooltip: 'Back to search',
			tooltipType: 'title'
		}).el.on('click', this.onSearch, this);

		this.goBackSearch.hide();

		this.searchPanel = Ext.create({
			xtype: 'lookupgridpanel',
			renderTo: Ext.query('.right-ct', this.el.dom)[0],
			width: 450,
			gridConfig: {
				region: 'center',
				width: 450,
				columns: [
                    { id: 'id', header: 'ID', width: 100, sortable: true, dataIndex: 'GeneralID' },
                    { header: 'Name', width: 400, sortable: true, dataIndex: 'Name', renderer: function (value) { return String.format("<div style='white-space:normal'><a class='lookupActionLink' href='javascript:void(0)'>{0}</a></div>", value); } },
                    { header: 'State', width: 100, sortable: true, dataIndex: 'EntityState.Name' },
                    { header: 'Project', sortable: true, dataIndex: 'ProjectAbbreviation' }
                ],
				enableHdMenu: false,
				tbar: {
					xtype: 'generallistitemgridfilter',
					nameWidth: 215,
					showReleaseIteration: false,
					excludeEntityId: this._assignableId,
					showEntityType: false,
					entityTypeIds: [8],
					showProjects: false,
					projectIds: this._projectIds,
					showState: true
				}
			}
		});

		this.btnPanel = Ext.create({
			xtype: 'container',
			baseCls: 'x-plain',
			cls: 'x-btns',
			hidden: true,
			hideMode: 'display',
			elements: 'body',
			layout: 'table',
			renderTo: Ext.query('div.x-row-editor', this.el.dom)[0],
			items: [{
				ref: 'saveBtn',
				itemId: 'saveBtn',
				xtype: 'button',
				text: this.saveText || 'Mark as Duplicate',
				width: this.minButtonWidth,
				handler: this.onDuplicate,
				scope: this
			}
            ]
		});
	},

	createBugView: function (barPositionName) {
		return Ext.create({
			xtype: 'duplicatebugview',
			applyTo: Ext.query(String.format('.{0}-ct', barPositionName), this.el.dom)[0]
		});
	},

	initBugViews: function (views) {
		Ext.each(views, function (view) {
			view.on('check', this.onCheck, this);
			view.on('dataloaded', this.onPanelDataLoaded, this);
		}, this);
	},

	positionButtons: function (panel, isLeft) {
		if (this.btnPanel) {
			var popupEl = Ext.query('div[class=mark-as-duplicate]', this.el.dom)[0];

			var h = popupEl.clientHeight;
			var width = popupEl.clientWidth;
			this.btnPanel.show();

			this.goBackSearch.show();

			var bw = this.btnPanel.getWidth();
			var pw = panel.getWidth();
			this.btnPanel.el.shift({ left: (isLeft ? 0 : (width / 2)) + (pw / 2) - (bw / 2) + 130, top: h - 33, stopFx: true, duration: 0.2 });
		}
	},

	onCheck: function (view, checkbox, checked) {
		if (checked) {
			var viewToDisable = this.leftBugView.id == view.id ? this.rightBugView : this.leftBugView;
			this.positionButtons(view, this.leftBugView.id == view.id);
			viewToDisable.getToolbarFilter().disableToolbar();
		}
	},

	onDuplicate: function () {
		var duplicate = this.leftBugView.getToolbarFilter().radio.getValue() === true ? this.leftBugView : this.rightBugView;
		var bug = this.leftBugView.getToolbarFilter().radio.getValue() === false ? this.leftBugView : this.rightBugView;
		if (duplicate && bug) {
            var data = { 'bugId': bug.bugId, 'duplicateBugId': duplicate.bugId, 'stateId': duplicate.getToolbarFilter().combo.getValue() == '' ? -1 : duplicate.getToolbarFilter().combo.getValue() };
            Ext.Ajax.request({
				url: new Tp.WebServiceURL('/PageServices/MarkAsDuplicateService.asmx/Mark').toString(),
				headers: { 'Content-Type': 'application/json' },
				success: function (res) {
					this.hide();

					if (this._btnTrigger){
						document.getElementById(this._btnTrigger).click();
                    }
                    else if(this.onMarkDelegate){
                        this.onMarkDelegate(data);
                    }
                    else {
						location.reload(true);
                    }
				},
				failure: function () {
				},
				jsonData: data,
				scope: this
			});
		}
	},

	onSearch: function () {
		this.goBackSearch.hide();

		this.btnPanel.hide();
		this.hideToolbars();
		this.rightBugView.getView().hide();
		this.searchPanel.show();
	},

	onSelectDuplicate: function (e, el, grid, record, row, cell) {
		var bugId = record.data['GeneralID'];
		if (bugId && bugId != this.leftBugView.getCurrentBugId()) {
			this.searchPanel.hide();
			this.rightBugView.show(bugId);
			this.goBackSearch.show();
		}
	},

	showToolbars: function () {
		this.goBackSearch.show();

		this.leftBugView.getToolbarFilter().show();
		this.rightBugView.getToolbarFilter().show();
	},

	hideToolbars: function () {
		this.leftBugView.getToolbarFilter().hide();
		this.rightBugView.getToolbarFilter().hide();
	},

	hideViews: function () {
		this.leftBugView.getView().hide();
		this.rightBugView.getView().hide();
	},

	onPanelDataLoaded: function (panel) {
		if (panel.id == this.leftBugView.id) {
			panel.getView().show();
			this.searchPanel.on('lookupactionlinkclick', this.onSelectDuplicate, this);
		}
		if (panel.id == this.rightBugView.id) {
			panel.getView().show();
			this.showToolbars();
			panel.getToolbarFilter().radio.setValue(true);
			this.btnPanel.show();
		}
		else {
			this.hideToolbars();
		}

		ImageMinimizer.resizeImages('img[rel^=mayNeedToResize]', 400, true);
	}
});