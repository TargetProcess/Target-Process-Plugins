Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.ProjectsFilterControl = Ext.extend(Ext.form.ComboBox, {
	constructor: function (config) {
		var config = Ext.apply({
			triggerAction: 'all',
			autoSelect: false,
			lazyRender: true,
			editable: false,
			projectId: null,
			projectIds: [],
			mode: 'local',
			value: [],
			store: new Ext.data.SimpleStore({
				fields: ['id', 'name'],
				listeners: {
					load: {
						fn: function (store) {
							if (this.projectId) {
								store.each(function (r, i) {
									if (r.data[this.valueField].length == 1 && r.data[this.valueField][0] == this.projectId) {
										this.select(i, false);
									}
								}, this);
							}
						}
					},
					scope: this
				}
			}),
			valueField: 'id',
			displayField: 'name',
			listeners: {
				afterrender: {
					fn: function (combo) {
						if (combo.projectId) {
							combo.fill();
						}
					}
				},
				beforequery: {
					fn: function (e) {
						if (e.combo.data == null) {
							e.combo.fill();
						}
						if (e.combo.store.data.length == 0) {
							e.combo.store.loadData(e.combo.data);
						}
					}
				}
			}
		}, config);

		Tp.controls.Lookup.ProjectsFilterControl.superclass.constructor.call(this, config);
	},

	fill: function () {
		if (this.data == null) {
			var curProject;
			var allProjectsValue = [], contextProjectsValue = [], otherProjectsValue = [];
			this.data = [[allProjectsValue, '--- All Projects ---']];
			this.data.push([contextProjectsValue, '- Projects from Context -']);
			Array.forEach(_projectContext, function (item) {
				if (!this.projectIds || this.projectIds.length == 0 || Array.contains(this.projectIds, item[this.valueField])) {
					if (this.projectId && item[this.valueField] == this.projectId) curProject = item[this.displayField];
					this.data.push([[item[this.valueField]], item[this.displayField]]);
					contextProjectsValue.push(item[this.valueField]);
					allProjectsValue.push(item[this.valueField]);
				}
			}, this);
			this.data.push([otherProjectsValue, '- Other Projects -']);
			Array.forEach(projects, function (item) {
				if (item.type == 'Project') {
					if (!this.projectIds || this.projectIds.length == 0 || Array.contains(this.projectIds, item[this.valueField])) {
						if (!curProject && this.projectId && item[this.valueField] == this.projectId) curProject = item[this.displayField];
						if (Array.findOne(_projectContext, function (i) { return i.id == item.id; }, this) == null) {
							this.data.push([[item[this.valueField]], item[this.displayField]]);
							otherProjectsValue.push(item[this.valueField]);
							allProjectsValue.push(item[this.valueField]);
						}
					}
				}
			}, this);
			if (otherProjectsValue.length == 0)
				this.data.pop();
			if (contextProjectsValue.length == 0)
				this.data.splice(1, 1);
			if (curProject){
				this.el.dom.value = curProject;
				this.value = [this.projectId];
				this.removeClass(this.emptyClass);
			}
		}
	},

	getValue: function () {
		return this.value == '' ? [] : this.value;
	}
});

Ext.reg('projectsfiltercontrol', Tp.controls.Lookup.ProjectsFilterControl);