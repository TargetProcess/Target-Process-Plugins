Tp.controls.kanbanboard.sections.FilterPanel.TypeControl = Ext.extend(Ext.BoxComponent, {

	onRender: function (ct, position) {
		Tp.controls.kanbanboard.sections.FilterPanel.TypeControl.superclass.onRender.call(this, ct, position);

		var tpl = new Ext.XTemplate(
                'Show',
                '<tpl for=".">',
                '<span id="panel-{value}-filter" style="display:{display}">',
				' <input id="{id}" type="checkbox" name="{id}" value="{value}"/><label for="{id}">{title}</label>',
				'</span>',
                '</tpl>');
		var filterTypes = this.controller.getFilterTypes();

		if (this.getVisibleCount(filterTypes) > 1) {
			tpl.overwrite(this.el, filterTypes);
		}
	},

	getVisibleCount: function (types) {
		function isVisible(type) {
			return type.display != 'none';
		}

		var count = 0;
		for (var i =0; i < types.length; i++) {
			if (isVisible(types[i])) {
				count += 1;
			}
		}
		return count;
	},

	afterRender: function () {

		Tp.controls.kanbanboard.sections.FilterPanel.TypeControl.superclass.afterRender.call(this);

		this.items = this.el.select('input[type=checkbox]');

		this.items.each(function (item) {
			var fly = Ext.fly(item);
			fly.dom.checked = this.filter.type[fly.dom.value];
			fly.on('click', function (ev, el) {
				this.onTypeChange(el.value, el.checked);
			}, this);
		}, this);
	},

	onTypeChange: function (type, show) {

		this.filter.type[type] = show;

		this.filter.notifyChanged();
	}

});