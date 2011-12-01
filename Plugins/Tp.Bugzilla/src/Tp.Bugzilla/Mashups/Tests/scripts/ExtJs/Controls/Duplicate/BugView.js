Ext.ns('Tp.controls.Duplicate');

Tp.controls.Duplicate.BugView = Ext.extend(Ext.Container, {
	constructor: function (config) {

		var config = Ext.apply({
			bugId: null,
			border: true,
			items: [
			{
				xtype: 'container',
				cls: 'bug-view p-10',
				hidden: true,
				hideMode: 'display',
				ref: 'view',
				items: [
				{
					xtype: 'box',
					autoEl: {
						tag: 'h2',
						html: '<a class="bug-id-link" href="javascript:void(0)"></a>&nbsp;<span class="bug-name"></span>'
					},
					ref: 'header'
				}, {
					xtype: 'box',
					autoEl: {
						tag: 'div',
                        cls: 'infoBlock'
                    },
					ref: 'infoBlock'
				}, {
					xtype: 'label',
                    cls: 'bug-description',
					ref: 'description'
				}]
			}, {
				xtype: 'toolbarfilter',
				ref: 'Filter',
				ownerCt: this.ownerCt,
				listeners: {
					check: {
						fn: function (checkbox, checked) { this.fireEvent('check', this, checkbox, checked); },
						scope: this
					}
				},
				ref: 'toolbarFilter'
			}]
		}, config);

		Tp.controls.Duplicate.BugView.superclass.constructor.call(this, config);
	},

	show: function (bugId) {
		if (bugId && this.bugId != bugId) {
			Ext.Ajax.request({
				url: new Tp.WebServiceURL('/PageServices/MarkAsDuplicateService.asmx/GetBug').toString(),
				headers: { 'Content-Type': 'application/json' },
				success: function (res) {
					var r = jsonParse(res.responseText).d;
                    var bugLink = Ext.query('a.bug-id-link', this.view.header.el.dom)[0];
                    bugLink.innerHTML = '#' + r.ID;
                    bugLink.href = Application.getViewUrl(r.ID, "Bug");
                    
					Ext.query('span.bug-name', this.view.header.el.dom)[0].innerHTML = r.Name;
					this.view.infoBlock.el.dom.innerHTML = String.format('<p><span>State:</span> {0}</p><p><span>Owner:</span> {1}</p><p><span>Creation date:</span> {2}</p>', r.State, r.OwnerName, r.CreateDate);
					this.view.description.update(r.Description);
					this.getToolbarFilter().fillStatesCombo(r.States);
					this.bugId = bugId;
					this.record = r;
					this.fireEvent('dataloaded', this);
					Tp.controls.Duplicate.BugView.superclass.show.call(this);
				},
				failure: function () {
				},
				jsonData: { 'bugId': bugId },
				scope: this
			});
		}
		else {
			this.fireEvent('dataloaded', this);
			Tp.controls.Duplicate.BugView.superclass.show.call(this);
		}
	},


	getToolbarFilter: function () {
		return this.toolbarFilter;
	},

	getView: function () {
		return this.view;
	},

	getCurrentBugId: function () {
		return this.bugId;
	}
});

Ext.reg('duplicatebugview', Tp.controls.Duplicate.BugView);
