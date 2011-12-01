Ext.namespace('Ext.ux');

/**
 * Allows drag and drop sorting of HTML elements.
 * 
 * See <a href="http://extjs.com/learn/Ext_Extension:Sortable">extension home page</a>
 * and <a href="http://extjs.com/forum/showthread.php?p=136245">forum page</a>
 * for details about this extension.
 *
 * @param {Object} config The config object.
 */
Ext.ux.Sortable = function(config) {

    config = Ext.applyIf(config, {
        container: document.body,
        selector: 'li',
        dragGroups: [
            'default'
        ],
        autoEnable: true,
        horizontal: false
    });

    this.ddGroups = {};

    Ext.applyIf(this, config);

    this.dragZone = new Ext.dd.SortableDragZone(this.container, {
        sortable: this,
        selector: this.selector,
        ddGroup: this.dragGroups[0],
        scroll: false,
        containerScroll: true
    });

    this.dropZone = new Ext.dd.SortableDropZone(this.container, {
        sortable: this,
        selector: this.selector,
        ddGroup: this.dragGroups[0],
        horizontal: this.horizontal
    });

    if (this.dragGroups.length > 1) {
        this.addToDDGroup(this.dragGroups);
    }
    else {
        this.ddGroups[this.dragGroups] = true;
    }

    this.addEvents({
        /**
         * @event sorted
         * Fires when drag and drop operation completes and reorders items.
         * @param {Ext.ux.Sortable} this
         */
        "sorted": true
    });

    //automatically start the DD
    if (this.autoEnable) {
        this.enable();
    }
};

Ext.extend(Ext.ux.Sortable, Ext.util.Observable, {
    /**
     * Function gets the items in the list area.
     * @public
     * @returns {Array} An array ob DOM references to the nodes contained in the sortable list.
     */
    serialize: function() {
        return Ext.query(this.selector, this.container);
    },

    /**
     * Enables DD on the container element.
     * @public
     */
    enable: function() {
        this.drags = this.serialize(false);

        var i = this.drags.length - 1;
        while (i >= 0) {
            Ext.dd.Registry.register(this.drags[i], { ddGroups: this.ddGroups });
            --i;
        }

        this.dropZone.unlock();
        this.dragZone.unlock();
    },

    /**
     * Disable all DD.
     * @public
     */
    disable: function() {
        this.drags = this.serialize(false);

        var i = this.drags.length - 1;
        while (i >= 0) {
            Ext.dd.Registry.unregister(this.drags[i]);
            --i;
        }

        this.dropZone.lock();
        this.dragZone.lock();
    },

    /**
     * Adds elements to a particular DD Group.
     * @public
     * @param {String/Array} DD group(s) you want to add your list to.
     */
    addToDDGroup: function(groupName, enable) {
        if (typeof groupName != 'string') {
            var i = groupName.length - 1;
            while (i >= 0) {
                this.ddGroups[groupName[i]] = true;
                this.dragZone.addToGroup(groupName[i]);
                this.dropZone.addToGroup(groupName[i]);
                --i;
            }
        }
        else {
            this.ddGroups[groupName] = true;
            this.dragZone.addToGroup(groupName);
            this.dropZone.addToGroup(groupName);
        }
        if (typeof enable !== 'undefined' || enable) {
            this.enable();
        }
    },

    /**
     * Removes a list from a particular DD Group.
     * @public
     * @param {String/Array} DD group(s) you want to remove your list from.
     */
    removeFromDDGroup: function(groupName, enable) {
        if (typeof groupName != 'string') {
            var i = groupName.length - 1;
            while (i >= 0) {
                this.ddGroups[groupName[i]] = false;
                this.dragZone.removeFromGroup(groupName[i]);
                this.dropZone.removeFromGroup(groupName[i]);
                --i;
            }
        }
        else {
            this.ddGroups[groupName] = false;
            this.dragZone.removeFromGroup(groupName);
            this.dropZone.removeFromGroup(groupName);
        }
        if (typeof enable !== 'undefined' || enable) {
            this.enable();
        }
    }
});

