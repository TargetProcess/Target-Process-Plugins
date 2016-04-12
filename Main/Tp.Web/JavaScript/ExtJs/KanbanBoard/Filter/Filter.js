Ext.ns('Tp.controls.kanbanboard.sections');

/**
* Renders as backlog filter form, makes it possible to sort and filter backlog items.
*/
Tp.controls.kanbanboard.sections.FilterPanel = Ext.extend(Ext.Panel, {
	filter: null,
	controller: null,
	projectId: 0,

	constructor: function (config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}

		if (!config.filter) {
			throw new Error('Filter not specified');
		}

		var typeOption = new Tp.controls.kanbanboard.sections.FilterPanel.TypeControl({
			controller: config.controller,
			filter: config.filter
		});

		var sortOption = new Tp.controls.kanbanboard.sections.FilterPanel.SortControl({
			controller: config.controller,
			filter: config.filter
		});

		this.projectId = config.controller.project.id;

		Tp.controls.kanbanboard.sections.FilterPanel.superclass.constructor.call(this, Ext.apply(config, this.createFilterConfig(typeOption, sortOption)));
		this._init();
		this.controller.setFilter(this);
	},

	_init: function () {
		//creates handler on render event for textField for explicit event binding as before there is no undelying element
		function createDeleteHandler(componentId, deleteHandler) {
			return function () {
				//we need explicit handler for DELETE Key as default one doesn't work out
				function deleteHandlerWrapper(eventObj, source) {
					if (eventObj.getCharCode() == eventObj.DELETE) {
						deleteHandler(Ext.fly(source), eventObj);
					}
				}

				var elementId = this.findById(componentId).getEl().id;
				Ext.EventManager.on(elementId, 'keyup', deleteHandlerWrapper, this);
			};
		}

		var keywordsId = this.getKeywordsId();
		var keywords = this.findById(keywordsId);
		keywords.on('render', createDeleteHandler(keywordsId, this.handleKeywordsEvents.createDelegate(this)), this);
		keywords.addListener('keydown', this.handleKeyDown);

		var tagsId = this.getTagsId();
		var tags = this.findById(tagsId);
		tags.on('render', createDeleteHandler(tagsId, this.handleTagsEvents.createDelegate(this)), this);
		tags.addListener('keydown', this.handleKeyDown);
	},

	createFilterConfig: function (typeOption, sortOption) {
		var items = [
			{
				border: false,
				items: [
					{
						border: false,
						bodyStyle: {
							'padding-top': '0px',
							'padding-bottom': '0'
						},
						items: [
							{
								border: false,
								bodyStyle: {
									'padding-top': '1px',
									'padding-bottom': '0px'
								},
								items: [
									{ id: this.getKeywordsId(),
										xtype: 'textfield',
										style: {
											width: '48%',
											marginRight: '2%'
										},
										emptyText: 'keywords',
										fieldLabel: 'Keyword',
										maxLength: 30,
										enableKeyEvents: true,
										listeners: {
											keypress: this.handleKeywordsEvents, // this is for alphanumeric keys
											specialkey: this.handleKeywordsEvents, // this is for backspace, delete, tab, etc
											buffer: 300,
											scope: this
										}
									},
									{
										id: this.getTagsId(),
										xtype: 'textfield',
										style: {
											width: '38%'
										},
										emptyText: 'tags',
										fieldLabel: 'Tag',
										maxLength: 30,
										enableKeyEvents: true,
										listeners: {
											keypress: this.handleTagsEvents, // this is for alphanumeric keys
											specialkey: this.handleTagsEvents, // this is for backspace, delete, tab, etc
											buffer: 300,
											scope: this
										}
									}
								]
							}
						]
					},
					{
						border: false,
						bodyStyle: {
							'padding-top': '3px',
							'padding-bottom': '5px'
						},
						items: typeOption
					},
					{
						border: false,
						bodyStyle: {
							'padding-top': '5px',
							'padding-bottom': '5px'
						},
						items: sortOption
					}
				]
			}
		];

		return {
			hidden: true,
			border: false,
			bodyCssClass: 'backlog-filter',
			items: items
		};
	},

	handleKeyDown: function (field, ev) {
		//Disables ENTER key for specified field
		if (ev.getKey() == ev.ENTER) {
			ev.stopEvent();
		}
	},

	handleSelect: function (field, ev) {
		return;
	},

	getKeywords: function (field) {
		var x = (field.getValue() || '').trim();
		if (x.length) {
			return x.toLowerCase().split(/\s+/).sort();
		}
		else {
			return [];
		}
	},

	handleKeywordsEvents: function (field, ev) {
		var a = this.getKeywords(field);
		if (a.join(' ') != this.filter.keywords.join(' ')) {
			this.filter.keywords = a;
			this.filter.notifyChanged();
		}
	},

	handleTagsEvents: function (field, ev) {
		if (ev.getCharCode() == 13)
			ev = null;
		var a = this.getKeywords(field);
		if (a.join(' ') != this.filter.tags.join(' ')) {
			this.filter.tags = a;
			this.filter.notifyChanged();
		}
	},

	getKeywordsId: function () {
		return 'keywords' + this.projectId;
	},

	getTagsId: function () {
		return 'tags' + this.projectId;
	}
});