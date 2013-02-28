Ext.ns('Tp.controls.kanbanboard');

/**
* An item such as <em>Task</em> and <em>Bug</em> that is displayed on a backlog or swimlanes.
*/
Tp.controls.kanbanboard.Item = Ext.extend(Ext.BoxComponent, {
	/**
	* Specify visual oppearance of an item element with a particular CSS class.
	*/
	cls: 'kanban-item',

	kanbanCardTemplate: new Ext.XTemplate(
			'<tpl for=".">',
				'<div class="kanban-item-title">',
					'<div class="kanban-id-count">',
						'<a class="kanban-item-id" href="' + appHostAndPath + '/View.aspx?id={id}">{id}</a>',
						'<tpl if="userStoryId">',
							'&nbsp;for&nbsp#',
							'<a class="kanban-item-id" href="' + appHostAndPath + '/View.aspx?id={userStoryId}">{userStoryId}</a>',
						'</tpl>',
					'</div>',
					'<tpl if="tags.length &gt; 0">',
						'<tpl for="tags">',
							'<div class="kanban-tags">',
								'<div class="tag-space"><div class="pin"></div><div class="kanban-tag">{first}</div></div>',
								'<tpl if="length &gt; 1">',
									'<div class="tag-space"><div class="pin"></div><div class="kanban-tag">&#133;</div></div>',
								'</tpl>',
							'</div>',
						'</tpl>',
					'</tpl>',
				'</div>',
				'<div class="name">{name}</div>',
				'<tpl if="cardUsers.length &gt; 0">',
					'<div class="kanban-avatars">',
						'<tpl for="cardUsers">',
							'<div class="kanban-avatar{active}"><img src="../../../avatar.ashx?size=22&UserId={id}&mode=raw"/></div>',
						'</tpl>',
					'</div>',
				'</tpl>',
				'<tpl if="bugCount &gt; 0 || taskCount &gt; 0 || impedimentCount &gt; 0">',
					'<tpl if="rolesCount &gt; 0">',
						'<div class="tasks-bugs">',
					'</tpl>',
					'<tpl if="rolesCount == 0">',
						'<div class="tasks-bugs tasks-bugs-left">',
					'</tpl>',
						'<tpl if="taskCount &gt; 0" >',
							'<a class="taskCountLink" href="javascript:void(0);"><span class="taskCount">{taskCount}</span></a>',
						'</tpl>',
						'<tpl if="bugCount &gt; 0" >',
							'<a class="bugCountLink" href="javascript:void(0);"><span class="bugCount">{bugCount}</span></a>',
						'</tpl>',
						'<tpl if="impedimentCount &gt; 0" >',
							'<a class="impedimentCountLink" href="javascript:void(0);"><span class="impedimentCount">{impedimentCount}</span></a>',
						'</tpl>',
					'</div>',
				'</tpl>',
			'</tpl>'
		),
	tooltipTemplate: new Ext.XTemplate(
			'<div>',
			'<div><strong id="entityType">{entityTypeTitle}</strong> ',
				'<span class="kanban-item-id">#{id}</span>',
				'&nbsp;',
				'<span class="kanban-item-name">{name}</span>',
			'</div>',
			'<tpl if="!userStoryId">',
				'<div class="pt-5"><strong id="bv">BV:</strong>{priorityName}</div>',
			'</tpl>',
			'<tpl if="userStoryId">',
				'<div class="pt-5"><strong id="bv">US </strong>#{userStoryId} {userStoryName}</div>',
			'</tpl>',
			'<tpl if="bugCount &gt; 0 || taskCount &gt; 0 || impedimentCount &gt; 0">',
				'<div class="pt-5">',
					'<tpl if="taskCount &gt; 0" >',
						'<strong id="task">Task:</strong> {taskCount}&nbsp;',
					'</tpl>',
					'<tpl if="bugCount &gt; 0" >',
						'<strong id="bug">Bug:</strong> {bugCount}&nbsp;',
					'</tpl>',
					'<tpl if="impedimentCount &gt; 0" >',
						'<strong id="impediment">Impediment:</strong> {impedimentCount}',
					'</tpl>',
				'</div>',
			'</tpl>',
			'<tpl if="tags.length &gt; 0">',
				'<div class="pt-5">',
					'<tpl for="tags">',
						'<strong id="tags">Tags:</strong> {list}',
					'</tpl>',
				'</div>',
			'</tpl>',
			'<tpl if="roles.length &gt; 0">',
				'<div>',
					'<tpl for="roles">',
						'<table cellspacing="0" cellpadding="0" class="pt-5">',
							'<tr>',
								'<tpl if="second">',
									'<td rowspan="2" valign="top">',
								'</tpl>',
								'<tpl if="!second">',
									'<td>',
								'</tpl>',
									'<strong>{roleName}:&nbsp;</strong> ',
								'</td>',
								'<tpl for="first">',
									'<td>{name}</td>',
								'</tpl>',
							'</tr>',
							'<tpl if="second">',
								'<tpl for="second">',
									'<tr><td>{name}</td></tr>',
								'</tpl>',
							'</tpl>',
						'</table>',
					'</tpl>',
				'</div>',
			'</tpl>',
			'</div>'
		),

	constructor: function (config) {
		if (!config) {
			throw new Error('Config not specified');
		}
		if (!config.entity) {
			throw new Error('Entity not specified');
		}
		if (!config.entity.entityState) {
			throw new Error('Entity state not specified');
		}

		var id = 'kanban-item-' + config.entity.entityType.name.toLowerCase() + '-' + config.entity.id;
		Ext.apply(config, {
			id: id,
			itemId: id
		});

		Tp.controls.kanbanboard.Item.superclass.constructor.call(this, config);
		this.addEvents("onBugsPopupShow");
		this.addEvents("onTasksPopupShow");
		this.addEvents("onImpedimentsPopupShow");
	},

	updateBugs: function (bugsCount) {
		this.entity.bugCount = bugsCount;
		this.onRenderHandler();
	},

	updateTasks: function (tasksCount) {
		this.entity.taskCount = tasksCount;
		this.onRenderHandler();
	},

	updateImpediments: function (impedimentsCount) {
		this.entity.impedimentCount = impedimentsCount;
		this.onRenderHandler();
	},

	//private
	onRender: function (ct, position) {
		Tp.controls.kanbanboard.Item.superclass.onRender.call(this, ct, position);
		this.onRenderHandler();
	},

	getEntityName: function () {
		return this.entity.name;
	},

	getRoleCount: function (roles, roleId) {
		var count = 0;
		for (var i = 0; i < roles.length; i++) {
			count += roles[i].id == roleId ? 1 : 0;
		}
		return count;
	},

	nextVisible: function () {
		var n = null, c = this.ownerCt.items;
		if (c.length > 1) {
			var i = c.indexOf(this);
			while (i != c.length - 1) {
				n = c.itemAt(++i);
				if (n && n.isVisible()) {
					break;
				}
			}
		}
		return n && n.isVisible() ? n : undefined;
	},

	prevVisible: function () {
		var p = null, c = this.ownerCt.items;
		if (c.length > 1) {
			var i = c.indexOf(this);
			while (i != 0) {
				p = c.itemAt(--i);
				if (p && p.isVisible()) {
					break;
				}
			}
		}
		return p && p.isVisible() ? p : undefined;
	},

	onRenderHandler: function () {
		var tags = this.entity.tags || [];
		var roles = this.entity.roles || [];
		var cardUsers = this.prepareCardUsers();
		var model = {
			id: this.entity.id,
			name: Ext.util.Format.htmlEncode(this.entity.name),
			entityState: this.entity.entityState,
			entityTypeId: this.entity.entityType.id,
			entityTypeName: Ext.util.Format.htmlEncode(this.entity.entityType.name),
			entityTypeTitle: this.entity.entityType.title,
			priorityName: Ext.util.Format.htmlEncode(this.entity.priorityName),
			projectName: Ext.util.Format.htmlEncode(this.entity.projectName),
			priorityImportance: Ext.util.Format.htmlEncode(this.entity.priorityImportance),
			viewUrl: Application.getViewUrl(this.entity.id, this.entity.entityType.name),
			bugCount: this.entity.bugCount,
			taskCount: this.entity.taskCount,
			impedimentCount: this.entity.impedimentCount,
			roles: roles,
			cardUsers: cardUsers,
			tags: {
				length: tags.length,
				first: tags.length > 0 ? tags[0] : "",
				list: tags.join(", ")
			},
			rolesCount: this.getRoleCount(roles, this.entity.entityState.roleId),
			userStoryId: this.entity.userStoryId || false,
			userStoryName: this.entity.userStoryName
		};
		var entityType = this.entity.entityType.name.toLowerCase();
		this.addClass('kanban-item-entitytype-' + entityType);
		if (entityType == 'task') {
		}
		else {
            this.removeClass(this.currentPriorityImportanceCls);
            this.currentPriorityImportanceCls = 'kanban-item-priority-' + this.entity.priorityImportance;
			this.addClass(this.currentPriorityImportanceCls);
		}
		this.addClass('kanban-item-name');

		this.kanbanCardTemplate.overwrite(this.el, model);

		Array.forEach(this.getEl().query("a.bugCountLink"),
			function (link) {
				Ext.get(link).on("click", Function.createDelegate(this, this.showBugPopup));
			},
			this);

		Array.forEach(this.getEl().query("a.taskCountLink"),
			function (link) {
				Ext.get(link).on("click", Function.createDelegate(this, this.showTaskPopup));
			},
			this);

		Array.forEach(this.getEl().query("a.impedimentCountLink"),
			function (link) {
				Ext.get(link).on("click", Function.createDelegate(this, this.showImpedimentPopup));
			},
			this);

		this.el.unselectable();

		var self = this;

		if (!this.tip) {
			this.tip = new Ext.ToolTip({
				target: this.el,
				html: this.tooltipTemplate.apply(model),
				width: 250,
				dismissDelay: 10000,
				showDelay: 500,
				listeners: {
					beforeshow: function updateTipBody() {
						if (!self.canShowTooltip()) {
							if (!this.anchorEl)
								this.anchorEl = { hide: Ext.emptyFn };
							return false;
						}
					}
				}
			});
		}

		if (!this.signedOnDblClick) {
			this.el.on('dblclick', this.showDetails, this);
		}

		this.signedOnDblClick = true;
	},

	showDetails: function () {
		var data = this.initialConfig.entity;
		this.fireEvent('showDetails', { entity: data });
	},

	showBugPopup: function () {
		this.fireEvent('onBugsPopupShow', this);
	},

	showTaskPopup: function () {
		this.fireEvent('onTasksPopupShow', this);
	},

	showImpedimentPopup: function () {
		this.fireEvent('onImpedimentsPopupShow', this);
	},

	prepareCardUsers: function () {
		if (this.entity.entityState.final) {
			return [];
		}
		var roles = this.entity.roles || [];
		var cardRole = this.entity.entityState.roleId;
		var cardUsers = [];
		var props = ['first', 'second'];
		Ext.each(roles.slice().sort(function (a, b) {
			return cardRole == a.id ? -1 : 1;
		}), function (role) {
			for (var key in props) {
				var prop = props[key];
				if (role[prop] && cardUsers.length < 4) {
					var user = role[prop];
					if (!Array.findOne(cardUsers, function (u) { return u.id == user.id; })) {
						user.active = cardRole == role.id ? '' : ' inactive';
						cardUsers.push(user);
					}
				}
			}
		});
		return cardUsers;
	}
});
