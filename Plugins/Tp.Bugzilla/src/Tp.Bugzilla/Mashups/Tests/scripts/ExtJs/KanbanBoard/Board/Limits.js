Ext.ns('Tp.controls.kanbanboard.board');

/*
* Board customizer dialog it allows reordering swimlanes.
*/
Tp.controls.kanbanboard.board.Limits = Ext.extend(Ext.Panel, {
	controller: null,
	fields: [],
	constructor: function (config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}

		var states = config.controller.swimlanes;

		if (config.controller.swimlanes.hasPlannedSwimlane()) {
			states = [config.controller.swimlanes.plannedSwimlane].concat(states);
		}

		this.fields = [];
		for (var n = 0; n < states.length; n++) {
			if (states[n].name != 'Error') {
				this.fields.push(new Ext.form.NumberField({
					name: states[n].id,
					value: states[n].limit == 0 ? null : states[n].limit,
					fieldLabel: states[n].title,
					allowBlank: true,
					allowDecimals: false,
					allowNegative: false,
					maxValue: 999,
					maxLength: 3
				}));
			}
		}

		Ext.apply(config, {
			border: false,
			items: new Ext.FormPanel({
				frame: false,
				border: false,
				bodyStyle: 'padding: 10px',
				defaults: { width: 32, labelStyle: 'font-weight:bold;width:200px' },
				items: this.fields
			})
		});

		Tp.controls.kanbanboard.board.Limits.superclass.constructor.call(this, config);
	},

	/*
	* Reorder swimlanes in the controller according to the user preferences.
	*/
	applyCustomization: function () {
		for (var n = 0; n < this.fields.length; n++) {
			this.controller.swimlanes.item(this.fields[n].name).limit = this.fields[n].getValue() || 0;
		}
	}
});