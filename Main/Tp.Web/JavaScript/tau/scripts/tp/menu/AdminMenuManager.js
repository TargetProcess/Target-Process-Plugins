define(["tp/menu/MenuManager","libs/jquery/jquery"],function(a){function b(a){this._create(a)}b.prototype={_create:function(b){this._itemText=b.itemText,this._itemUrl=(new Tp.WebServiceURL(b.itemUrl)).url,this._identifyUrlPart=b.identifyUrlPart,this._menuManager=new a({menu:$('[id="System Settings Action Group"]')}),this._item='<tr><td><div><a href="'+this._itemUrl+'">'+this._itemText+"</a></div></td></tr>"},addItem:function(){this._menuManager.addItem(this._item),this._fixSelection()},addItemAfter:function(a){this._menuManager.addItemAfter($('a:[text="'+a+'"]').parent().parent().parent(),this._item),this._fixSelection()},_fixSelection:function(){this._menuManager.fixSelection(this._identifyUrlPart,this._itemText)}};return b})