Tp.controls.menu.AlertView = Ext.extend(Ext.util.Observable, {
	_containerId: null,
	_hideMessageButtonId: null,
	constructor: function (config) {
		Ext.apply(this, config);
	},

	show: function () {
		Tp.controls.getOverlay().show();

		this._containerId = Ext.id();
		this._hideMessageButtonId = Ext.id();

		var t = new Ext.XTemplate('<div id="{containerId}" class="notificationPopup">',
                                        '<span>',
                                            '<tpl for="alertCollection" >',
                                                '{Message}<br />',
                                            '</tpl>',
                                            '<div class="btnBlock"><input type="button" value="Close" id="{hideMessageButtonId}"/></div>',
                                        '</span>',
                                    '</div>'
                                    );
		t.compile();
		t.append(Ext.getBody(), {
			containerId: this._containerId,
			hideMessageButtonId: this._hideMessageButtonId,
			alertCollection: this.alertCollection
		});

		Ext.get(this._hideMessageButtonId).on('click', this._onHideClick, this);
	},

	_onHideClick: function () {
		this.close();
	},
	markMessagesAsRead: function () {
		var messageIds = [];
		Ext.each(this.alertCollection, function (alertMessage) {
			messageIds.push(alertMessage.ID);
		});

		Ext.Ajax.request({
			url: appHostAndPath + '/PageServices/WebMethods.asmx/MarkMessagesAsRead',
			headers: { 'Content-Type': 'application/json' },
			success: this.onMarkSuccess,
			failure: this.onMarkFailed,
			jsonData: { 'messageIds': messageIds },
			scope: this
		});
	},

	onMarkSuccess: function () {
	},

	onMarkFailed: function () {
	},

	close: function () {
		this.markMessagesAsRead();
		Tp.controls.getOverlay().hide();
		Ext.get(this._containerId).hide();
	}
});