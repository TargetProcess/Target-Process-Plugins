Ext.ns('UseCaseHelp');

UseCaseHelp.ToolTip = Ext.extend(Ext.ToolTip, {
	constructor: function(config) {
		UseCaseHelp.ToolTip.superclass.constructor.apply(this, arguments);
		this.autoHide = false;
		this.closable = true;
		this.draggable = false;
		this.targetXY = [0, 0];
		this.layoutHandler = null;
		this.baseCls = 'usecasehelp x-tip';
	},

	showHelp: function() {
		this.layoutHandler = UseCaseHelp.ToolTipLayout.getInstance(this);
		this.layoutHandler.show();
	},

	onTargetOver: function() {
		//override base behavior. Do not show tooltip on onmouseover event  
	},

	onHide: function() {
		UseCaseHelp.ToolTip.superclass.onHide.call(this);
		this.disposeLayoutHandler();
	},

	disposeLayoutHandler: function() {
		if (this.layoutHandler)
			this.layoutHandler.dispose();
	}
});
