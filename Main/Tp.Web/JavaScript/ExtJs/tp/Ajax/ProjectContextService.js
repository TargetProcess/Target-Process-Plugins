Ext.ns('ExtJs.tp.Ajax');

ExtJs.tp.Ajax.ProjectContextService = Ext.extend(Ext.util.Observable, {
	constructor: function(config) {

		config = Ext.apply({

	}, config);

	ExtJs.tp.Ajax.ProjectContextService.superclass.constructor.call(this, config);
},

onSetProjectAsCurrentSucceed: function(response) {
	var uri = jsonParse(response.responseText).d.uri;
	document.location.href = uri;
},

onAccessServiceFailure: function(response) {
	Ext.MessageBox.show({
		title: response.statusText,
		msg: "Failed to access to ProjectContextService:<br>" + response.responseText,
		buttons: Ext.MessageBox.OK,
		icon: Ext.MessageBox.ERROR
	});
},

setCurrent: function(projectIds) {
	if (projectIds.constructor != Array) {
		projectIds = [projectIds];
	}

	Ext.Ajax.request({
		url: appHostAndPath + '/Wcf/ProjectContextService.asmx/SetCurrent',
		headers: { 'Content-Type': 'application/json' },
		success: this.onSetProjectAsCurrentSucceed,
		failure: Function.createDelegate(this, this.onAccessServiceFailure),
		jsonData: { 'projectIds': projectIds }
	});
},

getSerializedProjectContextParameter: function(selectedProjectId, acid, onSucceed) {
	Ext.Ajax.request({
		url: appHostAndPath + '/Wcf/ProjectContextService.asmx/GetSerializedProjectContextParameter',
		headers: { 'Content-Type': 'application/json' },
		success: onSucceed,
		failure: Function.createDelegate(this, this.onAccessServiceFailure),
		jsonData: { 'selectedProjectId': selectedProjectId, 'acid': acid }
	});
}

});

var projectContextService = null;

Ext.onReady(function (){
    projectContextService =  new ExtJs.tp.Ajax.ProjectContextService();
});