Ext.ns('Tp.controls.grid');

Tp.controls.grid.TestCaseFieldFactory = Ext.extend(Object, {
	createEditor: function () {
		return null;
	},

	statusRenderer: function (value) {
		if (value === true) {
			return "<span style='color:Green'>Passed</span>"
		}
		else if (value === false) {
			return "<span style='color:Red'>Failed</span>"
		}
		return null;
	},

	createRenderer: function (columnConfig) {
		if (columnConfig.Id == "LastStatus") {
			return this.statusRenderer;
		}

		if (columnConfig.Type == "date") {
			return Ext.util.Format.dateRenderer('d-M-Y H:i');
		}

		if (columnConfig.Facet.entityKind) {
			return entityIconIdRenderer(columnConfig.Facet.entityKind);
		}
		if (columnConfig.Facet.edit) {
			return testCaseEditRenderer();
		}

		if (columnConfig.Type == "HTMLText") {
			return Ext.util.Format.htmlEncode;
		}

		return null;
	}
});

function testCaseEditRenderer() {
	return function (value) {
		return String.format("<a href='{0}/Project/QA/TestCase/Edit.aspx?TestCaseID={1}'>Edit</a>", Application.baseUrl, value);
	};
}
