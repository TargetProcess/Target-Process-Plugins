Ext.ns('Tp.controls.Lookup');

Tp.controls.Lookup.WebServiceReader = Ext.extend(Ext.data.JsonReader, {
	read: function (response) {
		var json = response.responseText;
		var o = Ext.decode(json);
		if (!o) {
			throw { message: 'WebServiceReader.read: Json object not found' };
		}
		return this.readRecords(o.d);
	}
});
