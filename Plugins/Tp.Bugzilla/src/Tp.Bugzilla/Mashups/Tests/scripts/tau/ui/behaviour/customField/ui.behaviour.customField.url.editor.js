define(["Underscore","jQuery"],function(a,b){b.widget("ui.behaviourUrlEditor",{options:{onSave:b.noop},_create:function(){var b=this.element;this.$element=b;var c=this;b.find(":button:first").click(function(b){b.preventDefault(),a.bind(c._save,c)()}),b.find(":input").keyup(function(b){b.keyCode==jQuery.ui.keyCode.ENTER&&(b.preventDefault(),a.bind(c._save,c)())})},_save:function(){var a=this.element;this.options.onSave({url:a.find("[name=url]").val(),label:a.find("[name=label]").val()})},setValidationErrors:function(b){var c=this.$element;a.each(b,function(a,b){c.find("[name="+b+"]").addClass("ui-validationerror").attr("title",a)})},resetValidationErrors:function(a){this.$element.find(":text").removeClass("ui-validationerror").attr("title",null)},setValue:function(b){var c=this.$element;a.each(b,function(a,b){c.find("[name="+b+"]").val(a)})},destroy:function(){}})})