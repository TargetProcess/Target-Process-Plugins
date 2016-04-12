Ext.ns('Tp.controls.project');

Tp.controls.project.ProjectIdContainer = Ext.extend(Object, {
    _SELECTED_PROJECT_COOKIE_KEY: "previousSelectedProject",
	set: function(projectId) {
        Ext.util.Cookies.set(this._SELECTED_PROJECT_COOKIE_KEY, projectId, new Date().add(Date.YEAR, 1));
	},

    get: function() {
        return Ext.util.Cookies.get(this._SELECTED_PROJECT_COOKIE_KEY);
	}
});