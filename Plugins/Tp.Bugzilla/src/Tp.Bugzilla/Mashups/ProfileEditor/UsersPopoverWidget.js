tau.mashups
	.addDependency("libs/jquery/jquery.ui")
	.addDependency("libs/jquery/jquery.tmpl")
	.addModule("Bugzilla/UsersPopoverWidget", function () {
	    function tpUsersPopoverWidget(config) {
	        this._create(config);
	    }

	    tpUsersPopoverWidget.prototype = {
	        tpUserPopoverTemplate: '<li class="tau-autocomplete__item"><a>' +
                '<div class="user">' +
                '<input type="hidden" class="id" value="${Id}" /><input type="hidden" class="login" value="${Login}" />' +
                '<div class="avatar"><img src="${Avatar}" style="border-width: 0px;" /></div>' +
                '<div><strong class="name">${Name}</strong><br /><span class="role" style="font-size: 90%; color: #999;">${Role}</span></div>' +
                '</div>' +
                '</a></li>',

	        _create: function (config) {
	            var that = this;
	            var source = config.source;

	            config.elements.each(
				function () {
				    var element = $(this);
				    element.autocomplete({
				        minLength: 0,
				        delay: config.delay || 300,
				        source: $.map(source, function (user) {
				            return { value: user.Name, data: user };
				        }),
				        focus: function () {
				            return false;
				        },
				        select: function (event, ui) {
				            var data = ui.item.data;
				            $(this).attr('userId', data.Id).val(data.Name);
				            return false;
				        }
				    });

				    element.data('ui-autocomplete')._renderItem = function (ul, item) {
				        return $.tmpl(that.tpUserPopoverTemplate, item.data)
                            .data('ui-autocomplete-item', item)
                            .appendTo(ul);
				    };
				});
	        }
	    };
	    return tpUsersPopoverWidget;
	});