Ext.dd.SortableDragZone = function(el, config) {
    Ext.dd.DragZone.superclass.constructor.call(this, el, config);
};

Ext.extend(Ext.dd.SortableDragZone, Ext.dd.DragZone, {
    onInitDrag: function(x, y) {
        var srcEl = Ext.get(this.dragData.ddel);

        var dragged = srcEl.dom.cloneNode(true);
        dragged.id = '';

        srcEl.setStyle('visibility', 'hidden');

        this.proxy.update(dragged);
        this.onStartDrag(x, y);

        return true;
    },

    afterRepair: function() {
        var srcEl = Ext.get(this.dragData.ddel);
        srcEl.setStyle('visibility', '');

        this.dragging = false;
    },

    getRepairXY: function(e) {
        return Ext.Element.fly(this.dragData.ddel).getXY();
    },

    onEndDrag: function(data, e) {}
});

Ext.dd.SortableDropZone = function(el, config) {
    Ext.dd.DropZone.superclass.constructor.call(this, el, config);
};

Ext.extend(Ext.dd.SortableDropZone, Ext.dd.DropZone, {
    notifyEnter: function(source, e, data) {
        this.srcEl = Ext.get(data.ddel);
        if (this.testDDGroup()) {
            if (this.srcEl !== null) {
                if (this.srcEl.dom.parentNode !== this.el.dom) {
                    if (!Ext.query(this.selector, this.el).length > 0 && this.srcEl.is(this.selector)) {
                        this.srcEl.appendTo(this.el);
                    }
                }
                //add DD ok class to proxy
                if (this.overClass) {
                    this.el.addClass(this.overClass);
                }
                return this.dropAllowed;
            }
        }
    },

    notifyOver: function(dd, e, data) {
        if (this.testDDGroup()) {
            var n = this.getTargetFromEvent(e);
            if (!n) {
                if (this.lastOverNode) {
                    this.onNodeOut(this.lastOverNode, dd, e, data);
                    this.lastOverNode = null;
                }
                return this.onContainerOver(dd, e, data);
            }
            if (this.lastOverNode != n) {
                if (this.lastOverNode) {
                    this.onNodeOut(this.lastOverNode, dd, e, data);
                }
                this.onNodeEnter(n, dd, e, data);
                this.lastOverNode = n;
            }
            return this.onNodeOver(n, dd, e, data);
        }
    },

    notifyDrop: function(dd, e, data) {
        if (this.testDDGroup()) {
            if (this.srcEl !== null) {
                this.srcEl.setStyle('visibility', '');
            }
            this.sortable.fireEvent("sorted", this.sortable);
            return true;
        }
    },

    onContainerOver: function(dd, e, data) {
        if (this.testDDGroup()) {
            return this.dropAllowed;
        }
    },

    onNodeOver: function(n, dd, e, data) {
        if (this.testDDGroup()) {
            if (this.horizontal) {
                var x = e.getPageX();
                if (x < this.lastX) {
                    this.goingPrevious = true;
                }
                else if (x > this.lastX) {
                    this.goingPrevious = false;
                }
                this.lastX = x;
            }
            else {
                var y = e.getPageY();
                if (y < this.lastY) {
                    this.goingPrevious = true;
                }
                else if (y > this.lastY) {
                    this.goingPrevious = false;
                }
                this.lastY = y;
            }

            var destEl = Ext.get(n.ddel);

            if (this.goingPrevious) {
                this.srcEl.insertBefore(destEl);
            }
            else {
                this.srcEl.insertAfter(destEl);
            }

            return this.dropAllowed;
        }
        else {
            return this.dropNotAllowed;
        }
    },

    //private
    testDDGroup: function() {
        var groupTest = Ext.dd.Registry.getTarget(this.srcEl.id).ddGroups;
        var result = false;
        for (this.groups in groupTest) {
            if (groupTest[this.groups]) {
                result = true;
            }
        }
        return result;
    }
});