UseCaseHelp.ToolTipItem = Ext.extend(Ext.Component, {
	constructor: function(config) {
		UseCaseHelp.ToolTipItem.superclass.constructor.apply(this, arguments);
		if (this.initialConfig.Selector == null || this.initialConfig.Selector == "")
			throw "UseCaseHelp.ToolTipItem: Selector is null or empty.";

		if (this.initialConfig.ArrowUpID == null || this.initialConfig.ArrowUpID == "")
			throw "UseCaseHelp.ToolTipItem: arrowUpID is null or empty.";

		if (this.initialConfig.ArrowDownID == null || this.initialConfig.ArrowDownID == "")
			throw "UseCaseHelp.ToolTipItem: arrowDownID is null or empty.";

		if (!this.isTargetElementExist())
			throw "UseCaseHelp.ToolTipItem: target element does not exist.";

		this.target = Ext.get(this.initialConfig.TargetID);
		this.target.on('click', this.onTargetClick, this);
		this.arrowUpId = this.initialConfig.ArrowUpID;
		this.arrowDownId = this.initialConfig.ArrowDownID;
		this.title = this.initialConfig.Title;
		this.description = this.initialConfig.Description;
		this.alternativeDescription = this.initialConfig.AlternativeDescription;
		this.helpElementSelector = this.Selector;
		UseCaseHelp.ToolTipItemManagerFactory.getInstance().register(this);
	},

	onTargetClick: function() {
		this.showToolTip();
	},

	onToolTipHide: function() {
		this.target.applyStyles("font-weight:normal;");
	},

	onToolTipShow: function() {
		this.target.applyStyles("font-weight:bold;");
	},

	isTargetElementExist: function() {
		return (Ext.get(this.initialConfig.TargetID) == null) ? false : true;
	},

	showToolTip: function() {
		// in case if someone close UseCaseHelpPanel.
		if (!this.isTargetElementExist()) {
			this.destroyActiveTooltip();
			return;
		}

		var toolTip = new UseCaseHelp.ToolTip(this.collectToolTipParams());
		this.signForEvents(toolTip);

		this.onBeforeShowToolTip(toolTip);
		toolTip.showHelp();
		this.onAfterShowToolTip(toolTip);
	},

	destroyActiveTooltip: function() {
		if (UseCaseHelp.ToolTipItem.activeToolTip)
			UseCaseHelp.ToolTipItem.activeToolTip.destroy();
	},

	onBeforeShowToolTip: function(toolTip) {
		this.destroyActiveTooltip();
	},

	onAfterShowToolTip: function(toolTip) {
		UseCaseHelp.ToolTipItem.activeToolTip = toolTip;
	},

	signForEvents: function(toolTip) {
		toolTip.on('show', this.onToolTipShow, this);
		toolTip.on('beforedestroy', this.onToolTipHide, this);
	},

	collectToolTipParams: function() {
		var helpContext = this.getHelpContext();
		var params = new Object();
		params.html = helpContext.htmlDescription;
		params.target = helpContext.helpElement;
		params.title = this.title;
		params.arrowUpId = this.arrowUpId;
		params.arrowDownId = this.arrowDownId;
		return params;
	},

	getHelpContext: function() {
		var element = this.findElement(this.helpElementSelector);
		var htmlDescription = Ext.util.Format.htmlDecode(this.description);

		var helpElement;
		if (element) {
			helpElement = element;
		}
		else {
			helpElement = this.target;
		}

		htmlDescription += this.getNextPrevHtml();
		var result = new Object();
		result.helpElement = helpElement;
		result.htmlDescription = htmlDescription;
		return result;
	},

	getNextPrevHtml: function() {
		var nextId = UseCaseHelp.ToolTipItemManagerFactory.getInstance().getNextToolTipItemId(this.id);
		var prevId = UseCaseHelp.ToolTipItemManagerFactory.getInstance().getPrevToolTipItemId(this.id);

		var html = "<div style='text-align:right;'>&lt;&nbsp;"
		if (prevId)
			html += "<a href='javascript:Ext.ComponentMgr.get(\"" + prevId + "\").showToolTip();'>"
		else
			html += "<a href='#' style='color:grey;text-decoration:none'  onclick='return false;' disabled='true' >";
		html += "prev</a>&nbsp;|&nbsp;";

		if (nextId)
			html += "<a href='javascript:Ext.ComponentMgr.get(\"" + nextId + "\").showToolTip();'>"
		else
			html += "<a href='#' style='color:grey;text-decoration:none' onclick='return false;' disabled='true' >";
		html += "next</a>";

		html += "&nbsp;&gt;&nbsp;</div>";
		return html;
	},

	findElement: function(selector) {
		var result = Ext.query(selector);

		var visible = [];

		for (var i = 0; i < result.length; i++) {
			var e = Ext.get(result[i]);

			if (e.isVisible() && e.findParent("div{display=none}") == null) {
				visible.push(e);
			}
		}

		if (visible.length == 0)
			return null;

		return visible[0];
	}
})
