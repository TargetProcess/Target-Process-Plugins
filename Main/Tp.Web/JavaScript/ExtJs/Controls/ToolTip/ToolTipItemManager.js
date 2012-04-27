Ext.ns('UseCaseHelp.ToolTipItemManager');

UseCaseHelp.ToolTipItemManager.defaultItem = null;

UseCaseHelp.ToolTipItemManager = Ext.extend(Object, {

	getItemIndex: function(componentId) {
		for (var i = 0; i < this.items.length; i++) {
			if (this.items[i].id == componentId)
				return i;
		}
		throw "UseCaseHelp.ToolTipItemManager.getItemIndex: Unable to find item index.";
	},

	getNextToolTipItemId: function(componentId) {
		if (!this.items)
			return null;

		var index = this.getItemIndex(componentId);
		if (index >= this.items.length - 1)
			return null;

		return this.items[index + 1].id;
	},

	getPrevToolTipItemId: function(componentId) {
		if (!this.items)
			return null;

		var index = this.getItemIndex(componentId);
		if (index <= 0)
			return null;

		return this.items[index - 1].id;
	},

	register: function(item) {
		if (!this.items)
			this.items = new Array();
		this.items.push(item);
	}
})

UseCaseHelp.ShowDefaultToolTipItemManager = Ext.extend(UseCaseHelp.ToolTipItemManager, {

	register: function (item) {
		var self = this;
		UseCaseHelp.ShowDefaultToolTipItemManager.superclass.register.call(self, item);
		if (this.items.length <= 2)
			setTimeout(function () { self.items[0].showToolTip(); }, 200);
	}
})

Ext.ns('UseCaseHelp.ToolTipItemManagerFactory');

UseCaseHelp.ToolTipItemManagerFactory.Type = "UseCaseHelp.ToolTipItemManager";

UseCaseHelp.ToolTipItemManagerFactory.getInstance = function() {
	if (UseCaseHelp.ToolTipItemManagerFactory.instance == null)
		UseCaseHelp.ToolTipItemManagerFactory.instance = eval("new " + UseCaseHelp.ToolTipItemManagerFactory.Type);
	return UseCaseHelp.ToolTipItemManagerFactory.instance;
}