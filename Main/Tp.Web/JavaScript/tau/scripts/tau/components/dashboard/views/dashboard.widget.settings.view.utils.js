define(["require","jQuery","libs/react/react-ex","jsx!./dashboard.widget.settings.view"],function(t){var e=t("jQuery"),n=t("libs/react/react-ex"),i=t("jsx!./dashboard.widget.settings.view");return{toggleWidgetSettings:function(t,o,a,s){var r=o.closest(".i-role-dashboard-layout"),l={settingsComponent:null},d=function(){var t=!!o.tauBubble("instance");return t&&o.tauBubble("destroy"),s(!1),l.settingsComponent=null,t};return d()?e.Deferred().resolve(l):t.insertSettingsIntoNewContainer().then(function(u){var b=document.createElement("div");return l.settingsComponent=n.renderClass(i,{settingsContent:u.container,initialSizes:t.getSizes(),isEditable:a,applySettings:function(e){t.setLayoutSettingsSilently({aspectRatio:e.state.widgetAspectRatio,height:e.state.widgetHeight}),u.applySettings(),d()}},b),o.tauBubble({target:o,hideOnScrollContainer:o.scrollParent(),within:r.closest(".tau-dashboard"),appendTo:r,showArrow:!0,content:e(b),onPositionConfig:function(t){t.my="center top",t.at="center bottom",t.collision="flipfit flipfit"},onShow:function(t){s(!0),a?t.find(":input:visible:enabled:first, [tabindex]:visible:first").first().focus():t.find(":input:visible:enabled").attr("disabled","disabled")},onHide:function(t,e){e.preventDefault&&e.preventDefault(),d()},template:['<div class="tau-bubble-board">','    <div class="tau-bubble-board__arrow" role="arrow" data-orientation="top"></div>','    <div class="tau-bubble-board__inner tau-container" role="content"></div>',"</div>"].join(""),zIndex:999,dontCloseSelectors:[".ui-datepicker"]}),o.tauBubble("show"),l})}}});