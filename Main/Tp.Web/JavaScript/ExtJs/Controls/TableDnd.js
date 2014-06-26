Ext.ns("Tp.controls");

Tp.controls.TableDnd = Ext.extend(Object, {
	/** Keep hold of the current table being dragged */
	currentTable: null,
	/** Keep hold of the current drag object if any */
	dragObject: null,
	/** The current mouse offset */
	mouseOffset: null,
	/** Remember the old value of Y so that we don't do too much processing */
	oldY: 0,

	/** Actually build the structure */
	constructor: function(options) {
		options = options || {};
		if (options.tableId == null) {
			throw "tableId"
		}
		var tableEl = Ext.get(options.tableId);
		if (tableEl == null) {
			return;
		}
		var table = tableEl.dom;
		table.tableDnDConfig = {
			onDragStyle: options.onDragStyle,
			onDropStyle: options.onDropStyle,
			// Add in the default class for whileDragging
			onDragClass: options.onDragClass ? options.onDragClass : "tDnD_whileDrag",
			onDrop: options.onDrop,
			requiredAttribute: options.requiredAttribute,
			onDragStart: options.onDragStart,
			scrollAmount: options.scrollAmount ? options.scrollAmount : 5
		};
		Tp.controls.TableDnd.makeDraggable(table);
		Ext.getBody().on('mousemove', Tp.controls.TableDnd.mousemove);
		Ext.getBody().on('mouseup', Tp.controls.TableDnd.mouseup);
	}
});

Tp.controls.TableDnd.makeDraggable = function (table) {
	// Now initialise the rows
	var rows = table.rows; //getElementsByTagName("tr")
	var config = table.tableDnDConfig;
	for (var i = 0; i < rows.length; i++) {
		// John Tarr: added to ignore rows that I've added the NoDnD attribute to (Category and Header rows)
		var nodrag = rows[i].getAttribute("NoDrag");
		var requiredAttribute = config.requiredAttribute;

		var isRequiredAttributeDefined = true;

		if (requiredAttribute != null && requiredAttribute != 'undefined')
			isRequiredAttributeDefined = rows[i].getAttribute(requiredAttribute) != null;

		if ((nodrag == null || nodrag == "undefined") && isRequiredAttributeDefined) { //There is no NoDnD attribute on rows I want to drag
			var row = Ext.get(rows[i]);

			row.on('mousedown', function (ev) {
				if (ev.target.tagName != "A" && ev.target.tagName != "IMG") {
					Tp.controls.TableDnd.dragObject = this.dom;
					Tp.controls.TableDnd.currentTable = table;
					Tp.controls.TableDnd.mouseOffset = Tp.controls.TableDnd.getMouseOffset(this, ev);
					if (config.onDragStart) {
						// Call the onDrop method if there is one
						config.onDragStart(table, this.dom);
					}
					return false;
				}
			});

			row.setStyle('cursor', 'move');
			if (typeof row.dom.onselectstart != "undefined") //IE route
				row.dom.onselectstart = function () { return false; }
			else if (typeof row.dom.style.MozUserSelect != "undefined") //Firefox route
				row.dom.style.MozUserSelect = "none";
		}
	}
}

Tp.controls.TableDnd.mousemove = function(ev) {
	if (Tp.controls.TableDnd.dragObject == null) {
		return;
	}
	//console.log("Drag object oldy %s", Tp.controls.TableDnd.oldY);
	var dragObj = Ext.get(Tp.controls.TableDnd.dragObject);
	var config = Tp.controls.TableDnd.currentTable.tableDnDConfig;
	var mousePos = { x: ev.xy[0], y: ev.xy[1] };
	var y = mousePos.y - Tp.controls.TableDnd.mouseOffset.y;
	//auto scroll the window
	var yOffset = window.pageYOffset || document.documentElement.scrollTop;
	if (mousePos.y - yOffset < config.scrollAmount) {
		window.scrollBy(0, -config.scrollAmount);
	} else {
		var windowHeight = Tp.util.getClientHeight();
        
		if (windowHeight - (mousePos.y - yOffset) < config.scrollAmount) {
		    window.scrollBy(0, config.scrollAmount);
		}
	}
	//console.log("Drag object y %s", y);
	if (y != Tp.controls.TableDnd.oldY) {
		// work out if we're going up or down...
		var movingDown = y > Tp.controls.TableDnd.oldY;

		// update the old value
		Tp.controls.TableDnd.oldY = y;
		// update the style to show we're dragging
		if (config.onDragClass) {
			dragObj.addClass(config.onDragClass);
		} else {
			dragObj.css(config.onDragStyle);
		}
		// If we're over a row then move the dragged row to there so that the user sees the
		// effect dynamically
		var currentRow = Tp.controls.TableDnd.findDropTargetRow(y);
		var rows = Tp.controls.TableDnd.currentTable.rows;
		if (currentRow) {
			if (movingDown && Tp.controls.TableDnd.dragObject != currentRow) {
				Tp.controls.TableDnd.dragObject.parentNode.insertBefore(Tp.controls.TableDnd.dragObject, currentRow.nextSibling);
			} else if (!movingDown && Tp.controls.TableDnd.dragObject != currentRow) {
				Tp.controls.TableDnd.dragObject.parentNode.insertBefore(Tp.controls.TableDnd.dragObject, currentRow);
			}
		}
	}
	return false;
}

