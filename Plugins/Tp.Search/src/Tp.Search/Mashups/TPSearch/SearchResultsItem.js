tau.mashups
.addDependency("tau/mashups/TPSearch/Commands")
.addDependency("tau/mashups/TPSearch/SearchResultsItemTmpl")
.addDependency("tau/mashups/TPSearch/jquery/highlight")
.addModule("tau/mashups/TPSearch/SearchResultsItem", function (cmd, tmpl) {
	function searchResultsItem(id, keywords, getEntityCommand) {
		this._entityID = id;
		this._keywords = keywords;
		this.element = $('<div>Loading...</div>');
		getEntityCommand(this._entityID, $.proxy(this.entityLoaded, this));
	}

	searchResultsItem.prototype = {
		_keywords: [],
		_entityID: 0,
		element: null,

		getElement: function () {
			return this.element;
		},

		entityLoaded: function (data) {
			this.prepareData(data);
			this.element.empty();
			$.tmpl(tmpl, data).appendTo(this.element);
		},

		prepareData: function (data) {
			data.Description = this.shortenAndHighlight(data.Description);
			var generalName = this.shortenAndHighlight(data.General ? data.General.Name : data.Name);
			if (generalName.length !== 0) {
				data.Name = generalName;
			}
		},

		shortenAndHighlight: function (originText) {
			if (!originText || originText.length == 0) return '';

			originText = originText.replace(/<[img|IMG][^<]+?>/gi, '');
			var newText = $('<div>{text}</div>'.replace(/{text}/g, originText));

			$.each(this._keywords, function (i, val) {
				newText.highlight(val);
			});

			var count = 0;
			newText.contents().filter(function () {
				var isText = this.nodeType == Node.TEXT_NODE;
				if (isText == false) {
					count = count + 1;
				};
				return isText;
			}).each(
				function (i, val) {
					var x = 30;
					if (val) {
						var txt = val.textContent.replace(/\s+/g, ' ');
						if (txt.length > 2 * x) {
							var start = txt.substring(0, x);
							var end = txt.substring(txt.length - x, txt.length);
							val.textContent = start + ' ... ' + end;
						}
					}
				}
			);

			if (count == 0) {
				return '';
			}
			return newText.html();
		},

		show: function () {
		},

		hide: function () {
		}
	};

	return searchResultsItem;
});