Ext.ns('Tp.controls.Prioritization');

Tp.controls.Prioritization.AssignableFilter = Ext.extend(Ext.Panel, {
	constructor: function (config) {
		this.entityTypeCheckBoxes = this._extractCheckBoxes(config.store);

		this.stateNameTextField = new Ext.form.TextField({
			name: 'state',
			width: '166px',
			id: 'stateNameTextField',
			emptyText: 'State',
			cls: 'label',
			value: ''
		});

		this.selectEntityCheckBoxes = new Ext.form.CheckboxGroup({
			columns: 1,
			cls: 'label filter-panel-area-margin',
			items: this.entityTypeCheckBoxes
		});

		if (this.entityTypeCheckBoxes.length <= 1) {
			this.selectEntityCheckBoxes.hide();
		}

		config = Ext.apply({
			autoHeight: true,
			cls: 'filter-panel-margin',
			layout: 'table',
			layoutConfig: {
				columns: 1
			},
			items: [this.stateNameTextField, this.selectEntityCheckBoxes]
		}, config);

		Tp.controls.Prioritization.AssignableFilter.superclass.constructor.call(this, config);
	},

	_extractCheckBoxes: function (store) {
		if (store == null) {
			return [];
		}
		var entityTypeCheckBoxes = [];
		for (var i = 0; i < store.getCount(); i++) {
			var item = store.data.items[i].data;

			var checkBox = new Ext.form.Checkbox({
				boxLabel: ExtJs.tp.util.Format.EntityType(item.entityIcon),
				checked: true
			});
			checkBox.entityTypeId = item.entityTypeId;
			entityTypeCheckBoxes.push(checkBox);
		}
		;
		return entityTypeCheckBoxes;
	},

	getFilterValues: function () {
		var stateValue = new String(this.stateNameTextField.getValue());
		var result = {
			filter: {
				entityTypeIds: [],
				entityState: stateValue.trim()
			}
		};

		Array.forEach(this.entityTypeCheckBoxes, function (checkBox) {
			if (checkBox.getValue()) {
				result.filter.entityTypeIds.push(checkBox.entityTypeId);
			}
		}, this);

		return result;
	},

	restoreFilterValues: function (filter) {
		if (!filter)
			return;
		this.stateNameTextField.setValue(filter.entityState);

		Array.forEach(this.entityTypeCheckBoxes, function (checkBox) {
			checkBox.setValue(false);
		}, this);

		Array.forEach(filter.entityTypeIds, function (entityTypeId) {
			Array.forEach(this.entityTypeCheckBoxes, function (checkBox) {
				if (checkBox.entityTypeId == entityTypeId)
					checkBox.setValue(true);
			}, this);
		}, this);
	},

	setFilterDefaultValues: function () {
		this.stateNameTextField.setValue('');

		Array.forEach(this.entityTypeCheckBoxes, function (checkBox) {
			checkBox.setValue(true);
		}, this);
	}
});

Ext.reg('tpassignablefilter', Tp.controls.Prioritization.AssignableFilter);