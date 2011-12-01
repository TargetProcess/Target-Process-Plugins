Ext.ns('Tp.controls.kanbanboard.board');

/*
* Board customizer dialog it allows reordering swimlanes.
*/
Tp.controls.kanbanboard.board.ReFresh = Ext.extend(Ext.Panel, {
	controller: null,
	constructor: function (config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}

		Ext.apply(config, {
			border: false,
			layout: 'table',
			defaults: {
				bodyStyle: 'padding:10px'
			},
			layoutConfig: {
				tableAttrs: {
					style: {
						width: '100%'
					}
				}
			},
			items: [{
				html: '<b>Interval in minutes</b>'
			}, {
				xtype: 'sliderfield',
				value: config.controller.refreshInterval,
				tipText: function (thumb) { return thumb.value > 0 ? String(thumb.value) : 'Never'; },
				boxMaxWidth: 180,
				maxValue: 60,
				minValue: 0,
				width: 180
			}, {
				html: '<p>60 mins</p>'
			}]
		});

		Tp.controls.kanbanboard.board.ReFresh.superclass.constructor.call(this, config);

		this.slider = this.findByType('sliderfield')[0];
	},

	/*
	* Reorder swimlanes in the controller according to the user preferences.
	*/
	applyCustomization: function () {
		this.controller.refreshInterval = this.slider.getValue() || 0;
	}
});