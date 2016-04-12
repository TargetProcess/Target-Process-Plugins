Ext.ns('Tp.controls');

Tp.controls.Prioritization = Ext.extend(Object, {
	constructor: function(tableId, rowAttribute, postBackID, postBackRef, dropRowLast) {
		var initialRowIndex;
		function replaceLastRowByIndex(table, index) {
			var tbody = table.getElementsByTagName("TBODY")[0];
			var rows = new Array();
			while (table.rows.length != 0) {
				rows.push(tbody.removeChild(table.rows[0]));
			}
			for (var i = 0; i < rows.length - 1; i++) {
				if (i == index)
					tbody.appendChild(rows[rows.length - 1]);
				tbody.appendChild(rows[i]);
			}
		}

		var config = {
			requiredAttribute: rowAttribute,
			onDragStart: function(table, row) {
				initialRowIndex = row.rowIndex;
			},
			onDrop: function(table, row) {
				if (!row.attributes[rowAttribute])
					return;
				if (initialRowIndex == row.rowIndex)
					return;

				if (!dropRowLast && row.rowIndex == table.rows.length - 1) {
					replaceLastRowByIndex(table, initialRowIndex);
					return;
				}
				var previousId = 0;
				var nextId = 0;
				var params = '';
				var currentProjectId = row.attributes[rowAttribute].value;
				for (var i = 0; i < table.rows.length; i++) {
					if (table.rows[i].attributes[rowAttribute]) {
						if (table[row.rowIndex] == row)
							return;
						var projectId = table.rows[i].attributes[rowAttribute].value;
						if (projectId == currentProjectId) {
							if (i > 1 && table.rows[i - 1].attributes[rowAttribute])
								previousId = table.rows[i - 1].attributes[rowAttribute].value;
							if (i < table.rows.length - 1 && table.rows[i + 1].attributes[rowAttribute])
								nextId = table.rows[i + 1].attributes[rowAttribute].value;
							break;
						}
					}
				}

				if (postBackRef) {
					var order = [previousId, currentProjectId, nextId];
					postBackRef(order);
				}
				else {
					var params = 'REORDER:[' + previousId + ',' + currentProjectId + ',' + nextId + ']';
					__doPostBack(postBackID, params);
				}
			}
		};
		config.tableId = tableId;
		Tp.controls.TableDnd(config);
	}
});
 