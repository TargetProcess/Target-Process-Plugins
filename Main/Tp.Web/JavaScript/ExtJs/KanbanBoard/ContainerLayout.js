Tp.controls.kanbanboard.ItemContainerLayout = Ext.extend(Ext.layout.ContainerLayout, {
	runningTask: null,
	finalCallback: null,

	getItemHandler: function (target) {
		return (function (item, position) {
			if (item && (!item.rendered || !this.isValidParent(item, target))) {
				this.renderItem(item, position, target);
			}
		}).createDelegate(this);
	},

	renderAll: function (ct, target) {

        var that = this;
		var items = ct.items.items;
		if (items.length == 0) {
			this.finalCallback();
			return;
		}

		if (this.runningTask != null) {
			this.runningTask.dispose();
		}

		this.runningTask = new Tp.components.LongRunningTask(items,
            this.getItemHandler(target),
            this.finalCallback,
            null,
            function(item){
                return item && item.rendered && that.isValidParent(item, target);
            }
        );
		this.runningTask.init();
	}
});
