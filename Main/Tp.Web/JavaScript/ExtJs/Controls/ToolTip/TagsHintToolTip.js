Ext.ns('Tp.controls.ToolTip');

Tp.controls.ToolTip.TagsHintToolTipManager = function () {
	var inited = [];
	var tagsHints = [];

	return new function() {
		this.init = function(targetId) {
			if (!inited.contains(targetId)) {
				tagsHints.push({ id: targetId, hint: new Tp.controls.ToolTip.TagsHintToolTip({ targetId: targetId }) });
				inited.push(targetId);
			}
			else {
				Ext.each(tagsHints, function(tag) {
					if (tag.id == targetId)
						tag.hint.refreshTargetId(targetId);
				});
			}
		};
	};
} ();

Tp.controls.ToolTip.TagsHintToolTip = Ext.extend(Tp.controls.SimplePopup, {

	detailsCloseLinkId: null,

	constructor: function (config) {
		this.target = Ext.get(config.targetId);
		this.detailsCloseLinkId = config.targetId + 'close';

		config = Ext.apply({
			cls: 'hint-tool-tip',
			style: {
				position: 'absolute',
				zIndex: 10000000
			},

			items: [
                {
                	xtype: 'box',
                	html: '<div class="interim-menu-arrow">◢◣</div>' +
                    '<div class="tags-hint-tool-tip"><div class="tags-hint-tool-tip-box"><table><tr><td><strong>Hint</strong></td><td>' +
                    String.format('<a id="{0}" class="details-close-icon" href="javascript:void(0)"></a>', this.detailsCloseLinkId) +
                    '</td><tr><td colspan="2"><div class="hint-text">You can use a wild card (<strong style="font-size: 14px; font-weight: bold;">*</strong>) in your search query. (E.g., when you search for "dupl*" all items that start with "dupl" will appear.)</div></td></tr></table></div></div>'
                }
            ]
		}, config);

		this.target.on('mousedown', this.onClickTarget, this);

		Tp.controls.ToolTip.TagsHintToolTip.superclass.constructor.call(this, config);
	},

	refreshTargetId: function (targetId) {
		var me = this;
		if (me.target.dom == Ext.getDom(targetId)) {
			return;
		}

		me.target = Ext.get(targetId);
		me.target.on('mousedown', me.onClickTarget, me);
	},

	afterRender: function () {
		Tp.controls.ToolTip.TagsHintToolTip.superclass.afterRender.apply(this, arguments);
		Ext.get(this.detailsCloseLinkId).on('mousedown', this.onHideClick, this);
	},

	onClickTarget: function () {
		if (!this.rendered) {
			this.render(Ext.getBody());
		}

		var x = this.target.getLeft(false);
		var y = this.target.getTop(false) + this.target.getHeight(false);

		this.showForCoordinates(x - 20, y);
	},

	onHideClick: function () {
		this.hide();
	}
});
