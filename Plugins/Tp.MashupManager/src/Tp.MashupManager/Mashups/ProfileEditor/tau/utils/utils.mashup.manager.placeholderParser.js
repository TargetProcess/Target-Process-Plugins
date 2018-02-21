tau.mashups
    .addDependency('tau/mashup.manager/utils/utils.mashup.manager.placeholderUtils')
    .addModule('tau/mashup.manager/utils/utils.mashup.manager.placeholderParser', function(PlaceholderUtils) {

        var PlaceholderParser = function() {
            this.placeholderUtils = new PlaceholderUtils();
            this.savedcontent = '';
            this.selection = null;
        };

        PlaceholderParser.prototype = {
            attach: function($placeholder) {
                $placeholder.on('paste', function(event) {
                    this.handlepaste($placeholder, event);
                }.bind(this));
            },

            handlepaste: function(elem, e) {
                this.savedcontent = elem.val();
                this.selection = this._getSelection(elem[0]);

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
                var pasted = this.placeholderUtils.urlToPlaceholder(pasteddata);

                var value = this.savedcontent;
                var firstPart = value.substr(0, this.selection.start);
                var secondPart = value.substr(this.selection.end, value.length);

                elem.val(firstPart + pasted + secondPart);

                this._selectRange(elem[0], firstPart.length, firstPart.length);
            },

            _getSelection: function(elem) {
                var data = {start: 0, end: elem.value.length, length: 0};

                if (elem.selectionStart >= 0) {
                    // DOM 3
                    data.start = elem.selectionStart;
                    data.end = elem.selectionEnd;
                    data.length = data.end - data.start;
                    data.text = elem.value.substr(data.start, data.length);
                } else if (elem.ownerDocument.selection) {
                    // IE
                    var range = elem.ownerDocument.selection.createRange();
                    if (!range) {
                        return data;
                    }
                    var textRange = elem.createTextRange();
                    var dTextRange = textRange.duplicate();

                    textRange.moveToBookmark(range.getBookmark());
                    dTextRange.setEndPoint('EndToStart', textRange);

                    data.start = dTextRange.text.length;
                    data.end = data.start + range.text.length;
                    data.length = range.text.length;
                    data.text = range.text;
                }

                return data;
            },

            _selectRange: function(elem, start, end) {
                if (elem.setSelectionRange) {
                    elem.focus();
                    elem.setSelectionRange(start, end);
                } else if (elem.createTextRange) {
                    var range = elem.createTextRange();
                    range.collapse(true);
                    range.moveEnd('character', end);
                    range.moveStart('character', start);
                    range.select();
                }
            }
        };

        return PlaceholderParser;
    });
