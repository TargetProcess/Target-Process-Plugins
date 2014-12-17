define([
    'Underscore'
    , 'tau/mashup.manager/utils/utils.mashup.manager.placeholderUtils'
    , 'jQuery'
    , 'libs/jquery/jquery.fieldSelection'
], function(_, PlaceholderUtils) {
    var PlaceholderParser = function() {
        this.placeholderUtils = new PlaceholderUtils();
        this.savedcontent = '';
        this.selection = null;
    };

    PlaceholderParser.prototype = {
        attach: function(placeholder) {
            placeholder.on('paste', _.bind(function(event) {
                this.handlepaste(placeholder, event);
            }, this));
        },

        handlepaste: function(elem, e) {
            this.savedcontent = elem.val();
            this.selection = elem.fieldSelection();

            if (e && e.clipboardData && e.clipboardData.getData) {
                // Webkit - get data from clipboard, put into editdiv, cleanup, then cancel event
                if (/text\/plain/.test(e.clipboardData.types)) {
                    elem.val(e.clipboardData.getData('text/plain'));
                } else {
                    elem.val('');
                }
                this.waitforpastedata(elem);
                if (e.preventDefault) {
                    e.stopPropagation();
                    e.preventDefault();
                }
                return false;
            } else {
                // Everything else - empty editdiv and allow browser to paste content into it, then cleanup
                elem.val('');
                this.waitforpastedata(elem);
                return true;
            }
        },

        waitforpastedata: function(elem) {
            var context = this;
            if (elem.val()) {
                this.processpaste(elem);
            } else {
                var that = {
                    e: elem,
                    callself: function() {
                        context.waitforpastedata(that.e);
                    }
                };
                setTimeout(that.callself, 20);
            }
        },

        processpaste: function(elem) {
            var pasteddata = elem.val();

            var value = this.savedcontent;
            var pasted = this.placeholderUtils.urlToPlaceholder(pasteddata);

            var firstPart = value.substr(0, this.selection.start) + pasted;
            var secondPart = value.substr(this.selection.end, value.length);

            elem.val(firstPart + secondPart);

            this._selectRange(elem, firstPart.length, firstPart.length);
        },

        _selectRange: function(el, start, end) {
            return el.each(function() {
                if (this.setSelectionRange) {
                    this.focus();
                    this.setSelectionRange(start, end);
                } else if (el.createTextRange) {
                    var range = el.createTextRange();
                    range.collapse(true);
                    range.moveEnd('character', end);
                    range.moveStart('character', start);
                    range.select();
                }
            });
        }
    };

    return PlaceholderParser;
});
