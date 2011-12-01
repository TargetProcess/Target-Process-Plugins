define(["libs/jquery/jquery.ui"],function(a){(function(a,b){a.widget("ui."+b,{options:{message:"Question?",okLabel:"OK",cancelLabel:"Cancel"},_create:function(){if(a.type(this.options.message)!="string")throw new TypeError('Invalid confirmation message type. A "string" expected.');this.element.addClass(b+"Host"),this._$widget=this._bindBehaviour(this._createTemplate())},_createTemplate:function(){var c=a('<div class="'+b+'"/>'),d=a('<div class="inner"/>'),e=a("<p/>").text(this.options.message),f=a('<button type="button" class="ok button mr-5 danger">').text(this.options.okLabel),g=a('<button type="button" class="cancel button">').text(this.options.cancelLabel);return c.append(d.append(e,f,g))},_bindBehaviour:function(b){b.click(a.proxy(function(b){b.stopPropagation(),b.preventDefault();var c=a(b.target);if(c.is(".ok"))return this._trigger("ok");if(c.is(".cancel"))return this._trigger("cancel")},this));return b},show:function(){this._$widget.appendTo(this.element),this._trigger("show")},hideConfirmationMessage:function(){this._$widget.find(".inner").hide()},hide:function(){this._$widget.detach(),this._trigger("hide")},destroy:function(){a.Widget.prototype.destroy.apply(this,arguments),this._$widget.remove(),this.element.removeClass(b+"Host")}})})(a,"confirmation");return a})