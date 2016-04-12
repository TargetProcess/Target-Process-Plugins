Ext.ns('UseCaseHelp.ImageHelper');

UseCaseHelp.ImageHelper = Ext.extend(Object, {
	isImageExist: function (id) {
		return (Ext.fly(id) == null) ? false : true;
	},

	showImage: function (id, xy) {
		if (!this.isImageExist(id))
			return;
		var imageElement = new Ext.Element(id);
		imageElement.applyStyles("display:block;")
		imageElement.position("absolute", 100000, xy[0], xy[1]);
	},

	hideImage: function (id) {
		if (!this.isImageExist(id))
			return;
		new Ext.Element(id).applyStyles("display:none;")
	},

	getImageWidth: function (id) {
		if (!this.isImageExist(id))
			return;
		var imageElement = new Ext.Element(id);
		imageElement.applyStyles("display:block;")
		var width = imageElement.getWidth();
		imageElement.applyStyles("display:none;")
		return width;
	},

	getImageWidth: function (id) {
		if (!this.isImageExist(id))
			return;
		var imageElement = new Ext.Element(id);
		imageElement.applyStyles("display:block;")
		var width = imageElement.getWidth();
		imageElement.applyStyles("display:none;")
		return width;
	},

	getImageHeight: function (id) {
		if (!this.isImageExist(id))
			return;
		var imageElement = new Ext.Element(id);
		imageElement.applyStyles("display:block;")
		var width = imageElement.getHeight();
		imageElement.applyStyles("display:none;")
		return width;
	}
})

UseCaseHelp.ImageHelper.getInstance = function () {
	if (UseCaseHelp.ImageHelper.instance == null)
		UseCaseHelp.ImageHelper.instance = new UseCaseHelp.ImageHelper();
	return UseCaseHelp.ImageHelper.instance;
}
