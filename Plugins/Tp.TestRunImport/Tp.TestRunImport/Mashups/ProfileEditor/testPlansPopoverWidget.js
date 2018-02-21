tau.mashups
	.addDependency("libs/jquery/jquery.ui")
	.addDependency("libs/jquery/jquery.tmpl")
	.addModule("TestRunImport/testPlansPopoverWidget", function () {
		function testPlansPopoverWidget(config) {
			this._create(config);
		}

		testPlansPopoverWidget.prototype = {
			tpUserPopoverTemplate: '<li class="tau-autocomplete__item" style="padding: 5px;"><div><a>#${id} - ${name}</a></div></li>',

			_create: function (config) {
				var that = this;
				that.config = config;
				var projectId = config.projectId;

				config.elements.each(function () {
					var element = $(this);
					element.autocomplete({
						minLength: 0,
						source: function (request, response) {
							$.get(new Tp.WebServiceURL('/api/v2/TestPlan?select={id,name}&where=Project.Id%20=%20{ProjectId}%20and%20name.Contains(%27{SearchTerm}%27)&orderBy=name%20asc&take=1000'.replace(/{ProjectId}/g, projectId).replace(/{SearchTerm}/g, request.term)).url,
							function (data) {
								response($.map(data.items, function (testplan) {
									return { value: testplan.name, data: testplan };
								}));
							});
						},
						delay: config.delay || 300,
						focus: function () {
							return false;
						},
						select: function (event, ui) {
							var data = ui.item.data;
							$(this).attr('testplanid', data.id).val(data.name).blur();
							return false;
						}
					});

					element.data('ui-autocomplete')._renderItem = function (ul, item) {
						return $.tmpl(that.tpUserPopoverTemplate, item.data).data('ui-autocomplete-item', item).appendTo(ul);
					};

					element.bind('focus', that._focus);
				});
			},

			_focus: function() {
				$(this).autocomplete('search');
			},

			destroy: function() {
				var that = this;

				this.config.elements.each(function () {
					var element = $(this);
					element.off('focus', that._focus);
					if (element.data('ui-autocomplete')) {
						element.autocomplete('destroy');
					}
				});
			}
		};
		return testPlansPopoverWidget;
	});
