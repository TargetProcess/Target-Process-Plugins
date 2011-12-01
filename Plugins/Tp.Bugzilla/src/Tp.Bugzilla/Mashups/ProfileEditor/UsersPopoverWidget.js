tau.mashups
	.addDependency("libs/jquery/jquery.ui")
	.addDependency("libs/jquery/jquery.tmpl")
	.addModule("Bugzilla/UsersPopoverWidget", function () {
		function tpUsersPopoverWidget(config) {
			this._create(config);
		}

		tpUsersPopoverWidget.prototype = {
			tpUserPopoverTemplate: '<a href==""><ul class="user">\n    <input type="hidden" class="id" value="${Id}" />\n    <li class="avatar">\n        <img src="" style="border-width: 0px;">\n    </li>\n    <li>\n        <strong class="name">${Name}</strong>\n        <br> \n        <span class="role" style="font-size: 90%; color: #999;" >${Role}</span>\n    </li>\n</ul>\n</a>',

			_create: function (config) {

				var source = config.source;

				config.elements.each(
				function () {

					$(this).autocomplete({
						minLength: 0,
						delay: config.delay || 300,
						source: $.map(source, function (user) {
							return { value: user.Name, data: user };
						}),
						focus: function () {
							return false;
						},
						select: function (event, ui) {
							$(this).attr('userId', ui.item.data.Id);
							$(this).val(ui.item.data.Name);
							return false;
						}
					}).data('autocomplete')._renderItem = function (ul, item) {
						return $("<li></li>")
								.data('item.autocomplete', item)
								.append($.tmpl(tpUsersPopoverWidget.prototype.tpUserPopoverTemplate, item.data))
								.appendTo(ul)
								.find('img').attr('src', item.data.Avatar);
					};
				});
			}
		};
		return tpUsersPopoverWidget;
	});
