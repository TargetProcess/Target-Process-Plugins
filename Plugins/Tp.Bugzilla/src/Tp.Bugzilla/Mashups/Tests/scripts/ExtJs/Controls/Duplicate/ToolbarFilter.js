Ext.ns('Tp.controls.Duplicate');

Tp.controls.Duplicate.ToolbarFilter = Ext.extend(Ext.Toolbar, {
	constructor: function (config) {
		var config = Ext.apply({
			//hidden: true,
			hideMode: 'display',
			hideBorders: true,
            cls: 'this-bug-panel',
			items: [
			{
			    xtype: 'radio',
			    cls: 'nopadding',
				boxLabel: 'This Bug is Duplicate',
				listeners: {
					check: {
						fn: this.onCheck,
						scope: this
					}
				},
				ref: 'radio'
			}, {
				xtype: 'tbseparator'
			}, {
				xtype: 'combo',
				typeAhead: true,
				triggerAction: 'all',
				lazyRender: true,
				mode: 'local',
				disabled: true,
                editable: false,                
				width: 140,
				emptyText: 'Change State',
                emptyClass: 'activeStateCombo',
				store: new Ext.data.SimpleStore({
					fields: ['id', 'name']
				}),
				valueField: 'id',
				displayField: 'name',
				ref: 'combo',               
				afterrender: function(combo) {
					combo.list.setSize('auto', 0);
					combo.innerList.setSize('auto', 0);
				},
				getZIndex: function () {
					return 10000012;
				}
			}]
		}, config);

		Tp.controls.Duplicate.ToolbarFilter.superclass.constructor.call(this, config);
	},

	disableToolbar: function () {
		if (!this.combo.disabled) {
			this.radio.setValue(false);
		}
	},

	onCheck: function (checkbox, checked) {
		if (checked) {
			this.combo.enable();
			this.fireEvent('check', checkbox, checked);
		}
		else {
			this.combo.reset();
			this.combo.disable();
		}
	},

	fillStatesCombo: function (states) {
		var jsonData = new Object();
		jsonData.data = [];
		Array.forEach(states, function (item) {
			jsonData.data.push([item[this.combo.valueField], item[this.combo.displayField]]);
		}, this);
		this.combo.reset();
		this.combo.store.loadData(jsonData.data);
	}
});

Ext.reg('toolbarfilter', Tp.controls.Duplicate.ToolbarFilter);