define(["Underscore","jQuery"],function(a,b){b.widget("ui.behaviourCustomFieldRow",{options:{onActivate:b.noop},_create:function(){var a=this.element;this.$element=a;var b=this;a.click(function(a){a.preventDefault(),b.options.onActivate.call(this,a)})},setValidationErrors:function(b){var c=this.$element;a.each(b,function(a,b){c.find("[name="+b+"]").addClass("ui-editableText-validationerror").attr("title",a)})},resetValidationErrors:function(a){this.$element.find(":text").removeClass("ui-editableText-validationerror").attr("title",null)},setValue:function(b){var c=this.$element;a.each(b,function(a,b){c.find("[name="+b+"]").val(a)})},destroy:function(){}})})