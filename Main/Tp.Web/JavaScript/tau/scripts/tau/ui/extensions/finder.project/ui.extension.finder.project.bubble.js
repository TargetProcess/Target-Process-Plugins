define(["Underscore","jQuery","tau/core/extension.base"],function(_,$,ExtensionBase){return ExtensionBase.extend({"bus configurator.ready + $el.ready":function(evt,configurator,$el){var self=this,$bubbled=$el,$content=$el;this.fire("$list.ready",$content),this.fire("$bubbled.ready",$bubbled),this.fire("$popup.ready",$content),this.fire("$teamSection.ready",$()),this.fire("$projectSection.ready",$el),this.fire("$sections.ready",$el)}})})