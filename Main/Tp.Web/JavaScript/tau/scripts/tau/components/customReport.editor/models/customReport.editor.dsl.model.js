define(["require","tau/core/extension.base","tau/services/customReport/service.customReport.settings.wrapper"],function(t){var e=t("tau/core/extension.base"),r=t("tau/services/customReport/service.customReport.settings.wrapper");return e.extend({init:function(t){this._super(t),this._reportSettingsWrapper=new r},"bus boardSettings.ready:last + afterInit + configurator.ready:last":function(t,e,r,i){var s=i.getLoggedUser(),n=i.getBoardAccessService(),o=n.isEditable(e.boardSettings.settings,s);this._reportSettingsWrapper.getReportSettings(e.boardSettings).then(function(t){this.fire("dataBind",{report:JSON.stringify(t.report,null,4),dataSource:JSON.stringify(t.dataSource,null,4),isEditable:o})}.bind(this))},"bus beforeInit":function(t,e){var r=e.config.context.configurator;this.fire("configurator.ready",r)}})});