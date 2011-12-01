Ext.ns('Tp.controls.menu');

Tp.controls.menu.ItemSelector = Ext.extend(Object, {
    storage: null,
    _triggerItem: null,
    handler: null,
    _context: null,

    constructor: function(config) {
        Ext.apply(this, config);
    },

    _createMenu: function(items) {
        var menu = new Ext.menu.Menu({ shadow: false, enableScrolling: false, cls: 'morePanel', items: items, style: { overflow: 'visible'} });
        menu.on("click", this._onClick, this);
        return menu;
    },

    _updateTriggerElement: function(newContext, htmlToRender) {
        this._triggerItem.update(newContext, htmlToRender);
    },

    _toggleGroup: function(menu, item) {
        var index = menu.items.indexOf(item);
        if (index == -1) {
            return;
        }
        for (i = index + 1; i < menu.items.getCount(); i++) {
            var menuItem = menu.items.get(i);
            if (menuItem.isGroup) {
                break;
            }
            if (menuItem.hidden) {
                menu.items.get(i).show();
            }
            else {
                menu.items.get(i).hide();
            }
        }
        menu.doLayout();
    },

    _onClick: function(sender, item, eventObj) {
        if (item.isShowMore) {

            this.storage.getMenuItems(this._context, true,
                    function(items) {
                        var menu = this._createMenu(items);
                        menu.show(this._triggerItem.element);
                    }, this);
            return;
        }

        if (item.isGroup) {
            this._toggleGroup(sender, item);
            return;
        }

        sender.getEl().fadeOut({ endOpacity: 0.2, easing: 'easeBoth', duration: 1, remove: false, useDisplay: false });
        this.handler.handle(item.dataItem, this._context, this._updateTriggerElement.createDelegate(this, [item.htmlToRender], true));
    },

    show: function(triggerItem, context) {
        this._context = context;
        this._triggerItem = triggerItem;

        var showMenuCallback = function(items) {
            var menu = this._createMenu(items);
            menu.show(this._triggerItem.element);
            menu.getEl().fadeIn({ endOpacity: 1, easing: 'easeBoth', duration: 0.5, remove: false, useDisplay: false });
        }
        var items = this.storage.getMenuItems(context, false, showMenuCallback, this);
    }
});
