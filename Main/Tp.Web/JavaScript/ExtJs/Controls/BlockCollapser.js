Ext.ns('Tp.controls');

Tp.controls.BlockCollpaser = Ext.extend(Object, {
	imageElement: null,
	containerElement: null,
	
	constructor: function(imageElement, containerElement) {
		this.imageElement = imageElement;
		this.containerElement = containerElement;

		this.containerElement.setVisibilityMode(Ext.Element.DISPLAY);
		this.imageElement.on("click", Function.createDelegate(this, this.onShowProjectItemsDiv));
	},

	onShowProjectItemsDiv: function(e, target)
	{
		if (this.containerElement.isVisible())
		{
			this.imageElement.dom.src = this.imageElement.dom.src.replace("minus-small.png", "plus-small.png");
			this.containerElement.hide();
		}
		else
		{
			this.imageElement.dom.src = this.imageElement.dom.src.replace("plus-small.png", "minus-small.png");
			this.containerElement.show();
		}
	}
});