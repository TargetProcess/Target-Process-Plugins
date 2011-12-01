Ext.ns('Tp.controls');

var stopDropDownPanelHiding = false;

Tp.controls.MenuBootstrap = Ext.extend(Object, {
    POPUP_AREA_CLASS:"dropDownPanel",
    popupWindows:[],
    registerPopup:function(popup){
        this.popupWindows.push(popup);

        popup.addClass(this.POPUP_AREA_CLASS);
        popup.getTriggerElement().on('mousedown', this.onTriggerClick, popup);
        popup.getTriggerElement().on('mouseup', function(a) {
            a.stopPropagation();
        });

        popup.getContainerElement().on('mousedown', function(a) {
            a.stopPropagation();
        });
        popup.getContainerElement().on('mouseup', function(a) {
            a.stopPropagation();
        });
    },


    onTriggerClick: function (a, b) {
        a.stopPropagation();
        this.toggle();        
       
    },

    _onBodyMouseUpHandler:function(eventObj,element) {
        if (stopDropDownPanelHiding)
		    return;
        Ext.each(this.popupWindows,function(window){

                if(window.isVisible()){
                    window.hide();
                }
            });
    },
    
	_initBodyMouseUp: function() {
		Ext.getBody().on("mousedown", this._onBodyMouseUpHandler,this);
	},

	_getDropDownLink: function(eventObj) {
		var element = Ext.fly(eventObj.getTarget());
		if (!element.hasClass('ddLink')) {

            if (element.parent().hasClass('ddLink'))
                return element.parent();

            return element.child(".ddLink");
        }
        return element;
    },


    _positionPanel: function(dropDownPanel, parentLink) {
        var browserWidth = Ext.lib.Dom.getViewWidth();
        if (parentLink.getX() + dropDownPanel.getWidth() > browserWidth) {
            dropDownPanel.setX(browserWidth - dropDownPanel.getWidth());
            dropDownPanel.setY(parentLink.getY() + parentLink.getHeight());
        }
        else {
            dropDownPanel.setLeft('');
            dropDownPanel.setTop('');
        }
    },

	_hidePanel: function(node) {
		if (node.isVisible()) {
			node.hide();
			node.parent().removeClass('popupTab');
		}
	},
    _initProjectContextLink: function() {
		if (_projectContext && _projectContext.length > 1) {
			var clickHandler = function() {
				var link = this;
				if (!link._projectContextMenu) {
					var tpl = new Ext.XTemplate(
						'<div><tpl for="."><a href="',
						link.href || "#",
						'?ProjectID={id}">{name}</a><br></tpl></div>'
					);
					link.href = "javascript:void(0);";
					link._projectContextMenu = Ext.DomHelper.createDom({ tag: 'div', cls: 'morePanel', style: 'padding: 5px' }, Ext.getBody());
					tpl.overwrite(link._projectContextMenu, _projectContext);
				}
				Tp.Atlas.Extenders.HoverBehavior.show(link, link._projectContextMenu);
			}
			Ext.select('.projectContextLink').on('click', clickHandler);
		}
    },

    _initInfoToggle: function() {
        var elements = Ext.getBody().select(".help").elements;
        if (elements.length > 0) {
            Ext.getBody().select(".infoLink").setStyle('display', '');
        }
        else {
            Ext.getBody().select(".infoLink").setStyle('display', 'none');
        }

        var collapsed = true;
        var clickHandler = function(eventObj, sender) {
            var senderElement = Ext.get(sender);
            var helpPanel = Ext.getBody().select(".help");

            if (collapsed) {
                senderElement.update("Hide info");
                helpPanel.setVisibilityMode(Ext.Element.VISIBILITY);
                helpPanel.show(true);
                collapsed = false;
            }
            else {
                senderElement.update("Show this page info");
                helpPanel.setVisibilityMode(Ext.Element.DISPLAY);
                helpPanel.hide(true);
                collapsed = true;
            }
        };
        var link = Ext.getBody().select(".infoLink");
        link.on("click", clickHandler);
    },

	init: function() {
		this._initBodyMouseUp();
		this._initInfoToggle();
	}
});
