tau.mashups
    .addDependency("tp/plugins/bugzilla/bugInfoTemplate")
    .addDependency("BugzillaListDetails/InfoBubbleTemplate")
    .addDependency("libs/jquery/jquery")
    .addModule("BugzillaListDetails/InfoBubble", function (template, wrapperTemplate) {
        function bugzillaInfoBubble() {
            this.template = template;
            this.wrapperTemplate = wrapperTemplate;
            this.dialog = null;
            this._clickProxy = $.proxy(this._onClick, this);
            this.bugInfo = null;
        }

        bugzillaInfoBubble.prototype = {

            toggleBubble: function (event, bugInfo) {
                if (!this._isOpen() || this._alreadyOpenForAnotherBug(bugInfo)) {
                    event.stopPropagation();
                }
                if (this._alreadyOpenForAnotherBug(bugInfo)) {
                    this._close();
                }
                if (!this._isOpen()) {

                    var bug = $('a[bugid="' + bugInfo.TpId + '"]');
                    var scroll = this._getPageScroll();
                    var that = this;

                    this.bugInfo = bugInfo;
                    var wrapper = $(this.wrapperTemplate);
                    var rendered = $.tmpl(this.template, bugInfo);
                    rendered.appendTo(wrapper);
                    this.dialog = $(wrapper).dialog(
                        {
                            draggable: false,
                            position: [bug.offset().left - 18 - scroll[0], bug.offset().top + 30 - scroll[1]],
                            close: function () {
                                that.dialog = null;
                                that.bugInfo = null;
                            },
                            closeText: '',
                            open: function () {
                                $(this).parent().find('.ui-dialog-titlebar').hide();
                                $(this).parent().removeClass('ui-widget-content');
                                $(this).find('.bugzilla-external-link').click(function (e) {
                                    e.preventDefault();
                                    window.open($(this).attr('href'));
                                });
                            }
                        });

                    $('body').click(this._clickProxy);
                }
            },

            _close: function () {
                this.dialog.dialog('close').dialog('destroy');
            },

            _alreadyOpenForAnotherBug: function (bugInfo) {
                return this._isOpen() && this.bugInfo != null && this.bugInfo.Id !== bugInfo.Id;
            },

            _onClick: function (event) {
                var target = $(event.target);

                if (this._isOpen() && !this._isDialog(target)) {
                    this._close();
                    $('body').unbind('click', this._clickProxy);
                }
            },

            _isDialog: function (target) {
                return target.hasClass('bugzillaBubble') || target.parents('.bugzillaBubble').length > 0;
            },

            _isOpen: function () {
                return this.dialog != null && this.dialog.dialog('isOpen');
            },

            _getPageScroll: function () {
                var xScroll, yScroll;
                if (self.pageYOffset) {
                    yScroll = self.pageYOffset;
                    xScroll = self.pageXOffset;
                } else if (document.documentElement && document.documentElement.scrollTop) {
                    yScroll = document.documentElement.scrollTop;
                    xScroll = document.documentElement.scrollLeft;
                } else if (document.body) {// all other Explorers
                    yScroll = document.body.scrollTop;
                    xScroll = document.body.scrollLeft;
                }
                return new Array(xScroll, yScroll)
            }
        };

        return bugzillaInfoBubble;
    })