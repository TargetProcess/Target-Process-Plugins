define(["require","jQuery"],function(t){var e=t("jQuery"),l=3,n=200;return{componentDidMount:function(){var t=e(this.getDOMNode()),o=t.find(".tau-board-settings__template-list"),i=t.find(".tau-nav-left"),a=t.find(".tau-nav-right"),f=function(){i.toggle(o[0].scrollLeft>0),a.toggle(o[0].scrollWidth-o[0].scrollLeft-o[0].offsetWidth>0)};this._toggleIntervalId=setInterval(f,150),t.on("mousewheel.templateListScrollMixin",".tau-board-settings__template-list",function(t){t.preventDefault(),this.scrollLeft-=t.deltaY*t.deltaFactor*l}),t.on("click",".tau-nav-right",function(){o.animate({scrollLeft:o[0].scrollLeft+o[0].offsetWidth-n},250)}),t.on("click",".tau-nav-left",function(){o.animate({scrollLeft:o[0].scrollLeft-o[0].offsetWidth-n},250)})},componentWillUnmount:function(){e(this.getDOMNode()).off(".templateListScrollMixin"),clearInterval(this._toggleIntervalId)}}});