define([],function(){var e=function(){var e=this.configurator.getLoggedUser().id,t=this.configurator.getBoardSettingsFactory().createComponentSettings({id:"user"});t.set({set:{generalTab:"accountSettings"},callback:function(){this.resolve({entity:{id:e,type:"user"},cssClass:"tau-app tau-page-single tau-page-entity i-role-bubble-holder",handlers:{onPageLoaded:function(){this.configurator.service("errorBar").fire("notification",{message:"We created a new TP user for you, but didn't have enough details. Please change “John Doe” to your real name."})}.bind(this)}})}.bind(this)})};return e});