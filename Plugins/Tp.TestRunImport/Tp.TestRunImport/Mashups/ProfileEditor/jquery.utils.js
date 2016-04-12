tau.mashups
	.addDependency("libs/jquery/jquery")
	.addModule("TestRunImport/jquery.utils", function () {
		$.fn.enabled = function (enableIt) {
			var aEnabler = {
				getEnable: function (element) {
					return !element.hasClass("disabled");
				},

				setEnabled: function (element, value) {
					if (value) {
						element.removeClass("disabled");
					}
					else {
						element.addClass("disabled");
					}
				}
			};

			var defaultEnabler = {
				getEnable: function (element) {
					return element.is(":enabled");
				},

				setEnabled: function (element, value) {
					if (value) {
						element.removeAttr("disabled");
					}
					else {
						element.attr("disabled", "disabled");
					}
				}
			};

			var enabler = defaultEnabler;
			if ($(this).is('a')) {
				enabler = aEnabler;
			}

			if (enableIt != null) {
				enabler.setEnabled($(this), enableIt);
			}
			return enabler.getEnable($(this));
		};

		$.fn.serializeObject = function () {
			var o = {};
			var a = this.serializeArray();
			$.each(a, function () {
				if (o[this.name] !== undefined) {
					if (!o[this.name].push) {
						o[this.name] = [o[this.name]];
					}
					o[this.name].push(this.value || '');
				} else {
					o[this.name] = this.value || '';
				}
			});
			return o;
		};
	});
