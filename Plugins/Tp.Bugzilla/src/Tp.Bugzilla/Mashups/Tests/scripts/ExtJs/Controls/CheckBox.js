Ext.ns('Tp.controls');
/**
 * Fixes bug 12126.
 * The standard ExtJS 2.2 checkbox always returns false in getValue() if it has not been rendered yet.
 * But if a checkbox is placed on a tab, it only gets rendered when this tab is swithced to.
 *
 * TODO consider using configuration property 'deferredRender:false' on tab panels instead this workaround
 */
Tp.controls.CheckBox = Ext.extend(Ext.form.Checkbox, {
    getValue: function() {
        if (this.rendered) {
            return this.el.dom.checked;
        }
        return this.checked;
    }
});

Ext.reg('tpcheckbox', Tp.controls.CheckBox);