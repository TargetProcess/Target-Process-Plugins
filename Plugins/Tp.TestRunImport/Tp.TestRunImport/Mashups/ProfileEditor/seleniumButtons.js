tau.mashups
	.addDependency("libs/jquery/jquery")
	.addModule("TestRunImport/seleniumButtons", function () {
		function seleniumButtons(config) {
			this._create(config);
		}

		seleniumButtons.prototype = {
			rendered: null,
			active: null,
			seleniumButtonsTemplate:
				'<div class="button-group" style="display: none;" >' +
					'	<a href="javascript:void(0)" id="path" class="button">Result File</a>' +
						'	<a href="javascript:void(0)" id="url" class="button">Postback results URL</a>' +
							'</div>',

			_create: function (config) {
				this.active = config.active;
			},

			render: function () {
				var r = this.rendered = $.tmpl(this.seleniumButtonsTemplate, {});
				r.find('a#' + this.active) != null ? r.find('a#' + this.active).addClass('active') : r.find('a#path').addClass('active');
				this.rendered.find('a#path').click($.proxy(function (e) {
					r.find('a#url').removeClass('active');
					$(e.target).addClass('active');
					this.active = $(e.target).id();
					r.trigger('editorchange', [this]);
				}, this));
				this.rendered.find('a#url').click($.proxy(function (e) {
					r.find('a#path').removeClass('active');
					$(e.target).addClass('active');
					this.active = $(e.target).id();
					r.trigger('editorchange', [this]);
				}, this));
			},

			isPathActive: function () {
				return this.active == 'path';
			},

			isUrlActive: function () {
				return this.active == 'url';
			}
		};
		return seleniumButtons;
	});