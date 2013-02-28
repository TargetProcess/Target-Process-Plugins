Ext.namespace('Tp.components');

Tp.components.LongRunningTask = Ext.extend(Object, {
	_items: null,
	_itemsPerIteration: null,
	_itemHandler: null,
	_index: -1,
	_disposed: false,
	interval: 600,



	constructor: function (items, itemHandler, finalHandler, itemsPerIteration,itemPredicate) {
		Tp.util.validateForNulls([items, itemHandler]);
		this._items = items;
		this._itemHandler = itemHandler;
		this._finalHandler = finalHandler || function () { };
		this._itemsPerIteration = itemsPerIteration || 50;

        this.itemPredicate = itemPredicate || function(item){
            return item.rendered;
        };
	},

	init: function () {
		if (this._disposed) {
			throw new Error("Task is already disposed. You cannot run it again.");
		}
		Ext.TaskMgr.start(this);
	},

	dispose: function () {
		this._disposed = true;
		Ext.TaskMgr.stop(this);
	},

	isNoItemsToProcess: function () {
		return this._index == this._items.length - 1;
	},

	_stop: function () {
		this._finalHandler();
		this.dispose();
	},

	_run: function () {
		if (this.isNoItemsToProcess()) {
			this._stop();
			return;
		}

		if (this._items[this._index + 1].rendered) {
			while (this._index < this._items.length - 1) {
                var item = this._items[this._index + 1];
                if (!this.itemPredicate(item)) {
					break;
				}
				this._index++;
			}
		} else {
			while (this._index >= 0) {
				if (this.itemPredicate(this._items[this._index])) {
					break;
				}
				this._index--;
			}
		}

		var startFrom = this._index + 1;
		var endAt = startFrom + this._itemsPerIteration;

		for (var i = startFrom; i < endAt; i++) {
			if (i > this._items.length - 1) {
				this._stop();
				return;
			}
			this._index = i;
			this._itemHandler(this._items[i], i);
		}
	},

	run: function () {
		if (this._disposed) {
			return;
		}

		try {
			this._run();
		}
		catch (e) {
			this._stop();
			throw new Error(String.format("LongRunningTask:{0}", e.message));
		}
	}
});
