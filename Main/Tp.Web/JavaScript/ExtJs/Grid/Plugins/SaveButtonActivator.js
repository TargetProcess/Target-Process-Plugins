Ext.ns('Tp.controls.grid.plugins');

Tp.controls.grid.plugins.SaveButtonActivator = Ext.extend(Object, {
	grid: null,

	init: function(grid) {
		this.grid = grid;
		if (this.grid.saveButton != null)
			Tp.controls.grid.plugins.SaveButtonActivator.saveButton = this.grid.saveButton;
		this.grid.on("afteredit", this.activateSaveButton, this);
		this.grid.on(this.grid.afterSuccessSaved, this.activateSaveButton, this);
	},

	activateSaveButton: function() {
		if (this.grid.isDataModified())
			Tp.controls.grid.plugins.SaveButtonActivator.saveButton.enable();
		else
			Tp.controls.grid.plugins.SaveButtonActivator.saveButton.disable();
	}
})

Tp.controls.grid.plugins.SaveButtonActivator.saveButton = null;