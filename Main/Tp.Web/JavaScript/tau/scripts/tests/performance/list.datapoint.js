define([],function(){function prepareTestData(limit){var testData=[],severity={id:1,importance:1,__type:"severity"};testData.push(severity);var priorities={id:11,importance:1,__type:"priority"};testData.push(priorities);var projects={id:22,process:{id:1,__type:"process"},__type:"project"};testData.push(projects);var stateUserStory={id:33,name:"Done",isFinal:!0,isInitial:!1,isPlanned:!1,isCommentRequired:!1,numericPriority:65,entityType:{id:4,name:"UserStory",__type:"entityType"},process:{id:1,__type:"process"},nextStates:[],__type:"entityState"};testData.push(stateUserStory);var stateTask1={id:331,name:"Open",isFinal:!1,isInitial:!0,isPlanned:!1,isCommentRequired:!1,numericPriority:1,entityType:{id:1,name:"Task",__type:"entityType"},process:{id:1,__type:"process"},nextStates:[],__type:"entityState"};testData.push(stateTask1);var stateTask2={id:332,name:"Done",isFinal:!0,isInitial:!1,isPlanned:!1,isCommentRequired:!1,numericPriority:2,entityType:{id:1,name:"Task",__type:"entityType"},process:{id:1,__type:"process"},nextStates:[],__type:"entityState"};testData.push(stateTask2);var story={id:370,__type:"userStory",bugs:[],tasks:[]},createTask=function(i){return{id:1+i,name:"Development",numericPriority:151,tags:"",effort:6,effortCompleted:6,effortToDo:0,timeSpent:11,timeRemain:0,entityState:{id:33,name:"Done",isInitial:!1,isFinal:!0,numericPriority:65,__type:"entityState"},roleEfforts:[{id:44,effort:6,effortToDo:0,role:{id:1,name:"Developer",isPair:!0,hasEffort:!0,__type:"role"},__type:"roleEffort"}],owner:{id:2,firstName:"Target",lastName:"Process",__type:"generalUser"},assignments:[],priority:{id:11,name:"-",importance:1,__type:"priority"},project:{id:22,__type:"project"},__type:"task"}};for(var i=0;i<limit;i++)story.tasks.push(createTask(i));return testData.push(story),testData}return{prepareTestData:prepareTestData}})