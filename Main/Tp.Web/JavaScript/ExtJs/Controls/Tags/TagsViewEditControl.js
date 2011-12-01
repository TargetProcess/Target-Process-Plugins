Ext.ns("Tp.controls");

Tp.controls.TagsViewEditControl = Ext.extend(Ext.DataView, {
	constructor: function (config) {
		if (!Ext.get(config.applyTo))
			return;

		this.entityId = config.entityId;
		this.tpl = this.getTpl();

		config = Ext.apply({
			applyTo: config.applyTo,
			tpl: this.tpl,
			autoHeight: true,
			//this added for IE 7
			itemSelector: 'view-item'
		}, config);

		TagsStore.getStore(this.entityId, this);

		Tp.controls.TagsViewEditControl.superclass.constructor.call(this, config);
	},

	refresh: function () {
		Tp.controls.TagsViewEditControl.superclass.refresh.apply(this, arguments);
		this.subscribeTagDivs();
	},

	getTpl: function () {
		return new Ext.XTemplate(
                '<tpl for=".">',
                    '<div class="editable-tag view-item">',
                    '<tpl if="0 < primeId">' +
                            '<a href="{[Application.getViewUrl(values.primeId, values.kind)]}" class="a-tag" title="{relatedName}">{name}</a>' +
                            '<tpl if="allowToEdit == 1"><a class="delete-tag" name="{name}" href="javascript:void(0)" title="Unduplicate">&nbsp;&nbsp;&nbsp;</a></tpl>',
                    '</tpl>',
                    '<tpl if="primeId <= 0">',
                            '<span class="s-tag">{name}</span>',
                            '<tpl if="allowToEdit == 1"><a class="delete-tag" name="{name}" href="javascript:void(0)" title="Delete tag">&nbsp;&nbsp;&nbsp;</a></tpl>',
                    '</tpl>',
                    '</div>',
                '</tpl>'
            );
	},

	subscribeTagDivs: function () {
		var tagDivs = this.el.select('div[class*=editable-tag]', true);

		Ext.each(tagDivs, function (div) {
			var element = Ext.get(div);

			element.addClassOnOver('editable-tag-hover');
		}, this);

		var deleteLinks = this.el.select('div[class*=editable-tag] a[class=delete-tag]', true);

		Ext.each(deleteLinks, function (a) {
			var element = Ext.get(a);
			element.on('click', this.deleteTag, this);
		}, this);
	},

	deleteTag: function (e, t, o) {
		TagsStore.deleteTag(this.entityId, t.name, this);
	},

	onTagsReceived: function (response) {
		this.tagStore = new Ext.data.JsonStore({
			fields: ['name', 'primeId', 'relatedName', 'kind', 'allowToEdit'],
			data: Ext.util.JSON.decode(response.responseText).d
		});

		this.setStore(this.tagStore);
	},

	onTagsReceivedFail: function (response) {
		if (response.status === 0)
			return;
		SetLastWarning("There was an error during Tags getting.");
	},

	onDeleteTagSuccess: function () {
		TagsStore.getStore(this.entityId, this);
	},

	onDeleteTagFail: function () {
		SetLastWarning("There was an error during tag deletion.");
	}
});

TagsStore = function () {
	return new function () {
		this.getStore = function (entityId, context) {
			Ext.Ajax.request({
				url: new Tp.WebServiceURL('/PageServices/TagsBoardService.asmx/GetTagsForEntity').toString(),
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				success: Function.createDelegate(context, context.onTagsReceived),
				failure: Function.createDelegate(context, context.onTagsReceivedFail),
				jsonData:
                {
                	'entityId': entityId
                }
			});
		};

		this.deleteTag = function (entityId, tagName, context) {

			Ext.Ajax.request({
				url: new Tp.WebServiceURL('/PageServices/TagsBoardService.asmx/DeleteTagForEntity').toString(),
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				success: Function.createDelegate(context, context.onDeleteTagSuccess),
				failure: Function.createDelegate(context, context.onDeleteTagFail),
				jsonData:
                {
                	'entityId': entityId,
                	'tag': tagName
                }
			});
		}
	}
} ();

Ext.reg('tagsvieweditcontrol', Tp.controls.TagsViewEditControl);