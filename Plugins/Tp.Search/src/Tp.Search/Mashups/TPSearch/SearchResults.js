tau.mashups
.addDependency("libs/jquery/jquery.ui")
.addDependency("tau/mashups/TPSearch/Commands")
.addDependency("tau/mashups/TPSearch/SearchResultsItem")
.addDependency("tau/mashups/TPSearch/SearchResultsTmpl")
.addModule("tau/mashups/TPSearch/SearchResults", function (ui, commands, searchResultsItem, tmpl) {
	function searchResults() {
		this._dialog = $(tmpl);
		$('body').append(this._dialog);
		this._dialog.dialog({
			closeOnEscape: true,
			autoOpen: false,
			height: 450,
			width: 900,
			buttons: [{
				click: function (ev) {
					$(ev.currentTarget).addClass('ui-state-disabled').attr('disabled', 'disabled');
					$(this).trigger('prev');
				},
				disabled: true,
				text: 'Prev'
			}, {
				click: function (ev) {
					$(ev.currentTarget).addClass('ui-state-disabled').attr('disabled', 'disabled');
					$(this).trigger('next');
				},
				disabled: true,
				text: 'Next'
			}],
			create: function () {
				$(this).nextAll('div.ui-dialog-buttonpane').children('div.ui-dialog-buttonset').prepend('<span style="font-size: 1.1em; margin: 0.5em 0.4em 0.5em 0;"></span>');
			}
		});

		this._dialog.bind('prev', $.proxy(function () {
			this._page--;
			this._doSearch();
		}, this));

		this._dialog.bind('next', $.proxy(function () {
			this._page++;
			this._doSearch();
		}, this));
	}

	searchResults.prototype = {
		_dialog: null,
		_searchData: null,
		_pageSize: 20,
		_page: 0,

		Add: function (item, parent) {
			//parent.append(item.getElement());
			this._dialog.append(item.getElement());
			this._dialog.dialog('open');
		},

		Clear: function () {
			this._dialog.children().remove();
		},

		setPageSize: function (pageSize) {
			this._pageSize = pageSize;
		},

		search: function (searchData, el) {
			this.alignToEl = el;
			this._searchData = searchData;
			this._searchData.PageSize = this._pageSize;
			this._page = 0;
			this._doSearch();
		},

		_doSearch: function () {
			this._searchData.Page = this._page;
			commands.search(this._searchData, $.proxy(this.searchSuccess, this), $.proxy(this.searchFail, this));
		},

		getKeywords: function (text) {
			var validKeywords = [];
			// see http://kourge.net/projects/regexp-unicode-block
			var keywords = text.replace(/[~`!@#$%^&*()+=\-\[\]{}'"\\|\/,\.?<>]+/g, " ").split(' ');
			$.each(keywords, function (i, v) {
				if (v) {
					console.log(v);
					validKeywords.push(v);
				}
			});
			return validKeywords;
		},

		searchSuccess: function (res) {
			var totalPages = Math.ceil(res.Total / this._pageSize);
			if (this._page > 0 && res.Total > this._pageSize) {
				$(this._dialog)
					.nextAll('div.ui-dialog-buttonpane').children('div.ui-dialog-buttonset').children('button:first').removeAttr('disabled').removeClass('ui-state-disabled');
			}
			if (totalPages > this._page + 1 && res.Total > this._pageSize) {
				$(this._dialog)
					.nextAll('div.ui-dialog-buttonpane').children('div.ui-dialog-buttonset').children('button:last').removeAttr('disabled').removeClass('ui-state-disabled');
			}
			$(this._dialog)
				.nextAll('div.ui-dialog-buttonpane')
				.children('div.ui-dialog-buttonset')
				.children('span').text('Page {Page} of {PagesTotal}. Displaying {ItemStart} - {ItemEnd} of {ItemsTotal}'
						.replace(/{Page}/g, this._page + 1)
						.replace(/{PagesTotal}/g, totalPages)
						.replace(/{ItemStart}/g, this._page * this._pageSize + 1)
						.replace(/{ItemEnd}/g, this._page * this._pageSize + res.Entities.length + res.Comments.length)
						.replace(/{ItemsTotal}/g, res.Total));
			this.Clear();
			var self = this;
			/*var q = $(self.alignToEl);

			if (q.next('.tau-bubble').length <= 0) {
				q.after('<div class=\'tau-bubble i-orientaion_top i-state_visible\' style=\'z-index: 1; display: block;\'>' +
								'<div class=\'tau-bubble__arrow\' data-orientation=\'top\' style=\'top: 0px; left: 22px;\'></div>' +
								'<div class=\'tau-bubble__inner\'></div></div>');
				var b = q.next('.tau-bubble'), p = q.position();
				b.css('top', p.top + q.height() + 'px').css('left', p.left + 'px');
			}
			q.next('.tau-bubble').css('visibility', 'visible').animate({ opacity: 1 }, 300);
			var inner = $(q.next('.tau-bubble')).children('.tau-bubble__inner');*/

			var keywords = this.getKeywords(res.SearchString);
			$.each(res.Entities, function (i, val) {
				self.Add(new searchResultsItem(val, keywords, commands.getEntity), null/*inner*/);
			});
			$.each(res.Comments, function (i, val) {
				self.Add(new searchResultsItem(val, keywords, commands.getComment), null /*inner*/);
			});
		},

		searchFail: function () {
		}
	};
	return searchResults;
});