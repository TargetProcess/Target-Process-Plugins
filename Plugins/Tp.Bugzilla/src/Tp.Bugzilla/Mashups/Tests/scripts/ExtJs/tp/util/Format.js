Ext.ns('ExtJs.tp.util.Format');

ExtJs.tp.util.Format.AssignableIDLink = function(value, x, store) {
	return String.format(
			"<a href='{0}'>{1}</a>",
			Application.getViewUrl(value, store.data.entityKind), value);
};

ExtJs.tp.util.Format.TauIDLink = function (value, x, store) {
	return String.format("<a class='tau-target-link'>{0}</a>", value);
};


ExtJs.tp.util.Format.EntityType = function(value) {
	return String.format("<img class='x-item-icon' src='{0}/{1}' />", Application.baseUrl, value);
};

ExtJs.tp.util.Format.HtmlEncode = function(value) {
	return Ext.util.Format.htmlEncode(value);
};

ExtJs.tp.util.Format.ProjectAbbr = function (value) {
	value = ExtJs.tp.util.Format.HtmlEncode(value);
	value = String(value).replace("[[", "<span class='projectAbbrList'>");
	value = String(value).replace("]]", "</span>");
	return value;
};
