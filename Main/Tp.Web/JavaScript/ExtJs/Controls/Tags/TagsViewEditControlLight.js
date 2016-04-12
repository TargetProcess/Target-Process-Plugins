Ext.ns("Tp.controls");

Tp.controls.TagsViewEditControlLight = Ext.extend(Ext.DataView, {
     constructor: function (config) {

        if(!Ext.get(config.applyTo))
            return;

        this.entityId = config.entityId;

        this.tpl = this.getTpl();

        config = Ext.apply({
            applyTo: config.applyTo,
            tpl: this.tpl,
            autoHeight:true,
            //this added for IE 7
            itemSelector:'view-item'
        }, config);

		Tp.controls.TagsViewEditControlLight.superclass.constructor.call(this, config);

        this.tagStore = new Ext.data.JsonStore({
			fields: ['name', 'primeId', 'relatedName', 'kind', 'allowToEdit'],
			data: config.tagData
		});
        this.setStore(this.tagStore);
    },

    getTpl: function(){
        return new Ext.XTemplate(
                '<tpl for=".">',
                    '<span class="light-tag view-item">',
                    '<tpl if="0 < primeId"><a href="{[Application.getViewUrl(values.primeId, values.kind)]}" title="{relatedName}">{name}</a></tpl>',
                    '<tpl if="primeId <= 0"><span>{name}</span></tpl>',
                    ', </span>',
                '</tpl>'
            );
    },

    refresh: function(){
        Tp.controls.TagsViewEditControl.superclass.refresh.apply(this, arguments);
        this.removeComma();
    },

    removeComma: function(){
        var tags = Ext.query('[class*=light-tag]', this.el.dom);
        tags[tags.length - 1].innerHTML = tags[tags.length - 1].innerHTML.replace('>,', '>');
    }
});