Tp.controls.TableDnd.mouseup = function(e) {
	if (Tp.controls.TableDnd.currentTable && Tp.controls.TableDnd.dragObject) {
		var droppedRow = Tp.controls.TableDnd.dragObject;
		var config = Tp.controls.TableDnd.currentTable.tableDnDConfig;
		// If we have a dragObject, then we need to release it,
		// The row will already have been moved to the right place so we just reset stuff
		if (config.onDragClass) {
			Ext.fly(droppedRow).removeClass(config.onDragClass);
		} else {
			Ext.fly(droppedRow).css(config.onDropStyle);
		}
		Tp.controls.TableDnd.dragObject = null;
		if (config.onDrop) {
			// Call the onDrop method if there is one
			config.onDrop(Tp.controls.TableDnd.currentTable, droppedRow);
		}
		Tp.controls.TableDnd.currentTable = null; // let go of the table too
	}
}

Tp.controls.TableDnd.getMouseOffset = function(target, ev) {
	ev = ev || window.event;
	var docPos = Tp.controls.TableDnd.getPosition(target.dom);
	var mousePos = Tp.controls.TableDnd.mouseCoords(ev);
	return { x: mousePos.x - docPos.x, y: mousePos.y - docPos.y };
}

Tp.controls.TableDnd.getPosition = function(e) {
	var left = 0;
	var top = 0;
	/** Safari fix -- thanks to Luis Chato for this! */
	if (e.offsetHeight == 0) {
		/** Safari 2 doesn't correctly grab the offsetTop of a table row
		this is detailed here:
		http://jacob.peargrove.com/blog/2006/technical/table-row-offsettop-bug-in-safari/
		the solution is likewise noted there, grab the offset of a table cell in the row - the firstChild.
		note that firefox will return a text node as a first child, so designing a more thorough
		solution may need to take that into account, for now this seems to work in firefox, safari, ie */
		e = e.firstChild; // a table cell
	}

	while (e.offsetParent) {
		left += e.offsetLeft;
		top += e.offsetTop;
		e = e.offsetParent;
	}

	left += e.offsetLeft;
	top += e.offsetTop;

	return { x: left, y: top };
}

Tp.controls.TableDnd.mouseCoords = function(ev) {
    return {
        x: ev.getPageX() - document.body.clientLeft,
        y: ev.getPageY() - document.body.clientTop
	};
}

Tp.controls.TableDnd.findDropTargetRow = function(y) {
	var rows = Tp.controls.TableDnd.currentTable.rows;
	for (var i = 0; i < rows.length; i++) {
		var row = rows[i];
		// John Tarr added to ignore rows that I've added the NoDnD attribute to (Header rows)
		var nodrop = row.getAttribute("NoDrop");
		if (nodrop == null || nodrop == "undefined") {  //There is no NoDnD attribute on rows I want to drag
			var rowY = this.getPosition(row).y;
			var rowHeight = parseInt(row.offsetHeight) / 2;
			if (row.offsetHeight == 0) {
				rowY = Tp.controls.TableDnd.getPosition(row.firstChild).y;
				rowHeight = parseInt(row.firstChild.offsetHeight) / 2;
			}
			// Because we always have to insert before, we need to offset the height a bit
			if ((y > rowY - rowHeight) && (y < (rowY + rowHeight))) {
				// that's the row we're over
				return row;
			}
		}
	}
	return null;
}
