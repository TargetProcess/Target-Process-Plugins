Ext.ns('Tp.custom.plugins');
Tp.custom.plugins.TestCaseDetailsExpander = Ext.extend(Tp.controls.grid.plugins.BunchRowsExpander, {
	_expandedRowIndex: -1,
	_expandDivId: "hd_{id}",

	constructor: function () {
		Tp.custom.plugins.TestCaseDetailsExpander.superclass.constructor.call(this, { tpl: this.getExpandTemplate(), enableCaching: false, expandOnDblClick: false });
		this.addEvents('onCompleteLoading');
		this.on("expand", this.onExpand, this);
	},

	init: function (grid) {
		Tp.custom.plugins.TestCaseDetailsExpander.superclass.init.call(this, grid);
		this.grid.on("entityStateChanging", this.onEntityStateChanged, this);
	},

	onEntityStateChanged: function (entityId) {
		var store = this.grid.getStore();
		var currentRowIndex = store.indexOfId(entityId);
		this.collapseRow(currentRowIndex);

		if (currentRowIndex == (this.grid.getStore().getCount() - 1))
			return;

		var nextRowIndex = currentRowIndex + 1;
		this.expandRow(nextRowIndex);
	},

	renderer: function (v, p, record) {
		if (record.data.Modification == 2)
			return "&nbsp;";

		return Tp.custom.plugins.TestCaseDetailsExpander.superclass.renderer.call(this, v, p, record);
	},

	getExpandTemplate: function () {
		return new Ext.Template('<div id="' + this._expandDivId + '"/>');
	},

	getDivIdTemplate: function () {
		return new Ext.Template(this._expandDivId);
	},

	onExpand: function (sender, record, body, rowIndex) {
		var div = Ext.get(this.getDivIdTemplate().apply(record.data));
		div.update('Loading details...');
		//record.div = div;
		Ext.Ajax.request({
			url: new Tp.WebServiceURL('/PageServices/TestCaseListService.asmx/GetChanges').toString(),
			headers: { 'Content-Type': 'application/json' },
			success: Function.createDelegate(this, this.onCompleteLoadingDetails),
			jsonData: { 'testCaseID': record.id }
		});
		this._expandedRowIndex = rowIndex;
	},

	onCompleteLoadingDetails: function (changes, record) {
		var d = Ext.decode(changes.responseText).d;
		var html = "";
		var div = Ext.get(this.getDivIdTemplate().apply({'id':d.generalID}));
		if (d.steps.length > 0 || d.success.length > 0) {
			var templateSteps = new Ext.Template('<table style="padding-left:4px;width:100%" cellspacing="0" cellpadding="5"><tr><td id="tdSteps_' + div.dom.id + '" width="50%"><b>Steps</b><br/><br/>{steps}</td><td id="tdSuccess_' + div.dom.id + '" width="50%"><b>Success</b><br/><br/>{success}</td></tr></table><br />');
			html += templateSteps.apply(d);
		}

		div.update(html);
		var tdSteps = Ext.get('tdSteps_' + div.dom.id);
		var tdSuccess = Ext.get('tdSuccess_' + div.dom.id);
		this.resizeImages(tdSteps);
		this.resizeImages(tdSuccess);
		this.onCompleteLoading();
	},

	onCompleteLoading: function () { },

	resizeImages: function (parentElement) {
		if (parentElement == null) {
			return;
		}
		var images = Ext.query('img', parentElement.dom.id);
		for (var n = 0; n < images.length; n++) {
			var img = Ext.fly(images[n]);
			img.setVisibilityMode(Ext.Element.DISPLAY);
			img.setVisible(true, true);
			if (img.getWidth() > parentElement.getWidth()) {
				var ratio = img.getHeight() / img.getWidth();
				var newImgHeight = ratio * (parentElement.getWidth() - 50);
				img.setHeight(newImgHeight);
				img.setWidth(parentElement.getWidth() - 50);
				img.applyStyles({ display: 'block' })
				var anchor = new Ext.Element(document.createElement('a'));
				anchor.set({ rel: 'lightbox', href: img.getAttribute('src') });
				anchor.replace(img.dom);
				anchor.appendChild(img.dom);
			}
		}

	}
})