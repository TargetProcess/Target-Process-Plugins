if (window["Range"]) {
	if (typeof Range.prototype.createContextualFragment == "undefined") {
		Range.prototype.createContextualFragment = function (html) {
			var doc = this.startContainer.ownerDocument;
			var container = doc.createElement("div");
			container.innerHTML = html;
			var frag = doc.createDocumentFragment(), n;
			while ((n = container.firstChild)) {
				frag.appendChild(n);
			}
			return frag;
		};
	}
}

Ext.override(Ext.Element, {
	getAttribute: (Ext.isIE && (Ext.isIE7 || Ext.isIE8) && document.documentMode !== 9) ?
		function (name, ns) {
			var d = this.dom, type;
			if (ns) {
				type = typeof d[ns + ":" + name];
				if (type != 'undefined' && type != 'unknown') {
					return d[ns + ":" + name] || null;
				}
				return null;
			}
			if (name === "for") {
				name = "htmlFor";
			}
			return d[name] || null;
		}
		: function (name, ns) {

			var d = this.dom;
			if (ns) {
				return d.getAttributeNS(ns, name) || d.getAttribute(ns + ":" + name);
			}
			return d.getAttribute(name) || d[name] || null;
		}
});

Ext.override(Ext.grid.GridPanel, {
	processEvent: function(name, e) {
		var t = e.getTarget();

		if (!t) {
			return;
		}

		if (!this.el) {
			return;
		}


		if (Ext.fly(this.el.id).query('.x-grid3').length > 1 && Ext.fly(t).parent('.x-grid3') != null) {
			return;
		}

		this.fireEvent(name, e);

		var v = this.view;
		var header = v.findHeaderIndex(t);

		if (header !== false) {
			this.fireEvent("header" + name, this, header, e);
		} else {
			var row = v.findRowIndex(t);
			var cell = v.findCellIndex(t);
			if (row !== false) {
				this.fireEvent("row" + name, this, row, e);
				if (cell !== false) {
					this.fireEvent("cell" + name, this, row, cell, e);
				}
			}
		}
	}
});

Ext.override(Ext.grid.ColumnModel, {
	moveColumn: function(oldIndex, newIndex) {
		if (this.isMovable(oldIndex)) {
			var c = this.config[oldIndex];
			this.config.splice(oldIndex, 1);
			this.config.splice(newIndex, 0, c);
			this.dataMap = null;
			this.fireEvent("columnmoved", this, oldIndex, newIndex);
		}
	},

	isMovable: function(col) {
		if (typeof this.config[col].movable == "undefined") {
			return this.enableColumnMove || true;
		}

		return this.config[col].movable;
	}
});

Ext.override(Ext.EventObjectImpl, {
	getTarget: function(selector, maxDepth, returnEl) {
		var targetElement;
		try {
			targetElement = selector ? Ext.fly(this.target).findParent(selector, maxDepth, returnEl) : (returnEl ? Ext.get(this.target) : this.target);
		} catch (e) {
			targetElement = this.target;
		}

		return targetElement;
	}
});

Ext.override(Ext.form.HtmlEditor, {

	getWin: function() {
		//FIX HTML EDITOR FOR CHROME
		if ((Ext.isChrome || Ext.isSafari) && !window.frames[this.iframe.name]) {
			for (var i = 0; i < window.frames.length; i++) {
				if (window.frames[i].name == "ux-lightbox-shim")
					return window.frames[i];
			}
		}

		if (Ext.isIE || Ext.isGecko) {
			var frames = document.getElementsByTagName("iframe");
			for (var i = 0; i < frames.length; i++) {
				if (frames[i] == this.iframe)
					return frames[i].contentWindow;
			}
			for (var i = 0; i < frames.length; i++) {
				if (frames[i].name == "") {
					return frames[i].contentWindow;
				}
			}
			alert("The frame is not found for HtmlEditor");
		}
		return Ext.isIE ? this.iframe.contentWindow : window.frames[this.iframe.name];
	}
});
