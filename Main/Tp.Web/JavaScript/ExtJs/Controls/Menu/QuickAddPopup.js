Ext.ns('Tp.controls.menu');

Tp.controls.menu.QuickAddPopup = Ext.extend(Tp.controls.menu.BasePopup, {
	quickAddMenuContainer: null,
	popupTabClassName: 'popupTab',
	projectIdContainer: null,

	constructor: function (config) {
		if (!config.className)
			config.className = "quickAddPopupMenu";

		Tp.controls.menu.QuickAddPopup.superclass.constructor.call(this, config);

		this.projectIdContainer = new Tp.controls.project.ProjectIdContainer();

		this.init();

		this.on("aftershow", this.onShowHandler, this);
		this.on("afterhide", this.onHideHandler, this);
	},

	onShowHandler: function () {
		this.getTriggerElement().parent().addClass(this.popupTabClassName);
	},

	onHideHandler: function () {
		this.getTriggerElement().parent().removeClass(this.popupTabClassName);
	},

	init: function () {
		var style;
		if (this.projectsData.length > 1) {
			style = 'margin-top: 25px;'
		}
		else if (this.projectsData.length == 1) {
			style = 'padding:10px 0px !important;';
		}
		this._clearProjectsIfUserHaveNoActions();
		this._restoreSelectedProjectFromCookie();
		this.getContentTemplate().overwrite(this.getContainerElement(), { projects: this.projectsData,
			actionList: this.actionList,
			selectedProjectId: this.selectedProjectId,
			nonProjectActionList: this.nonProjectActionList,
			actionListStyle: style
		});        
		this._attachHandlersToControls();
		this._adjustActionsForCurrentProject();
	},

	_removeProjectIdsWhichNotHaveItemsToAdd: function () {
		for (var i = this.projectsData.length; i--; i >= 0) {
			if (this._userHaveNoPermissionsToAddAnyItemsToProject(this.projectsData[i].projectId)) {
				this.projectsData.splice(i, 1);
			}
		}
	},

	_clearProjectsIfUserHaveNoActions: function () {
		if (this.actionList.length == 0) {
			this.projectsData = [];
		}
	},

	_userHaveNoPermissionsToAddAnyItemsToProject: function (projectId) {
		for (var i = 0; i < this.actionList.length; i++) {
			var action = this.actionList[i];
			if (action.availableProjectIds.indexOf(projectId) != -1) {
				return false;
			}
		}
		return true;
	},
	_attachRadioButtonsHandlers: function () {
		var radioContainers = Ext.select("input.quickAddRadio");
		Array.forEach(radioContainers.elements, this.initRadio, this);
	},

	_attachHandlersToControls: function () {
		this._attachRadioButtonsHandlers();
	},

	initRadio: function (checkbox) {
		var checkboxElement = Ext.fly(checkbox);
		var checked = parseInt(checkboxElement.getAttribute("value")) == this._getCurrentProjectId();
		var radio = new Ext.form.Radio({ applyTo: checkbox,
			checked: false,
			name: 'rb_quickadd',
			id: checkboxElement.getAttribute("id"),
			boxLabel: checkboxElement.getAttribute("projectName"),
			inputValue: checkboxElement.getAttribute("value"),
			checked: checked

		});
		radio.on("check", this.onProjectSelectionChange, this);
		if (checked) {
			radio.getEl().parent().addClass('selectedProject');
		}
	},

	onProjectSelectionChange: function (radio, checked) {
		if (checked) {
			radio.el.parent().addClass('selectedProject');
			this._setCurrentProjectId(parseInt(radio.getRawValue()));
			this._adjustActionsForCurrentProject();
		}
		else {
			radio.getEl().parent().removeClass('selectedProject');
		}

	},
	getContentTemplate: function () {
		var halfCountOfActions = Math.ceil(this.actionList.length / 2);
		var tpl = new Ext.XTemplate(
            '<tpl if="projects.length == 0 && nonProjectActionList == 0"  >',
                '<div style="padding:10px">',
                    'You don\'t have permissions to add items.',
                '</div>',
            '</tpl>',
            '<tpl if="projects.length &gt; 0">',
                '<tpl if="projects.length &gt; 1">',
                    '<div class="projectList">',
                    '<p>Select Project</p>',
                        '<tpl for="projects">',
                                '<input type="radio" value="{projectId}" class="quickAddRadio" id="checkBox_{projectId}" name="rb_quickadd" projectName="{name}">',
                        '</tpl>',
                    '</div>',
                '</tpl>',
                '<div class="actionList" style="{actionListStyle}>">',
                    '<div class="firstColumn">',
                        '<tpl for="actionList">',
                            '<tpl if="xindex &lt;= ' + halfCountOfActions + '">',
                                '<p><a navigateUrl="{url}" href="{url}" class="actionLink" entityTypeId="{entityTypeId}" >{title}</a></p>',
                            '</tpl>',
                        '</tpl>',
                    '</div>',
                    '<div class="secondColumn">',
                        '<tpl for="actionList">',
                            '<tpl if="xindex &gt; ' + halfCountOfActions + '">',
                                '<p><a navigateUrl="{url}" href="{url}" class="actionLink" entityTypeId="{entityTypeId}">{title}</a></p>',
                            '</tpl>',
                        '</tpl>',
                    '</div>',
                '</div>',
            '</tpl>',
            '<tpl if="nonProjectActionList.length &gt; 0">',
                '<div style="clear: both"></div>',
                '<div class="basicLinks"',
                '<tpl if="projects.length == 0">',
                    ' style="border-top:none !important;" ',
                '</tpl>',
                '>',
                    '<tpl for="nonProjectActionList">',
                        '<p><a href="{url}">{title}</a></p>',
                    '</tpl>',
                '</div>',
            '</tpl>'
        );

		return tpl;
	},

	onActivate: function () {
		this._adjustActionsForCurrentProject();
	},

	_getAcid: function () {
		var projectData = _projectsData.getProjectData(this._getCurrentProjectId());
		return projectData.acid;
	},

	_setupLinksHrefForCurrentProject: function () {
		var actionLinks = Ext.select('div.actionList a.actionLink');
        Array.forEach(actionLinks.elements, this._addProjectContextParameter, this);
	},

    _insertParam: function(key, value, queryString)
    {
        if(!/\.aspx/i.test(queryString)){
            return queryString;
        }
        var keyValuePair = key + "=" + value;
        var r = new RegExp("(&|\\?)" + key + "=[^\&#]*");
        queryString = queryString.replace(r, "$1" + keyValuePair);
        if(!RegExp.$1) {
            var symbol = queryString.indexOf('?') < 0 ? '?' : '&';
            queryString += symbol + keyValuePair;
        };
        return queryString;
    },

	_addProjectContextParameter: function (link) {
		//this._saveSelectedProjectToCookie();
		var linkElement = Ext.fly(link);
		var href = linkElement.getAttribute('href');
        var hrefWithParameter = this._insertParam( this.appContextKey, this._getAcid(), href);
		linkElement.set({ href: hrefWithParameter });
	},

	_saveSelectedProjectToCookie: function () {
		this.projectIdContainer.set(this._getCurrentProjectId());
	},

	_restoreSelectedProjectFromCookie: function () {
		var currentUrl = new Tp.URL(document.location.href);
		if (!currentUrl.getPath().endsWith('View.aspx') &&
            !currentUrl.getPath().endsWith('Edit.aspx') &&
            !currentUrl.getPath().endsWith('ReleasePlan.aspx')) {
			var projectId = this.projectIdContainer.get();
			if (projectId) {
				for (var i = 0; i < this.projectsData.length; i++) {
					if (this.projectsData[i].projectId == projectId) {
						this._setCurrentProjectId(projectId);
						return;
					}
				}
			}
		}
	},

	_getCurrentProjectId: function () {
		return this.selectedProjectId;
	},

	_getCurrentProcessId: function () {
		var processId;
		var projectId = this._getCurrentProjectId();
		Ext.each(this.projectProcess, function (pp) {
			if (pp.projectId == projectId) {
				processId = pp.processId;
			}
		}, this);
		return processId;
	},

	_setCurrentProjectId: function (value) {
		this.selectedProjectId = value;
	},

	getProjectData: function (projectId) {
		for (var i = 0; i < this.projectsData.length; i++) {
			if (this.projectsData[i].projectId == projectId) {
				return this.projectsData[i];
			}
		}
	},

	getActionByEntityTypeId: function (entityTypeId) {
		for (var i = 0; i < this.actionList.length; i++) {
			if (this.actionList[i].entityTypeId == entityTypeId) {
				return this.actionList[i];
			}
		}
	},

	_isEntityEnabledForProject: function (entityTypeId, projectId) {
		var action = this.getActionByEntityTypeId(entityTypeId);

		if (action) {
			for (var i = 0; i < action.availableProjectIds.length; i++) {
				if (action.availableProjectIds[i] == projectId)
					return true;
			}
		}

		return false;
	},

	_adjustActionsForCurrentProject: function () {
		this._setupLinkAvailabilityForCurrentProject();
        this._setupLinksHrefForCurrentProject();
	},

    _setupLinkAvailabilityForCurrentProject: function () {
		Ext.select('div.actionList a.actionLink').each(function (element) {
            var projectId = this._getCurrentProjectId();
			if (this._isEntityEnabledForProject(parseInt(element.getAttribute('entityTypeId')), projectId)) {
				this._enableLink(element);
			}
			else {
				this._disableLink(element);
			}
			this._replaceWithTerm(element);
		}, this);
	},

	_disableLink: function (link) {
		var extLink = Ext.fly(link);
		extLink.addClass('disabledAction');
		extLink.set({ href: 'javascript:void(0);', disabled: 'disabled' }, true);
	},

	_enableLink: function (link) {
		var extLink = Ext.fly(link);
		extLink.removeClass('disabledAction');
		extLink.set({ href: extLink.getAttribute('navigateUrl') }, true);
		extLink.dom.removeAttribute('disabled');

	},

	_replaceWithTerm: function (link) {
		var extLink = Ext.fly(link);
		if (!extLink.getAttribute('defaultTerm')) {
			extLink.set({ 'defaultTerm': extLink.dom.innerHTML });
		}
		var name = extLink.getAttribute('defaultTerm');
		Ext.each(this.terms, function (term) {
			if (term.defaultTerm == name) {
				var processId = this._getCurrentProcessId();
				Ext.each(term.processTerms, function (processTerm) {
					if (processTerm.processId == processId) {
						name = processTerm.term;
					}
				}, this);
			}
		}, this);
		extLink.dom.innerHTML = name;
	}
});

Ext.onReady(function () {
	var triggerElement = Ext.get("quickAddLink");
	if (triggerElement) {
		quickAddMenuData.triggerElement = triggerElement;
		new Tp.controls.menu.QuickAddPopup(quickAddMenuData);
	}
});