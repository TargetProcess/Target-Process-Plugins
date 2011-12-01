Ext.ns('Tp.controls.kanbanboard.sections');

Tp.controls.kanbanboard.sections.SwimlaneHeader = Ext.extend(Ext.BoxComponent, {
	autoEl: 'span',

	onRender: function (ct, position) {
		Tp.controls.kanbanboard.sections.SwimlaneHeader.superclass.onRender.call(this, ct, position);
		this.swimlane.initTitle = new String(this.title);
	}
});