define(["jQuery","tau/core/extension.base"],function(t,e){return e.extend({"bus afterRender + afterInit":function(e,n,o){var i=o.config.context.configurator,r=i.getApplicationPath(),a=i.loggedUser.isAdministrator,p=a?"/Admin/GlobalSetting.aspx":"/PersonalSettings.aspx",u=r+p+"?rmnav=1&tp3=1",c=n.element.find(".i-role-settings");c.tauIFramePopup({url:u}),i.getFeaturesService().isEnabled("contextPrototype")&&c.on("tauiframepopuphide",function(){i.getApplicationContextService().getApplicationContext({}).done(function(t){i.getPrototypeContextEntityService().updateContextPrototype(t)})}),c.click(function(){t(this).tauIFramePopup("show")})}})});