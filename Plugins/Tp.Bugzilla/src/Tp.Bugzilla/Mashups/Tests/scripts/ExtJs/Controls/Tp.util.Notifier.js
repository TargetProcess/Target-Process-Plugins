//
// Copyright (c) 2005-2009 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

Ext.ns('Tp.util');

/**
 * Display various notification messages in a popup window.
 */
Tp.util.Notifier = function() {
    var msgCt;

    function createBox(t, s, cls) {
        return ['<div class="msg">',
            '<div class="x-box-tl"><div class="x-box-tr"><div class="x-box-tc"></div></div></div>',
            '<div class="x-box-ml"><div class="x-box-mr"><div class="x-box-mc">',
            '<h3 class="', cls, '">', t, '</h3>',
            '<span class="', cls, '">', s,'</span>',
            '</div></div></div>',
            '<div class="x-box-bl"><div class="x-box-br"><div class="x-box-bc"></div></div></div>',
            '</div>'].join('');
    }

    function doNotify(title, format, cls, pause) {
        if (!msgCt) {
            msgCt = Ext.DomHelper.insertFirst(document.body, { id: 'notifier' }, true);
        }
        msgCt.alignTo(document, 't-t');
        var s = String.format.apply(String, Array.prototype.slice.call(arguments, 1));
        var m = Ext.DomHelper.append(msgCt, { html: createBox(title, s, cls) }, true);
        m.slideIn('t').pause(pause).ghost("t", { remove: true });
    }

    return {
        /**
         * Display notification message.
         * @param title Title.
         * @param format Message.
         */
        notify: function(title, format) {
            doNotify(title, format, 'message', 0.5);
        },

        /**
         * Display error message.
         * @param title Title.
         * @param format Message.
         */
        error: function(title, format) {
            doNotify(title, format, 'error', 5);
        }
    };
}();