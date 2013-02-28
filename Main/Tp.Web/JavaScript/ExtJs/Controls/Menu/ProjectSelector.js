try
{
log.info("ProjectSelector.js file executing.");
if (!Ext) {
	log.info("ExtJs is undefined");
}

if (!Ext.extend) {
    log.error("Ext.extend is undefined.");
}

if (!Ext.ns) {
    log.error("Ext.ns is undefined.");
}

Ext.ns('Tp.controls.menu');

if (!Tp || !Tp.controls || !Tp.controls.menu) {
    log.error("Tp.controls.menu is undefined.");
}

Tp.controls.menu.ProjectSelector = Ext.extend(Object, {
	projectContext: null,
	projectsData: null,
	checkBoxes: [],
	inProcessing: false,
	popupContainer: null,
	projectSelectionLink: null,
	selectAllProjectsLink: null,
	unSelectAllProjectsLink: null,
	showAllButton: null,
	projectsCount: 0,

	constructor: function (config) {
		Ext.apply(this, config);
		this.projectContext = config.projectContext;
		this.projectsData = config.projectsData;
		Array.forEach(this.projectsData, function (project) {
			if (project.type != 'Program')
				this.projectsCount += 1;
		}, this);
	},

	init: function () {
		this.setChildProjectsForProgram();

		var projectSelectionMenu = Ext.select("div.projectMenu").first();
		if (projectSelectionMenu == null) {
		    log.info("Tp.controls.menu.ProjectSelector: projectSelectionMenu is null");
		    return;
		}

		this.getContentTemplate().overwrite(projectSelectionMenu, { projects: this.projectsData, selectedProjects: this.projectContext });
		log.info("Tp.controls.menu.ProjectSelector: template rendered");
		this.initSelectAllButtons();

		var checkBoxContainers = Ext.select("input.projectSelectorCheckBox");
		Array.forEach(checkBoxContainers.elements, this.initRadio, this);

		var selectProjectsButtonHolderDiv = Ext.get('selectProjectsButtonHolderDiv');
		if (!Ext.isEmpty(selectProjectsButtonHolderDiv)) {

			this.showAllButton = new Ext.Button({ renderTo: 'selectProjectsButtonHolderDiv', text: 'Show', width: 120, height: 10, disabled: this.getSelectedProjectIds().length == 0 });
			this.showAllButton.on("click", Function.createDelegate(this, this.onShowClicked));
		}

		this.checkProjectsInContext();

		this.popupContainer = Ext.select("div.projectSelection").first();
		this.popupContainer.setVisibilityMode(Ext.Element.DISPLAY);

		this.projectSelectionLink = Ext.select("a.projectSelectionLink").first();
		this.projectSelectionLink.on("click", Function.createDelegate(this, this.onProjectMenuClicked));

		var moreLink = Ext.select("a.moreLinkPPS");
		moreLink.on("click", Function.createDelegate(this, this.onMoreClicked));

		var lessLink = Ext.select("a.lessLinkPPS");
		lessLink.on("click", Function.createDelegate(this, this.onLessClicked));

		Ext.EventManager.onWindowResize(Function.createDelegate(this, this.onResize));
		Ext.getBody().on("mouseup", Function.createDelegate(this, this.onBodyClick));
	},

	initSelectAllButtons: function () {
		this.selectAllProjectsLink = Ext.select("a.selectAllProjects").first();
		this.unSelectAllProjectsLink = Ext.select("a.unSelectAllProjects").first();
		if (!Ext.isEmpty(this.selectAllProjectsLink) && !Ext.isEmpty(this.unSelectAllProjectsLink)) {
			this.selectAllProjectsLink.on("click", Function.createDelegate(this, this.onSelectAllProjectsClicked));
			this.unSelectAllProjectsLink.on("click", Function.createDelegate(this, this.onUnSelectAllProjectsClicked));
		}
	},

	setChildProjectsForProgram: function () {
		Array.forEach(this.projectsData, function (project) {
			var childProjectIds = new Array();
			if (project.type == 'Program') {
				Array.forEach(this.projectsData, function (childProject) {
					if (childProject.programId == project.id) {
						childProjectIds.push(childProject.id);
					}
				});
			}
			project.childProjectIds = childProjectIds;
		}, this);
	},

	onBodyClick: function (args, htmlElement) {
		if (this.popupContainer.contains(htmlElement) || this.projectSelectionLink.contains(htmlElement) || this.projectSelectionLink.dom == htmlElement)
			return;

		this.popupContainer.hide();
	},

	onResize: function () {
		var browserHeight = Ext.lib.Dom.getViewHeight();
		var popupHeight = this.popupContainer.getHeight();
		var usedHeight = popupHeight + this.popupContainer.getTop();

		var container = Ext.select("div.projectSelectionScrolling").first();
		var height = browserHeight - this.popupContainer.getTop() - 100;

		if (usedHeight > browserHeight) {
			if (height > 0)
				container.setHeight(height);
		}
		else {
			var renewHeight = container.dom.scrollHeight;
			container.setHeight(height > renewHeight ? renewHeight : height);
		}

		this.doPosition();
	},

	initRadio: function (container) {
		var projectId = container.id.replace('psc_', '');
		var checkBox = new Ext.ux.form.XCheckbox({ applyTo: container });
		checkBox.on("check", Function.createDelegate(this, this.onProjectSelectionChange));
		this.checkBoxes.push(checkBox);
		checkBox.project = this.getProjectById(projectId);
		checkBox.disabled = checkBox.project.type == 'Program' && checkBox.project.childProjectIds.length == 0;
	},

	onMoreClicked: function () {
		this.switchVisibilityLessLink(true);
	},

	switchVisibilityLessLink: function (showLess) {
		var moreLink = Ext.select("a.moreLinkPPS");
		moreLink.setVisibilityMode(Ext.Element.DISPLAY);
		moreLink.setVisible(!showLess);

		var lessLink = Ext.select("a.lessLinkPPS");
		lessLink.setVisibilityMode(Ext.Element.DISPLAY);
		lessLink.setVisible(showLess);

		var moreProjectsSpan = Ext.select('#moreProjectsSpan');
		moreProjectsSpan.setVisibilityMode(Ext.Element.DISPLAY);
		moreProjectsSpan.setVisible(showLess);
	},

	onLessClicked: function () {
		this.switchVisibilityLessLink(false);
	},

	doPosition: function () {
		var targetElement = Ext.select('a.projectSelectionLink').first();
		var xOffset = targetElement.getX();
		var yOffset = targetElement.getY();
		yOffset = yOffset + targetElement.getHeight() - 4;
		this.popupContainer.setLeft(xOffset);
		this.popupContainer.setTop(yOffset);
	},

	onProjectMenuClicked: function () {
		if (this.popupContainer.isVisible()) {
			this.popupContainer.hide();
		}
		else {
			this.doPosition();
			this.popupContainer.show();
			this.onResize();
		}
	},

	onSelectAllProjectsClicked: function () {
		Array.forEach(this.checkBoxes, function (checkBox) {
			checkBox.setValue(true);
		});
	},

	onUnSelectAllProjectsClicked: function () {
		Array.forEach(this.checkBoxes, function (checkBox) {
			checkBox.setValue(false);
		});
	},

	getSelectedProjectIds: function () {
		var selectedProjectIds = [];
		Array.forEach(this.checkBoxes, function (checkBox) {
			var project = checkBox.project;
			if (project.type != 'Program' && checkBox.getValue())
				selectedProjectIds.push(project.id);
		}, this);
		return selectedProjectIds;
	},

	onShowClicked: function () {
		projectContextService.setCurrent(this.getSelectedProjectIds());
	},

	checkProjectsInContext: function () {
		Array.forEach(this.projectContext, function (projectInContext) {
			Array.forEach(this.checkBoxes, function (checkBox) {
				if (checkBox.project.id == projectInContext.id)
					checkBox.setValue(true);
			});
		}, this);
	},

	getProjectById: function (projectId) {
		var foundProject = null;
		Array.forEach(this.projectsData, function (project) {
			if (projectId == project.id)
				foundProject = project;
		}, this);

		return foundProject;
	},

	getChildCheckBoxes: function (project) {
		var childCheckBoxes = [];
		Array.forEach(this.checkBoxes, function (checkBox) {
			if (checkBox.project.programId == project.id)
				childCheckBoxes.push(checkBox);
		}, this);
		return childCheckBoxes;
	},

	getCheckBoxForProject: function (project) {
		var foundCheckBox = null;
		Array.forEach(this.checkBoxes, function (checkBox) {
			if (checkBox.project.id == project.id)
				foundCheckBox = checkBox;
		}, this);
		return foundCheckBox;
	},

	onProjectSelectionChange: function (checkBox, checked) {

		if (this.inProcessing)
			return;

		this.inProcessing = true;

		var project = checkBox.project;

		var childCheckBoxes = this.getChildCheckBoxes(project);

		Array.forEach(childCheckBoxes, function (checkBox) {
			checkBox.setValue(checked);
		}, this);

		var program = this.getProjectById(project.programId);

		if (program) {
			childCheckBoxes = this.getChildCheckBoxes(program);

			var allChecked = true;

			Array.forEach(childCheckBoxes, function (checkBox) {
				if (!checkBox.getValue())
					allChecked = false;
			}, this);

			var programCheckBox = this.getCheckBoxForProject(program);

			if (programCheckBox)
				programCheckBox.setValue(allChecked);
		}

		this.inProcessing = false;

		this.setSelectAllVisibility();
		this.setShowButtonEnabled(this.getSelectedProjectIds().length > 0);

	},

	setShowButtonEnabled: function (enabled) {
		this.showAllButton.setDisabled(!enabled);
	},

	setSelectAllVisibility: function () {
		var allSelected = this.getSelectedProjectIds().length == this.projectsCount;
		this.unSelectAllProjectsLink.setVisibilityMode(Ext.Element.DISPLAY);
		this.selectAllProjectsLink.setVisibilityMode(Ext.Element.DISPLAY);
		this.unSelectAllProjectsLink.setVisible(allSelected);
		this.selectAllProjectsLink.setVisible(!allSelected);
	},

	getCheckBoxMarkup: function () {
		return '<input type="checkbox" id="psc_{id}" class="projectSelectorCheckBox"/>';
	},

	getContentTemplate: function () {
		return new Ext.XTemplate(
                '<span class="topTitle">Projects&nbsp;</span>',
                 '<a href="javascript:void(0)" class="projectSelectionLink">',
                    '<tpl for="selectedProjects">',
                        '<tpl if="xindex &lt; 3">{name}</tpl>',
                        '<tpl if="xindex == 1 && xcount &gt; 1">,&nbsp;</tpl>',
                    '</tpl>',
                    '<tpl if="selectedProjects.length == 0">',
                        'select ...',
                    '</tpl>',
                    '<span id="moreProjectsSpan" style="display:none;">',
                        '<tpl for="selectedProjects">',
                            '<tpl if="xindex &gt; 2">,&nbsp;</tpl>',
                            '<tpl if="xindex &gt; 2">{name}</tpl>',
                        '</tpl>',
                    '</span>',
                 '</a>',

                '<tpl if="selectedProjects.length &gt; 2">',
                    '<a class="moreLinkPPS">+{[values.selectedProjects.length - 2]} more</a>',
                    '<a class="lessLinkPPS" style="display:none">less</a>',
                '</tpl>',

                '<div class="projectSelection" style="display:none"><div class="context-popup-uxo-t" style="right: -25px;"></div>',
				'<tpl if="projects.length &gt; 0">',
					'<div class="p-5" style="height: 18px; display: block;">',
						'<a href="javascript:void(0)" class="selectAllProjects" style="">select all</a>',
						'<a href="javascript:void(0)" class="unSelectAllProjects" style="display:none">unselect all</a>',
					'</div>',
					'<div style="border-top: 1px solid #ddd"></div>',
				'</tpl>',

                '<div class="projectSelectionScrolling"><table>',
                    '<tpl for="projects">',
                        '<tpl if="type==\'Program\'">',
                            '<tr class="Program">',
                                '<td',
									'<tpl if="childProjectIds.length == 0">',
										' style="opacity:0.6; filter: alpha(opacity = 60);"',
									'</tpl>',

								'>',
                                    this.getCheckBoxMarkup(),
                                    '<img src="{[this.getProgramIcon()]}" alt=""/>',
                        '</tpl>',
                        '<tpl if="type==\'Project\'">',
                            '<tr class="Project">',
                                '<td',
									'<tpl if="programId &gt; 0">',
										' class="projectInProgram"',
									'</tpl>',

                                '>',
                                    this.getCheckBoxMarkup(),
                                    '<img src="{[this.getProjectIcon()]}" alt=""/>',
                        '</tpl>',
							'<tpl if="type==\'Project\'">',
                            	'<tpl if="abbreviation != &quot;&quot;"><div class="projectAbbr">{abbreviation}</div></tpl><a onclick=\'projectContextService.setCurrent({id})\'>{name}</a></td>',
							'</tpl>',
							'<tpl if="type==\'Program\'">',
								'<tpl if="childProjectIds.length == 0">',
									'<span style="color:#28428B; vertical-align:middle;">{name}</span>',
								'</tpl>',
								'<tpl if="childProjectIds.length &gt; 0">',
                            		'<a onclick=\'projectContextService.setCurrent([{childProjectIds}])\'>{name}</a>',
								'</tpl>',
								'</td>',
							'</tpl>',
                        '</tr>',
                    '</tpl>',
                    '<tpl if="projects.length &lt; 1">',

                        '<tr><td>There are no projects added</td></tr>',
                    '</tpl>',

                '</table></div>',
                '<tpl if="projects.length &gt; 0">',
                    '<div style="border-top: 1px solid #ddd"></div>',
                    '<div id="selectProjectsButtonHolderDiv" class="selectProjectsButtonHolder"></div>',
                '</tpl>',
                '</div>',
                    {
                    	getProjectIcon: function () {
                    		return this.getIcon("Project");
                    	},

                    	getIcon: function (type) {
                    		return appHostAndPath + "/img/" + type + ".gif";
                    	},

                    	getProgramIcon: function () {
                    		return this.getIcon("Program");
                    	}
                    }
                );
	}
});
log.info("Registering project selector creation by Ext.onReady.");
    Ext.onReady(function () {
        log.info("Project selector: creating project pre onReady called");
    });
Ext.onReady(function () {
	log.info("Creating project selector");
	var projectSelector = new Tp.controls.menu.ProjectSelector({ projectsData: (typeof projects != 'undefined' ? projects : []), projectContext: (typeof _projectContext != 'undefined' ? _projectContext : []) });
	projectSelector.init();
	log.info("Project selector created");
});
    Ext.onReady(function () {
        log.info("Project selector: creating project post onReady called");
    });
log.info("Project selector initialization is finished successfully.");
}
// TODO: Remove the try/catch/finally and logging.
catch(exc) // it's temp code
{
    log.error(exc.toString());
}
finally
{
    log.info("Project selector initialization is finished");
}