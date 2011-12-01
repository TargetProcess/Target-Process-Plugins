Ext.ns("Tp.controls.relatedEntities");

Tp.controls.relatedEntities.RelatedEntityList = Ext.extend(Ext.DataView, {
    constructor: function (config) {
        this.entityId = config.entityId;

        this.tpl = new Ext.XTemplate(
            '<div class="pt-10 pb-20">',
            '<tpl><div class="h3 pb-5">Duplicates</div></tpl>',
            '<tpl for=".">',
                '<div class="related-item" style="padding: 3px 10px; display: block; border-bottom: 1px solid #ddd">',
                '<tpl if="allowToEdit == 1"><div style="width: 60px; float: right; padding: 2px 5px 2px 15px;"><a href="javascript:void(0)" name="{id}" class="remove-link-relation" title="Unduplicate">Unduplicate</a></div></tpl>',
                '<tpl><img class="x-item-icon" src="' + Application.baseUrl + '/{iconSrc}" />&nbsp;&nbsp;&nbsp;<a class="pr-5" href="{[Application.getViewUrl(values.id, values.kind)]}" name="{kind}">{id}</a></tpl>',
                '<tpl><span>{[Ext.util.Format.htmlEncode(values.name)]}</span></tpl>',
                '</div>',
            '</tpl>',
            '</div>'
        );

        config = Ext.apply({
            applyTo: config.applyTo,
            tpl: this.tpl,
            autoHeight:true,
            //this added for IE 7
            itemSelector:'related-item'

        }, config);

        RelatedEntitiesStore.getStore(this.entityId, this);

		Tp.controls.relatedEntities.RelatedEntityList.superclass.constructor.call(this, config);
    },    

    refresh: function(){      
        Tp.controls.TagsViewEditControl.superclass.refresh.apply(this, arguments);
        this.subscribeRemoveLinks();
    },

    subscribeRemoveLinks: function(){
        var divs = Ext.query('div[class=related-item]', this.el.dom);
        Ext.each(divs, function (div) {
            var element = Ext.get(div);

            element.addClassOnOver('related-item-hover');
        }, this);

        var removeLinks = Ext.query('a[class=remove-link-relation]', this.el.dom);
        Ext.each(removeLinks, function (a) {
            var element = Ext.get(a);
            element.on('click', this.deleteRelation, this);
        }, this);        
    },

    deleteRelation: function(e,t,o){
        RelatedEntitiesStore.removeRelation(this.entityId, parseInt(t.name), this);
    },

    onDataLoadSuccess: function(response){
        this.entitiyStore = new Ext.data.JsonStore({
			fields: ['id', 'name', 'owner', 'kind', 'iconSrc', 'allowToEdit'],
			data: Ext.util.JSON.decode(response.responseText).d
		});

        this.setStore(this.entitiyStore);
    },

    onRemoveRelationSuccess: function(){
        RelatedEntitiesStore.getStore(this.entityId, this);
    },

    onRemoveRelationFail: function(response){          
        SetLastWarning("There was an error during relation removing.");
    },

    onDataLoadFail: function(response, opts){
        if(response.status === 0)
            return;
        SetLastWarning("There was an error during Duplicates getting.");
    }
});

 RelatedEntitiesStore = function() {

    return new function() {

        this.getStore = function(entityId, context) {
            Ext.Ajax.request({
                url: new Tp.WebServiceURL('/PageServices/RelatedEntitiesService.asmx/GetDataFor').toString(),
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                success: Function.createDelegate(context, context.onDataLoadSuccess),
                failure: Function.createDelegate(context, context.onDataLoadFail),
                jsonData:
                {
                    'entityId': entityId
                }
            });
        };

        this.removeRelation = function(primeId, relatedId, context){
            Ext.Ajax.request({
                url: new Tp.WebServiceURL('/PageServices/RelatedEntitiesService.asmx/RemoveRelation').toString(),
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                success: Function.createDelegate(context, context.onRemoveRelationSuccess),
                failure: Function.createDelegate(context, context.onRemoveRelationFail),
                jsonData:
                {
                    'primeId': primeId,
                    'relatedId': relatedId
                }
            });
        };
    }
}();
