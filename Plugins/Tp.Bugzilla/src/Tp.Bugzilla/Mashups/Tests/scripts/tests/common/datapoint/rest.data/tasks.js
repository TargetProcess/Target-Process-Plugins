define(["Underscore","tests/common/datapoint/rest.data/entityStates","tests/common/datapoint/rest.data/projects"],function(a,b,c){return{getTasks:function(){var d=c.getProjects(),e=function(b){return a(d).detect(function(a){return a.id===b})},f=b.getEntityStates(),g=[{id:31,name:"Task Name1",__type:"task",tags:["task","urgent"],entityState:a(f).detect(function(a){return a.name=="Open"&&a.entityType.name=="Task"}),project:e(1),priority:{id:1,__type:"priority",name:"Must have",importance:1},effort:6,effortCompleted:2,effortToDo:4,timeSpent:1,timeRemain:12,roleEfforts:[{id:318,role:{id:1,name:"Developer",__type:"role"}}],assignments:[{id:309,role:{id:1,name:"Developer",__type:"role"},generalUser:{id:1,firstName:"Vasili",lastName:"Ivanov"}},{id:310,role:{id:1,name:"Developer",__type:"role"},generalUser:{id:2,firstName:"Ivan",lastName:"Vasilijev"}}]},{id:33,name:"Task Name3",__type:"task",tags:[],entityState:a(f).detect(function(a){return a.name=="Done"&&a.entityType.name=="Task"}),project:e(1),priority:{id:2,__type:"priority",name:"Nice to have",importance:5},effort:34,effortCompleted:0,effortToDo:0,timeSpent:1,timeRemain:33,roleEfforts:[{id:318,role:{id:1,name:"Developer",__type:"role"}}],assignments:[{id:309,role:{id:1,name:"Developer",__type:"role"},generalUser:{id:1,firstName:"Vasili",lastName:"Ivanov"}},{id:310,role:{id:1,name:"Developer",__type:"role"},generalUser:{id:2,firstName:"Ivan",lastName:"Vasilijev"}}]}];return g}}})