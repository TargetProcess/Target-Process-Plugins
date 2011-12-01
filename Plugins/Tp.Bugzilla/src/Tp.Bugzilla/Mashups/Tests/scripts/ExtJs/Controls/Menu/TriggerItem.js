Ext.ns('Tp.controls.menu');

Tp.controls.menu.TriggerItem = Ext.extend(Object, {

	itemSelector: null,

	element: null,

	context: null,

	constructor: function(config) {
		Tp.util.validateForNulls([config]);
		Tp.util.validateForNulls([config.itemSelector]);
		Ext.apply(this, config);
		this.element = Ext.get(config.id);
		this.element.on('click', this._onClick, this);
	},

	_onClick: function() {
		this.itemSelector.show(this, this.context);
	},

	update: function(newContext, html) {
		this.context = newContext;
		this.element.update(html);
		this.element.highlight("ffd700", { attr: "background-color", endColor: "ffffff", easing: 'easeIn', duration: 2.5 });
	}
});
