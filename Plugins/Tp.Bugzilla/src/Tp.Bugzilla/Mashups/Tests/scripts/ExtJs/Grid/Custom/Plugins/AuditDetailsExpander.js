
Ext.ns('Tp.custom.plugins');
Tp.custom.plugins.AuditDetailsExpander = Ext.extend(Tp.controls.grid.plugins.BunchRowsExpander, {

	_expandDivId: "hd_{ID}_{Entity}_{InnerEntity}",

	constructor: function () {
		Tp.custom.plugins.AuditDetailsExpander.superclass.constructor.call(this, { tpl: this.getExpandTemplate(), enableCaching: false });
		this.on("expand", this.onExpand, this);
	},

	renderer: function (v, p, record) {
		if (record.data.Modification == 2)
			return "&nbsp;";

		return Tp.custom.plugins.AuditDetailsExpander.superclass.renderer.call(this, v, p, record);
	},

	getExpandTemplate: function () {
		return new Ext.Template('<div style="padding-left:8px;" class=" wrap" id="' + this._expandDivId + '"/>');
	},

	getDivIdTemplate: function () {
		return new Ext.Template(this._expandDivId);
	},

	onExpand: function (sender, record, body, rowIndex) {
		var div = Ext.get(this.getDivIdTemplate().apply(record.data));
		div.update('Loading details...');

		var entity = record.data.Entity;
		if (record.data.InnerEntity != null && record.data.InnerEntity != '')
			entity = record.data.InnerEntity;

		var handler = Function.createDelegate(this, this.onCompleteLoadingDetails);
		Tp.Web.Extensions.ExtJs.AuditHistoryListService.GetChanges(record.data.ID, entity, handler, null, div);
	},

	onCompleteLoadingDetails: function (changes, div) {
		var html = "";

		var templateChange = new Ext.Template('<b>{Field}</b> changed from <span style="color:red;">{OldValue}</span> to <span style="color:green;">{NewValue}</span><br/>');
		var templateInitialize = new Ext.Template('<b>{Field}</b>: <span style="color:green;">{NewValue}</span><br/>');

		for (var i = 0; i < changes.length; i++) {

			var template = templateChange;

			if (changes[i].OldValue == null || changes[i].OldValue == '' || changes[i].OldValue == 'Empty') {
				var template = templateInitialize;
				changes[i].OldValue = "Empty";
			}

			if (changes[i].NewValue == null)
				changes[i].NewValue = "Empty";

			changes[i].OldValue = this.formatValue(changes[i].OldValue, changes[i].ChangeID, changes[i].NativeField);
			changes[i].NewValue = this.formatValue(changes[i].NewValue, changes[i].ChangeID, changes[i].NativeField);

			html += template.apply(changes[i]);
		}

		if (changes.length == 0)
			html = 'no any changes...';

		div.update(html);
	},

	formatValue: function (value, changeID, nativeField) {
		if (value && typeof (value.toLocalDate) == 'function')
			value = value.format('d-M-Y');

		if (value != null && value.toString() == "true")
			return "Yes";

		if (value != null && value.toString() == "false")
			return "No";

		if (nativeField == 'Description' && changeID.indexOf('Attachment-Description') == -1)
			return value;

		if (changeID.indexOf('TestCase-Success') > 0 || changeID.indexOf('TestCase-Steps') > 0)
			return value;

		if (changeID.indexOf('Request-SourceType') > 0) {

			if (value == 1)
				return "Email";

			if (value == 2)
				return "Phone";

			if (value == 3)
				return "Internal";

			if (value == 4)
				return "External";
		}
		return Ext.util.Format.htmlEncode(value);
	}
})