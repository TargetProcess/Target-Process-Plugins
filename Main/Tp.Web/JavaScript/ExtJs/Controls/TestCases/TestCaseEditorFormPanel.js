Ext.ns('Tp.controls.TestCases');

Tp.controls.TestCaseEditorFormPanel = Ext.extend(Ext.form.FormPanel, {

	constructor: function () {
		Tp.controls.TestCaseEditorFormPanel.superclass.constructor.call(this, this._createConfig());
		setTimeout(function () { var txtName = Ext.fly('txtName'); if (txtName) txtName.focus(); }, 1000);
	},

	_createConfig: function () {
		return {
			baseCls: 'x-plain',
			defaultType: 'textfield',
			layout: 'table',
			layoutConfig: { columns: 2 },
			items: [
                {
                	xtype: 'hidden',
                	name: 'generalID',
                	colspan: 2,
                	autoWidth: true
                },
                {
                	xtype: 'label',
                	text: 'Name',
                	colspan: 2,
                	autoWidth: true
                },
                {
                	id: 'txtName',
                	xtype: 'textfield',
                	text: 'name',
                	name: 'name',
                	width: '98.8%',
                	colspan: 2
                },
                {
                	xtype: 'label',
                	text: 'Steps',
                	autoWidth: true
                },
                {
                	xtype: 'label',
                	text: 'Success',
                	autoWidth: true
                },
                this._createHtmlEditor('steps'),
                this._createHtmlEditor('success')
            ]
		};
	},

	_createHtmlEditor: function (htmlEditorNameId) {
		return {
			id: htmlEditorNameId,
			name: htmlEditorNameId,
			xtype: 'htmleditor',
			enableColors: false,
			enableAlignments: false,
			enableFontSize: false,
			enableFormat: true,
			width: 352,
			height: 280,
			value: '<br/>'
		}
	}
});
