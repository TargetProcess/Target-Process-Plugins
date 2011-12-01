ImageMinimizer = (function() {
	return {
		register: function(sel, maxWidth) {
			this.resizeImages(sel, maxWidth);
		},

		resizeImages: function(sel, maxWidth, notAddLink) {
			var images = Ext.query(sel);
			for (var n = 0; n < images.length; n++) {
				var img = Ext.fly(images[n]);
				img.focus = Ext.emptyFn;
				img.setVisibilityMode(Ext.Element.DISPLAY);
				img.setVisible(true, true);
				var scrollTopAfter = Ext.getBody().getScroll().top;
				var scrollLeftAfter = Ext.getBody().getScroll().left;
				if (img.getWidth() > maxWidth) {
					var ratio = img.getHeight() / img.getWidth();
					var newImgHeight = ratio * maxWidth;
					img.set({ width: 0, height: 0, rel: '' });
					img.setHeight(newImgHeight);
					img.setWidth(maxWidth);
					img.setStyle('display', 'block');
					img.applyStyles({ display: 'block' })

                    if(!notAddLink)
                    {
                        var anchor = new Ext.Element(document.createElement('a'));
                        anchor.set({ rel: 'lightbox', href: img.getAttribute('src') });
                        anchor.replace(img.dom);
                        anchor.appendChild(img.dom);
                    }
				}
			}
			var hash = window.location.hash;
			if (hash != null && String.isInstanceOfType(hash) && hash.length > 0) {
				hash = hash.replace(/^#/, "");
				if (hash.length > 0) {
					var anchor = Ext.get(hash);
					if (anchor != null)
						window.scrollTo(anchor.getAnchorXY()[0], anchor.getAnchorXY()[1]);
				}
			}
		}
	}
})()

Ext.onReady(function() {
	ImageMinimizer.resizeImages('img[rel^=mayNeedToResize]', 500);
	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(
		function() {
			ImageMinimizer.resizeImages('img[rel^=mayNeedToResize]', 500);
		}
	)
})
