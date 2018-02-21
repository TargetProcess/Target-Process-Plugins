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

if (Sys.Browser.agent == Sys.Browser.Safari) {
    /* Bug 74878 */
    Sys.Preview.UI.IEDragDropManager.prototype.getScrollOffset = function Sys$Preview$UI$IEDragDropManager$getScrollOffset(element, recursive) {
        var left = element.scrollLeft;
        var top = element.scrollTop;
        var offsetDoubleCalculated = (document.documentElement.scrollTop == document.body.scrollTop) && (document.documentElement.scrollLeft == document.body.scrollLeft);
        if (recursive) {
            var parent = element.parentNode;
            while (parent != null && parent.scrollLeft != null) {
                if (parent == document.documentElement && offsetDoubleCalculated) {
                    break;
                }
                left += parent.scrollLeft;
                top += parent.scrollTop;

                if (parent == document.body && (left != 0 && top != 0)) {
                    break;
                }
                parent = parent.parentNode;
            }
        }
        return { x: left, y: top };
    };

	Sys.UI.DomElement.getLocation = function (element) {
		/// <summary locid="M:J#Sys.UI.DomElement.getLocation" />
		/// <param name="element" domElement="true"></param>
		/// <returns type="Sys.UI.Point"></returns>
		var e = Function._validateParams(arguments, [
					{ name: "element", domElement: true }
				]);
		if (e) throw e;
		if ((element.window && (element.window === element)) || element.nodeType === 9) return new Sys.UI.Point(0, 0);
		var offsetX = 0;
		var offsetY = 0;
		var previous = null;
		var previousStyle = null;
		var currentStyle = null;
		for (var parent = element; parent; previous = parent, previousStyle = currentStyle, parent = parent.offsetParent) {
			var tagName = parent.tagName ? parent.tagName.toUpperCase() : null;
			currentStyle = Sys.UI.DomElement._getCurrentStyle(parent);
			if ((parent.offsetLeft || parent.offsetTop) &&
						!((tagName === "BODY") &&
							(!previousStyle || previousStyle.position !== "absolute"))) {
				offsetX += parent.offsetLeft;
				offsetY += parent.offsetTop;
			}
			if (previous !== null && currentStyle) {
				if ((tagName !== "TABLE") && (tagName !== "TD") && (tagName !== "HTML")) {
					offsetX += parseInt(currentStyle.borderLeftWidth) || 0;
					offsetY += parseInt(currentStyle.borderTopWidth) || 0;
				}
				if (tagName === "TABLE" &&
							(currentStyle.position === "relative" || currentStyle.position === "absolute")) {
					offsetX += parseInt(currentStyle.marginLeft) || 0;
					offsetY += parseInt(currentStyle.marginTop) || 0;
				}
			}
		}
		currentStyle = Sys.UI.DomElement._getCurrentStyle(element);
		var elementPosition = currentStyle ? currentStyle.position : null;
		if (!elementPosition || (elementPosition !== "absolute")) {
			for (var parent = element.parentNode; parent; parent = parent.parentNode) {
				tagName = parent.tagName ? parent.tagName.toUpperCase() : null;
				if ((tagName !== "BODY") && (tagName !== "HTML") && (parent.scrollLeft || parent.scrollTop)) {
					offsetX -= (parent.scrollLeft || 0);
					offsetY -= (parent.scrollTop || 0);
					currentStyle = Sys.UI.DomElement._getCurrentStyle(parent);
					if (currentStyle) {
						offsetX += parseInt(currentStyle.borderLeftWidth) || 0;
						offsetY += parseInt(currentStyle.borderTopWidth) || 0;
					}
				}
			}
		}
		return new Sys.UI.Point(offsetX, offsetY);
	};
}

window.Application = function () {
	var urlBuilder = null;
	require(['tau/configurator'], function(configurator) {
		urlBuilder = configurator.getUrlBuilder();
	});
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
			return urlBuilder.getApplicationPath() + '/' + urlBuilder.getBoardPageRelativePath() + urlBuilder.getNewViewUrl(entityId, entityKind);
		},

		getShortViewUrl: function (entityId) {
			return urlBuilder.getShortViewUrl({id: entityId});
		}
	});
	return new type();
} ();
