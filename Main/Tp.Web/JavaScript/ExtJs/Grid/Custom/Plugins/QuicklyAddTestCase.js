function QuicklyAddTestCase(gridId, userStoryId, processId) {
	var panel = new Tp.controls.TestCaseCreateEditorWindow({
		userStoryId: userStoryId,
		processId: processId
	});

	this.gridId = gridId;

	panel.on("saved", Tp.controls.grid.Handlers.refreshGrid(this.gridId), this),

	panel.show();
}