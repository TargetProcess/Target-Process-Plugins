define(["Underscore","jQuery","tau/components/extensions/component.extension.base"],function(_,$,ExtensionBase){return ExtensionBase.extend({"bus $form.ready":function(evt,$form){var self=this;$form.delegate(".tau-chart-time-switch button","click",function(){$form.find(".tau-chart-time-switch button").removeClass("tau-checked"),$(this).addClass("tau-checked"),self.fire("time.form.changed")})},"bus $form.ready:last + time.resolve":function(e,$form,promise){var $element=$form.find(".tau-chart-time-switch button.tau-checked");promise.resolve({key:$element.data("val"),label:$element.data("axislabel"),startDateFilter:$element.data("startdatefilter")})}})})