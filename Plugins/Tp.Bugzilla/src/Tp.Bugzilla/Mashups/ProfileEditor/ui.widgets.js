tau.mashups
	.addDependency("libs/jquery/jquery")
	.addDependency("libs/jquery/jquery.ui")
	.addDependency("libs/jquery/jquery.tmpl")
	.addDependency("Bugzilla/jquery.utils")
	.addModule("Bugzilla/ui.widgets", function () {

		$.widget("ui.error", {
			messageTemplate: '<span class="error">${message}</span>',

			_create: function () {
				this.element.focus($.proxy(this.clear, this));

				this.element.addClass('ui-error');
				this.element.addClass('error');
				this.errorSpan = $.tmpl(this.messageTemplate, this.options);
				this.element.prev('p').append(this.errorSpan);
			},

			clear: function () {
				this.errorSpan.remove();
				this.element.removeClass('error');
				this.element.removeClass('ui-error');
				this.destroy();
			}
		});

		$.widget("ui.synchronizeUser", {
			_create: function () {
				this._synchronizeId();
				this._synchronizeName();
				this.element.blur($.proxy(this._synchronizeId, this));
			},

			userId: function (value) {
				if (value) {
					this.element.attr('userId', value);
				}

				return this.element.attr('userId');
			},

			userName: function (value) {
				if (value) {
					this.element.val(value);
				}

				return this.element.val();
			},

			_synchronizeName: function () {
				var userId = this.userId();

				if (userId == null || userId == -1) {
					return;
				}

				var user = $.grep(this.options.source, function (u) {
					return u.Id == userId;
				});

				if (user.length > 0) {
					this.userName(user[0].Name);
				} else {
					this._userDeleted();
				}
			},

			_userDeleted: function () {
				this.element.enabled(false);
				// TODO: bad that widget knows about other elements on page, think how to do better
				this.element.parents('.users-block').find('.svnuser').enabled(false);
			},

			_synchronizeId: function () {
				var context = this;

				var user = $.grep(this.options.source, function (u) {
					return u.Name == context.userName();
				});

				if (user.length > 0) {
					this.userId(user[0].Id);
				}
			}
		});

		$.widget("ui.tpUserInput", {
			_create: function () {
				this.element.trigger('mappingadded');

				this.element.change(function () {
					$(this).attr('userId', -1);
				});

				this.element.bind('validate', function () {
					if ($(this).val() == '') {
						return;
					}
					if ($(this).attr('userId') != null) {
						if ($(this).attr('userId') != -1) {
							return;
						}
					}

					$(this).error({ message: '' });
				});
			},

			validate: function () {
				this.element.trigger('validate');
			}
		});

		$.widget("ui.success", {
			_create: function () {
				this.successElement = $('<p class="message-ok"><span>Connection was established successfully</span></p>');
				this.element.before(this.successElement);
			},

			clear: function () {
				this.successElement.remove();
				this.destroy();
			}
		});

		$.widget("ui.editorAnimation", {
			animation: {
				appendTo: function (element, container) {
					element.hide();
					element.appendTo(container);
					element.show("blind", { direction: "vertical" }, 500);
				},

				remove: function (element) {
					element.hide("blind", { direction: "vertical" }, 500);
					element.remove();
				}
			},

			noAnimation: {
				appendTo: function (element, container) {
					element.appendTo(container);
				},

				remove: function (element) {
					element.remove();
				}
			},

			_create: function () {
			},

			_getStrategy: function () {
				if (globalAnimation.prototype.turnedOn) {
					return this.animation;
				} else {
					return this.noAnimation;
				}
			},

			appendTo: function (container) {
				this._getStrategy().appendTo(this.element, container);
			},

			remove: function () {
				this._getStrategy().remove(this.element);
			}
		});
		
		function globalAnimation() {
		}

		globalAnimation.prototype = {
			turnedOn: false
		};

		return globalAnimation;
	});