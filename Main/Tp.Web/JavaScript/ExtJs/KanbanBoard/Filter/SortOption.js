Tp.controls.kanbanboard.sections.FilterPanel.SortControl = Ext.extend(Ext.BoxComponent, {
	onRender: function (ct, position) {
		Tp.controls.kanbanboard.sections.FilterPanel.SortControl.superclass.onRender.call(this, ct, position);

		var tpl = new Ext.XTemplate(
                'Sort by',
                '<tpl for=".">',
                ' <a id="{id}" href="javascript:void(0)" name="x-sort" value="{value}">{title}</a>',
                '</tpl>');

		tpl.overwrite(this.el, this.controller.getSortOptions());
	},

	//private
	afterRender: function () {
		Tp.controls.kanbanboard.sections.FilterPanel.SortControl.superclass.afterRender.call(this);

		this.items = this.el.select('a[name=x-sort]');
		this.items.each(function (item) {
			var fly = Ext.fly(item);
			if (fly.getAttribute('value') == this.filter.sort) {
				fly.addClass('backlog-current-sort');
			}
			Ext.EventManager.on(fly.dom.id, 'click', function (ev, el) { this.onSortChange(el); }, this);
		}, this);
	},

	clearCls: function () {
		this.el.select('a[name=x-sort]').each(function (item) {
			Ext.get(item).removeClass('backlog-current-sort');
		});
	},

	onSortChange: function (el) {
		var key = el.getAttribute('value');
		if (this.filter.sort != key) {
			this.clearCls();
			Ext.fly(el).addClass('backlog-current-sort');
			this.filter.sort = key;
			this.filter.notifyChanged();
		}
	}
});
