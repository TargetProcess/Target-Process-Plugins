Ext.ns('Tp.controls.grid.plugins');

Tp.controls.grid.plugins.GroupSummary = function (config) {
	Ext.apply(this, config);
};

Ext.extend(Tp.controls.grid.plugins.GroupSummary, Tp.controls.grid.plugins.TotalSummary, {
	groupSummaries: null,

	bindEvents: function () {
		Tp.controls.grid.plugins.GroupSummary.superclass.bindEvents.call(this);
		this.view.doGroupEnd = this.doGroupEnd.createDelegate(this);
		this.view.afterMethod('onAllColumnWidthsUpdated', this.doAllWidths, this);
	},

	findGroupSummary: function (groupValue) {
		if (this.groupSummaries == null)
			return null;

		for (var i = 0; i < this.groupSummaries.length; i++) {
			if (this.groupSummaries[i].GroupValue == groupValue) {
				return this.groupSummaries[i].Summary;
			}

			if (groupValue instanceof Date) {
				if (this.groupSummaries[i].GroupValue && groupValue.toString() == this.groupSummaries[i].GroupValue.toString()) {
					return this.groupSummaries[i].Summary;
				}
			}
			if (!this.groupSummaries[i].GroupValue && !groupValue) {
				return this.groupSummaries[i].Summary;
			}
		}

		return null;
	},

	doGroupEnd: function (buf, g, cs, ds, colCount) {
		if (g.rs.length < 2) {
			buf.push(this.view.endGroup);
			return;
		}

		var summary = this.findGroupSummary(g.gvalue);
		if (summary == null) {
			buf.push(this.view.endGroup);
			return;
		}

		buf.push('</div>', this.renderSummary({ data: summary }, cs), '</div>');
	},

	onStoreLoad: function (sender, records, options) {
		if (!this.isGroupingOn()) {
			return;
		}

		this.groupSummaries = options.groupSummaries;
		this.view.refresh();
	},

	doAllWidths: function (ws, tw) {
		var gs = this.view.getGroups(), s, cells, wlen = ws.length;
		for (var i = 0, len = gs.length; i < len; i++) {
			s = gs[i].childNodes[2];
			if (s == null)
				continue;
			s.style.width = tw;
			s.firstChild.style.width = tw;
			cells = s.firstChild.rows[0].childNodes;
			for (var j = 0; j < wlen; j++) {
				cells[j].style.width = ws[j];
			}
		}
	}
});
