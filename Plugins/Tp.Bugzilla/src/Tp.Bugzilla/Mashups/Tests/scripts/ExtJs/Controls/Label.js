Ext.ns('Tp.controls');

/**
 * Fixes bug in original ExtJS-2.2 label when it renders text instead of html
 * if called setter setText on a label which has not been rendered yet.
 */
Tp.controls.Label = Ext.extend(Ext.form.Label, {
    setText: function(t, encode) {
        if (encode !== false) {
            this.text = t;
        } else {
            this.html = t;
        }
        if (this.rendered) {
            this.el.dom.innerHTML = encode !== false ? Ext.util.Format.htmlEncode(t) : t;
        }
        return this;
    }
});

Ext.reg('tplabel', Tp.controls.Label);