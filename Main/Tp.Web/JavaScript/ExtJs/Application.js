/* <![CDATA[ */
/**
* Singleton application object.
*
* Things to consider while extending this class:
* - Data and Metadata -- provide current user and process information such as entity types, workflow, etc.
* - UI -- basic UI stuff like error notification, etc.
* - Preferences -- allow widgets and forms to save state in application preferences.
* - Site map -- provide collection of common links
* - Remoting -- make it remote service factory?
* - Security -- client side action validation.
* - Events -- make it possible for various components to register and subscribe for global events?
*/

Date.monthNumbers = {};
var am = Sys.CultureInfo.CurrentCulture.dateTimeFormat.AMDesignator, pm = Sys.CultureInfo.CurrentCulture.dateTimeFormat.PMDesignator;
if (am && am.length > 0 || pm && pm.length > 0) {
	Date.formatCodes['a'] = Date.formatCodes['A'] = String.format("(this.getHours() < 12 ? '{0}' : '{1}')", am, pm);
	Date.parseCodes['a'] = Date.parseCodes['A'] = { g:1, c: String.format("if (results[{{0}}] == '{0}') {{\nif (!h || h == 12) {{ h = 0; }}\n}} else {{ if (!h || h < 12) {{ h = (h || 0) + 12; }}}}", am), s: String.format("({0}|{1})", am, pm) }
}
Array.forEach(Sys.CultureInfo.CurrentCulture.dateTimeFormat.AbbreviatedMonthNames, function (m, i) { if(m && m.length > 0) Date.monthNumbers[m] = i; });
Array.forEach(Sys.CultureInfo.CurrentCulture.dateTimeFormat.MonthNames, function (m, i) { if (m && m.length > 0) Date.monthNames[i] = m; });
Array.forEach(Date.dayNames, function (d, i) { Date.dayNames[i] = Sys.CultureInfo.CurrentCulture.dateTimeFormat.DayNames[i]; });
Date.getShortMonthName = function (month) { return Sys.CultureInfo.CurrentCulture.dateTimeFormat.AbbreviatedMonthNames[month]; };
Date.getMonthNumber = function (name) { return Date.monthNumbers[name]; };
Ext.apply(Ext.DatePicker.prototype, {
	disabledDaysText: "",
	disabledDatesText: "",
	monthNames: Date.monthNames,
	dayNames: Date.dayNames,
	startDay: Sys.CultureInfo.CurrentCulture.dateTimeFormat.FirstDayOfWeek
});
Ext.apply(Ext.form.NumberField.prototype, {
	decimalSeparator: Sys.CultureInfo.CurrentCulture.numberFormat.CurrencyDecimalSeparator
});

var Application = function () {
	var type = new Ext.extend(Object, {
		menuBootstrap: null,
		constructor: function () {
			Ext.state.Manager.setProvider(new Ext.state.CookieProvider());
		},

		getMenuBootsrap: function () {
			if (this.menuBootstrap == null) {
				this.menuBootstrap = new Tp.controls.MenuBootstrap();
				this.menuBootstrap.init();
			}
			return this.menuBootstrap;
		},

		baseUrl: '',

		user: {},

		getViewUrl: function (entityId, entityKind) {
			var url = '/View.aspx?ID=' + encodeURIComponent(entityId);
			if (entityKind) {
				url = url + '&Entity=' + encodeURIComponent(entityKind);
			}
			return new Tp.WebServiceURL(url).toString();
		}
	});
	return new type();
} ();
