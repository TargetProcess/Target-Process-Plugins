Ext.ns('Tp.controls.menu');

var projectContextPopupCount = 0;
function genProjectContextPopupId()
{
    return "pcp_"+(projectContextPopupCount++);
}
Tp.controls.menu.ProjectContextPopup = Ext.extend(Tp.controls.menu.BasePopup, {
	popupTabClassName: 'popupTab',
	projectIdContainer: null,

	constructor: function (config) {

		Tp.controls.menu.ProjectContextPopup.superclass.constructor.call(this, config);
		this.projectIdContainer = new Tp.controls.project.ProjectIdContainer();

		this.on("aftershow", this.onShowHandler, this);
	},

	_createContainer: function (config) {
		this.containerId = genProjectContextPopupId();
		var link = this.getTriggerElement();
		var hrefToRedirect = link.getAttribute('href') || '#';

		var tpl = new Ext.XTemplate(
						'<div id="{projectListId}"><tpl for="."><a href="',
						hrefToRedirect,
						hrefToRedirect.indexOf('?') >= 0 ? '&' : "?",
						'ProjectID={id}" projectId="{id}">{name}</a><br></tpl></div>'
					);
		link.set({ href: "javascript:void(0);" });
		this.container = new Ext.Element(Ext.DomHelper.createDom({ tag: 'div', cls: 'morePanel', style: 'padding: 5px' }, Ext.getBody()));
		tpl.overwrite(this.container, Ext.apply(this._projectContext, { projectListId: this.containerId }));

		Ext.each(Ext.query('[@id=' + this.containerId + '] a'), function (element) { Ext.get(element).on('click', this._onLinkClick, this); }, this)
	},

	_onLinkClick: function (element) {
		var el = Ext.fly(element.target);
		var projectId = el.getAttribute('projectId');
		this.projectIdContainer.set(projectId);
		var url = new Tp.URL(el.getAttribute('href'));
		url.setArgumentValue(this.acidKey, _projectsData.getProjectData(projectId).acid);
		el.set({ href: url.toString() });
	},

	onShowHandler: function () {
		this.showPopup(this.getTriggerElement(), this.container);
	},

	showPopup: function (link, panel) {
		var extPanel = Ext.get(panel);

		extPanel.alignTo(link, "?").show();

		// hide when ESC pressed
		Ext.get(document).on({
			'keyup': function (e) { if (e.getKey() == e.ESC) { extPanel.hide(); } },
			single: true
		});
	},

	init: function () {
	}
});
