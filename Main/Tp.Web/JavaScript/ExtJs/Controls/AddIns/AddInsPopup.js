Tp.controls.menu.AddInsPopup = Ext.extend(Ext.util.Observable, {
	_containerId: null,
	_hideMessageButtonId: null,
	constructor: function (config) {
		Ext.apply(this, config);
	},

	show: function () {
		Tp.controls.getOverlay().show();

		this._containerId = Ext.id();
		this._hideMessageButtonId = Ext.id();

		var t = new Ext.XTemplate('<div id="{containerId}" class="add-insPopup">',
										'<div class="titleBlock"><div class="titleBlockInner"><h1>Download Add-ins</h1><p>Useful TargetProcess Add-ins.</p><a href="javascript:void(0)" id="{hideMessageButtonId}" class="close-popup-btn"></a></div></div>',
										'<table class="pb-20">',
											'<tpl for="addInsCollection">',
												'<tr>',
												'<td class="addinsLogo pt-20"><div class="{Logo}" /></div>',
												'<td class="pt-20" style="width: 340px"><h2>{Name}</h2>',
												'<span class="desc">{Description}</span></td>',
												'<td class="downAddins pt-20">',
													'<tpl if="Href.length &gt; 0">',
														'<a class="downloadButton" href="{Href}"><div style="float: left">Download</div><div class="downloadIcon"></div></a>',
													'</tpl>',
													'<tpl if="Href.length == 0">',
														'<div class="disabledLink"><div style="float: left">Download</div><div class="downloadIcon"></div></div>',
													'</tpl>',
												'</td>',
											'</tpl>',
										'</table>',
									'</div>');
		t.compile();
		t.append(Ext.getBody(), {
			containerId: this._containerId,
			hideMessageButtonId: this._hideMessageButtonId,
			addInsCollection: this.addInsCollection
		});

		Ext.get(this._hideMessageButtonId).on('click', this._onHideClick, this);
	},

	_onHideClick: function () {
		this.close();
	},

	close: function () {
		Tp.controls.getOverlay().hide();
		Ext.get(this._containerId).hide();
	}
});