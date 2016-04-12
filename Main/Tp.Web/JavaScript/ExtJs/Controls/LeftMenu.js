Ext.ns('Tp.controls');
Tp.controls.LeftMenu = Ext.extend(Object, {
	hideReportLink: null,
	showReportLink: null,
	menuElement: null,

	constructor: function (config) {
		Ext.apply(this, config)

		Ext.get(this.hideReportLink).on('click', function (ev, el) {
			this.hideAdminMenu();
			Ext.util.Cookies.set(this.cookieKey, false);
		}, this);

		Ext.get(this.showReportLink).on('click', function (ev, el) {
			this.showAdminMenu();
			Ext.util.Cookies.set(this.cookieKey, true)
		}, this);

		if (Ext.util.Cookies.get(this.cookieKey) == false.toString()) {
			this.hideAdminMenu();
		}
		else {
			this.showAdminMenu();
		}

		Tp.controls.LeftMenu.superclass.constructor.call(this, config);
	},

	showAdminMenu: function () {
		Ext.get(this.menuElement).show('display');
		Ext.get(this.showReportLink).hide('display');
	},

	hideAdminMenu: function () {
		Ext.get(this.menuElement).hide('display');
		Ext.get(this.showReportLink).show('display');
	}
});