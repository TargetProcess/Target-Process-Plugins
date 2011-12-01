


/**
* Specialized rich text editor field to use in custom report grid.
*/

Tp.controls.RichTextField = Ext.extend(Ext.form.HtmlEditor, {

	appended: false,

	okIdHolder: "bottomBarRicheditHolderOk",

	cancelIdHolder: "bottomBarRicheditHolderCancel",

	constructor: function(config) {
		Tp.util.validateForNulls([config]);
		config.enableAlignments = false;
		config.enableColors = false;
		config.enableFont = false;
		config.enableFontSize = false;
		Tp.controls.RichTextField.superclass.constructor.call(this, config);
		this.on("beforeshow", this.onShowEditor, this);
	},

	createNonExtJsButton: function(text, handler, elementContext) {
		var template = "<table class='x-btn-wrap x-btn' cellspacing='0' cellpadding='0' border='0' style='width: auto;'><tbody><tr>"
              + "<td class='x-btn-left'> </td>"
                      + "<td class='x-btn-center'><em unselectable='on'><button class='x-btn-text' type='button'>{0}</button></em></td>"
              + "<td class='x-btn-right'></td>"
        + "</tr></tbody></table>";
		Ext.DomHelper.insertHtml('beforeEnd', elementContext, String.format(template, text)).onclick = handler;
	},
            
    setValue:function(v){
        var value = v.replace(/src=[']~\//i,"src='" + appHostAndPath + "/").replace(/src=["]~\//i,"<img src=\"" + appHostAndPath + "/");
        Ext.form.HtmlEditor.prototype.setValue.call(this,value);
    },
        /*
    getValue:function(){
        
        var value = Ext.form.HtmlEditor.prototype.getValue.call(this);
        
        var reqexp1 = new RegExp("src='" + appHostAndPath + "/");
        var reqexp2 = new RegExp("src=\"" + appHostAndPath + "/");

        var value = v.replace(reqexp1,"src='~/").replace(reqexp2,"src=\"~/");        
        return value;
    },   */   
	createToolbarFrame: function() {
		var editorWrap = this.getEl().parent();
		var htmlBottomStr = String.format("<div class='x-toolbar'><center><table><tr><td id='{0}' ></td><td id='{1}' ></td></tr></table></center></div>", this.okIdHolder, this.cancelIdHolder);
		Ext.DomHelper.insertHtml('beforeEnd', editorWrap.dom, htmlBottomStr);
	},

	makeEditorFrameTopElement: function() {
		var editorFrame = this.getEl().parent().parent();
		document.body.appendChild(editorFrame.dom);
	},

	onShowEditor: function() {
		if (this.appended)
			return;
		this.getToolbar().enable();
		this.makeEditorFrameTopElement();
		this.createToolbarFrame();
		this.createNonExtJsButton("Ok", this.onOkClick.createDelegate(), Ext.fly(this.okIdHolder).dom);
		this.createNonExtJsButton("Cancel", this.onCancelClick.createDelegate(), Ext.fly(this.cancelIdHolder).dom);
		this.appended = true;
	},

	onOkClick: function() {
		Tp.controls.grid.CurrentEditableItem.getInstance().grid.stopEditing();
	},

	onCancelClick: function() {
		Tp.controls.grid.CurrentEditableItem.getInstance().grid.stopEditing(true);
	}

});

Ext.reg('tprichtextfield', Tp.controls.RichTextField);