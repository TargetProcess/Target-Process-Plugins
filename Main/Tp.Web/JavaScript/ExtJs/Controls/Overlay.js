Tp.controls.Overlay = Ext.extend(Object,{
    constructor:function(){

    },

    show:function(){
        var overlay = Ext.get("overlay");
		overlay.addClass("overlay");

        var bodyHeight = Ext.getBody().getHeight();

        if (document.documentElement && document.documentElement.clientHeight
                && document.documentElement.clientHeight > bodyHeight)
        {
            bodyHeight = document.documentElement.clientHeight;
        }

        if (window.innerHeight && window.innerHeight > bodyHeight)
            bodyHeight = window.innerHeight;
        
		if (overlay.getHeight() < bodyHeight)
			overlay.setHeight(bodyHeight);

        Ext.EventManager.onWindowResize(this.show, this)
    },

    hide:function(){
        var overlay = Ext.get("overlay");
        overlay.removeClass("overlay");
        overlay.setHeight(0);

        Ext.EventManager.removeResizeListener(this.show, this);
    },

    getZIndex: function(){
        var rule = Ext.util.CSS.getRule('.overlay');
        return rule.style.zIndex;
    }
});

Tp.controls.getOverlay = function(){
	return overlayControl;
}

var overlayControl = new Tp.controls.Overlay();
