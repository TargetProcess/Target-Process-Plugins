Ext.ns('Tp.controls.kanbanboard.sections');

/**
* Kanban board consists of several vertical swimlanes.
* Each swimlane displays items such as <em>Tasks</em> and <em>Bugs</em> in a particular state,
* such as <em>Open</em>, <em>In Progress</em>, etc.
* It also allows dragging items onto other swimlanes.
* See {@link Tp.controls.kanbanboard.sections.Backlog} for more details.
*/
Tp.controls.kanbanboard.sections.ProjectSwimlane = Ext.extend(Tp.controls.kanbanboard.sections.Swimlane, {
	comparator: function (a, b) {
		return a.numericPriority - b.numericPriority;
	}
});

Tp.controls.kanbanboard.sections.OrderByEndDateSwimlane = Ext.extend(Tp.controls.kanbanboard.sections.Swimlane, {
	comparator: function (a, b) {
		//return b.endDate - a.endDate;
        return a.endDate - b.endDate;
	}
});