Ext.ns('Tp.controls.project');

Tp.controls.project.ProjectDataContainer = Ext.extend(Object, {
    hashTable:{},
    constructor:function(config){
        Ext.apply(this,config);
        for(var i=0;i<config.length;i++){
            this.hashTable[config[i].projectId] = config[i];
        }
    },

    getProjectData:function(projectId){
        return this.hashTable[projectId];
    }
});